package ec.edu.monster.servicios;

import ec.edu.monster.configuracion.TokenGenerador;
import ec.edu.monster.conexion.ConexionBDD;
import ec.edu.monster.controladores.TokenFiltro;
import ec.edu.monster.modelos.DAO.CuentaDAO;
import ec.edu.monster.modelos.DAO.PerteneceDAO;
import ec.edu.monster.modelos.DAO.RolDAO;
import ec.edu.monster.modelos.DAO.TransaccionDAO;
import ec.edu.monster.modelos.DTO.CuentaDTO;
import ec.edu.monster.modelos.DTO.PerteneceDTO;
import ec.edu.monster.modelos.DTO.RolDTO;
import ec.edu.monster.modelos.DTO.TransaccionDTO;
import ec.edu.monster.modelos.DTO.TransferenciaDTO;
import ec.edu.monster.modelos.Respuestas.RespuestaError;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import io.jsonwebtoken.Claims;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import java.math.BigDecimal;
import java.sql.Connection;
import java.sql.SQLException;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

/** Operaciones monetarias atómicas y su historial contable. */
@Stateless
public class TransaccionServicio {

    @EJB private CuentaDAO cuentaDAO;
    @EJB private TransaccionDAO transaccionDAO;
    @EJB private PerteneceDAO perteneceDAO;
    @EJB private RolDAO rolDAO;
    @EJB private TokenGenerador tokenGenerador;
    @EJB private ConexionBDD conexionBDD;
    @EJB private DisponibilidadCuentaServicio disponibilidadCuentaServicio;

    @FunctionalInterface
    private interface TrabajoBD<T> {
        T ejecutar(Connection conexion) throws SQLException, TransaccionExcepcion;
    }

    private Claims validarToken(String token) throws TransaccionExcepcion {
        try { return TokenFiltro.verificar(token, tokenGenerador); }
        catch (ec.edu.monster.modelos.Respuestas.LoginExcepcion e) {
            throw new TransaccionExcepcion(e.getMessage(), e.getFaultInfo());
        }
    }

    private String rolNombre(String rolId) {
        try {
            RolDTO rol = rolDAO.obtenerPorId(rolId);
            return rol == null ? "" : rol.getRolNombre();
        } catch (SQLException e) {
            return "";
        }
    }

    private boolean esAdminOCajero(Claims claims) {
        String rol = rolNombre(claims.get("rol", String.class));
        return "ADMIN".equals(rol) || "CAJERO".equals(rol);
    }

    private boolean cuentaAsignada(String cuentaId, String usuarioId) {
        try {
            for (PerteneceDTO asignacion : perteneceDAO.listarPorUsuario(usuarioId)) {
                if (cuentaId.equals(asignacion.getCuentaId())) return true;
            }
        } catch (SQLException ignored) {
            // La operación será denegada si no se puede probar la pertenencia.
        }
        return false;
    }

    private void validarPermisoCuenta(String cuentaId, Claims claims) throws TransaccionExcepcion {
        if (!esAdminOCajero(claims) && !cuentaAsignada(cuentaId, claims.getSubject())) {
            throw error("ACCESO_DENEGADO", "No tiene permiso para operar sobre esta cuenta.", "cuentaId");
        }
    }

    /** Mantiene el UUID como clave interna, permitiendo que el cliente envíe el número público. */
    private String resolverCuentaId(String referencia) throws TransaccionExcepcion {
        try {
            String id = cuentaDAO.resolverId(referencia);
            if (id == null) throw error("CUENTA_NO_ENCONTRADA", "El número de cuenta indicado no existe.", "cuentaNumero");
            return id;
        } catch (SQLException e) {
            throw errorBaseDatos("No se pudo resolver el número de cuenta.", e);
        }
    }

    private void validarTipoYMonto(String tipo, String cuentaId, BigDecimal monto) throws TransaccionExcepcion {
        if (tipo == null || tipo.trim().isEmpty()) {
            throw error("CAMPO_REQUERIDO", "El campo 'tipo' es obligatorio.", "tipo");
        }
        String tipoNormalizado = tipo.trim().toUpperCase();
        if (!"INGRESO".equals(tipoNormalizado) && !"RETIRO".equals(tipoNormalizado)) {
            throw error("TIPO_INVALIDO", "El tipo debe ser INGRESO o RETIRO.", "tipo");
        }
        validarCuentaYMonto(cuentaId, monto);
    }

    private void validarCuentaYMonto(String cuentaId, BigDecimal monto) throws TransaccionExcepcion {
        if (cuentaId == null || cuentaId.trim().isEmpty()) {
            throw error("CAMPO_REQUERIDO", "El campo 'cuentaId' es obligatorio.", "cuentaId");
        }
        if (monto == null || monto.compareTo(BigDecimal.ZERO) <= 0) {
            throw error("MONTO_INVALIDO", "El monto debe ser mayor a cero.", "monto");
        }
    }

    /** Registra un depósito o retiro junto con el saldo en una única transacción JDBC. */
    public TransaccionDTO generarTransaccion(String token, String tipo, String cuentaId, BigDecimal monto)
            throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        validarTipoYMonto(tipo, cuentaId, monto);
        String cuentaNormalizada = resolverCuentaId(cuentaId);
        String tipoNormalizado = tipo.trim().toUpperCase();
        validarPermisoCuenta(cuentaNormalizada, claims);

        return disponibilidadCuentaServicio.ejecutarExclusivo(List.of(cuentaNormalizada), () ->
            ejecutarEnTransaccion(conexion -> {
                CuentaDTO cuenta = obtenerCuentaActivaParaActualizar(conexion, cuentaNormalizada);
                BigDecimal saldoAnterior = cuenta.getCuentaSaldo();
                BigDecimal saldoPosterior = calcularSaldo(tipoNormalizado, saldoAnterior, monto);
                TransaccionDTO movimiento = crearMovimiento(
                        cuentaNormalizada, monto, tipoNormalizado, saldoAnterior, saldoPosterior,
                        claims.getSubject(), LocalDateTime.now());
                transaccionDAO.insertar(conexion, movimiento);
                cuentaDAO.actualizarSaldo(conexion, cuentaNormalizada, saldoPosterior);
                return movimiento;
            })
        );
    }

    /** Mueve fondos entre dos cuentas y confirma los cuatro cambios como una unidad. */
    public TransferenciaDTO realizarTransferencia(String token, String cuentaOrigenId,
            String cuentaDestinoId, BigDecimal monto) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        validarCuentaYMonto(cuentaOrigenId, monto);
        validarCuentaYMonto(cuentaDestinoId, monto);
        String origenId = resolverCuentaId(cuentaOrigenId);
        String destinoId = resolverCuentaId(cuentaDestinoId);
        if (origenId.equals(destinoId)) {
            throw error("CUENTAS_IGUALES", "La cuenta de origen y destino deben ser diferentes.", "cuentaDestinoId");
        }
        validarPermisoCuenta(origenId, claims);

        return disponibilidadCuentaServicio.ejecutarExclusivo(List.of(origenId, destinoId), () ->
            ejecutarEnTransaccion(conexion -> {
                // El orden estable de bloqueos evita interbloqueos entre transferencias cruzadas.
                CuentaDTO primera = obtenerCuentaActivaParaActualizar(conexion,
                        origenId.compareTo(destinoId) <= 0 ? origenId : destinoId);
                CuentaDTO segunda = obtenerCuentaActivaParaActualizar(conexion,
                        origenId.compareTo(destinoId) <= 0 ? destinoId : origenId);
                CuentaDTO origen = origenId.equals(primera.getCuentaId()) ? primera : segunda;
                CuentaDTO destino = destinoId.equals(primera.getCuentaId()) ? primera : segunda;

                BigDecimal saldoOrigenAnterior = origen.getCuentaSaldo();
                if (saldoOrigenAnterior.compareTo(monto) < 0) {
                    throw error("SALDO_INSUFICIENTE", "La cuenta de origen no tiene saldo suficiente.", "monto");
                }
                BigDecimal saldoOrigenPosterior = saldoOrigenAnterior.subtract(monto);
                BigDecimal saldoDestinoAnterior = destino.getCuentaSaldo();
                BigDecimal saldoDestinoPosterior = saldoDestinoAnterior.add(monto);
                LocalDateTime fecha = LocalDateTime.now();
                String transferenciaId = UUID.randomUUID().toString();
                String referencia = transferenciaId.substring(0, 8);

                TransaccionDTO debito = crearMovimiento(origenId, monto,
                        "TRANSFER_SAL#" + referencia, saldoOrigenAnterior, saldoOrigenPosterior,
                        claims.getSubject(), fecha);
                TransaccionDTO credito = crearMovimiento(destinoId, monto,
                        "TRANSFER_ING#" + referencia, saldoDestinoAnterior, saldoDestinoPosterior,
                        claims.getSubject(), fecha);
                transaccionDAO.insertar(conexion, debito);
                transaccionDAO.insertar(conexion, credito);
                cuentaDAO.actualizarSaldo(conexion, origenId, saldoOrigenPosterior);
                cuentaDAO.actualizarSaldo(conexion, destinoId, saldoDestinoPosterior);

                TransferenciaDTO resultado = new TransferenciaDTO();
                resultado.setTransferenciaId(transferenciaId);
                resultado.setCuentaOrigenId(origenId);
                resultado.setCuentaDestinoId(destinoId);
                resultado.setMonto(monto);
                resultado.setDebito(debito);
                resultado.setCredito(credito);
                resultado.setCreadoEn(fecha);
                return resultado;
            })
        );
    }

    public TransaccionDTO obtenerTransaccion(String token, String transaccionId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (transaccionId == null || transaccionId.trim().isEmpty()) {
            throw error("CAMPO_REQUERIDO", "El campo 'transaccionId' es obligatorio.", "transaccionId");
        }
        try {
            TransaccionDTO transaccion = transaccionDAO.obtenerPorId(transaccionId.trim());
            if (transaccion == null) {
                throw error("TRANSACCION_NO_ENCONTRADA", "La transacción indicada no existe.", "transaccionId");
            }
            validarPermisoCuenta(transaccion.getCuentaId(), claims);
            return transaccion;
        } catch (SQLException e) {
            throw errorBaseDatos("No se pudo obtener la transacción.", e);
        }
    }

    public List<TransaccionDTO> listarTransacciones(String token) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        try {
            List<TransaccionDTO> todas = transaccionDAO.listar();
            if (esAdminOCajero(claims)) return todas;
            List<TransaccionDTO> propias = new ArrayList<>();
            for (TransaccionDTO transaccion : todas) {
                if (cuentaAsignada(transaccion.getCuentaId(), claims.getSubject())) propias.add(transaccion);
            }
            return propias;
        } catch (SQLException e) {
            throw errorBaseDatos("No se pudo listar las transacciones.", e);
        }
    }

    public List<TransaccionDTO> listarPorCuenta(String token, String cuentaId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (cuentaId == null || cuentaId.trim().isEmpty()) {
            throw error("CAMPO_REQUERIDO", "El campo 'cuentaId' es obligatorio.", "cuentaId");
        }
        String cuentaNormalizada = resolverCuentaId(cuentaId);
        validarPermisoCuenta(cuentaNormalizada, claims);
        try {
            return transaccionDAO.listarPorCuenta(cuentaNormalizada);
        } catch (SQLException e) {
            throw errorBaseDatos("No se pudo listar las transacciones.", e);
        }
    }

    public List<TransaccionDTO> listarPorUsuario(String token, String usuarioId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (usuarioId == null || usuarioId.trim().isEmpty()) {
            throw error("CAMPO_REQUERIDO", "El campo 'usuarioId' es obligatorio.", "usuarioId");
        }
        if (!esAdminOCajero(claims) && !claims.getSubject().equals(usuarioId.trim())) {
            throw error("ACCESO_DENEGADO", "No puede consultar transacciones de otro usuario.", "usuarioId");
        }
        try {
            List<TransaccionDTO> resultado = new ArrayList<>();
            for (TransaccionDTO transaccion : transaccionDAO.listar()) {
                if (usuarioId.trim().equals(transaccion.getTransaccionCreadoPor())) resultado.add(transaccion);
            }
            return resultado;
        } catch (SQLException e) {
            throw errorBaseDatos("No se pudo listar las transacciones.", e);
        }
    }

    /** El historial financiero es inmutable; para corregirlo se debe registrar una reversa autorizada. */
    public boolean eliminarTransaccion(String token, String transaccionId) throws TransaccionExcepcion {
        validarToken(token);
        throw error("TRANSACCION_INMUTABLE",
                "Los movimientos contables no se eliminan. Registre una operación de reversa autorizada.",
                "transaccionId");
    }

    /**
     * Compensa un depósito o retiro sin borrar el movimiento original. Las
     * transferencias se revierten con una nueva transferencia en sentido contrario.
     */
    public TransaccionDTO reversarTransaccion(String token, String transaccionId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdminOCajero(claims)) {
            throw error("ACCESO_DENEGADO", "Solo ADMIN o CAJERO pueden reversar movimientos.", "token");
        }
        if (transaccionId == null || transaccionId.trim().isEmpty()) {
            throw error("CAMPO_REQUERIDO", "El campo 'transaccionId' es obligatorio.", "transaccionId");
        }
        String idOriginal = transaccionId.trim();
        try {
            TransaccionDTO original = transaccionDAO.obtenerPorId(idOriginal);
            if (original == null) {
                throw error("TRANSACCION_NO_ENCONTRADA", "La transacción indicada no existe.", "transaccionId");
            }
            if (!"INGRESO".equals(original.getTransaccionTipo()) && !"RETIRO".equals(original.getTransaccionTipo())) {
                throw error("REVERSA_NO_ADMITIDA",
                        "Solo se pueden reversar depósitos o retiros simples. Para una transferencia use una transferencia inversa.",
                        "transaccionId");
            }
            String tipoReversa = "REV#" + idOriginal.replace("-", "");
            if (transaccionDAO.existePorTipo(tipoReversa)) {
                throw error("TRANSACCION_YA_REVERSADA", "La transacción indicada ya fue reversada.", "transaccionId");
            }
            String cuentaId = original.getCuentaId();
            return disponibilidadCuentaServicio.ejecutarExclusivo(List.of(cuentaId), () ->
                ejecutarEnTransaccion(conexion -> {
                    // Se verifica de nuevo dentro del bloqueo para impedir dos reversas simultáneas.
                    if (transaccionDAO.existePorTipo(conexion, tipoReversa)) {
                        throw error("TRANSACCION_YA_REVERSADA", "La transacción indicada ya fue reversada.", "transaccionId");
                    }
                    CuentaDTO cuenta = obtenerCuentaActivaParaActualizar(conexion, cuentaId);
                    BigDecimal saldoAnterior = cuenta.getCuentaSaldo();
                    BigDecimal saldoPosterior = "INGRESO".equals(original.getTransaccionTipo())
                            ? calcularSaldo("RETIRO", saldoAnterior, original.getTransaccionSaldo())
                            : saldoAnterior.add(original.getTransaccionSaldo());
                    TransaccionDTO reversa = crearMovimiento(cuentaId, original.getTransaccionSaldo(), tipoReversa,
                            saldoAnterior, saldoPosterior, claims.getSubject(), LocalDateTime.now());
                    transaccionDAO.insertar(conexion, reversa);
                    cuentaDAO.actualizarSaldo(conexion, cuentaId, saldoPosterior);
                    return reversa;
                })
            );
        } catch (TransaccionExcepcion e) { throw e; }
        catch (SQLException e) { throw errorBaseDatos("No se pudo reversar la transacción.", e); }
    }

    private CuentaDTO obtenerCuentaActivaParaActualizar(Connection conexion, String cuentaId)
            throws SQLException, TransaccionExcepcion {
        CuentaDTO cuenta = cuentaDAO.obtenerPorIdParaActualizar(conexion, cuentaId);
        if (cuenta == null) {
            throw error("CUENTA_NO_ENCONTRADA", "La cuenta indicada no existe.", "cuentaId");
        }
        if (!"ACTIVA".equalsIgnoreCase(cuenta.getCuentaEstado())) {
            throw error("CUENTA_INACTIVA", "La cuenta no está activa.", "cuentaId");
        }
        return cuenta;
    }

    private BigDecimal calcularSaldo(String tipo, BigDecimal saldoAnterior, BigDecimal monto)
            throws TransaccionExcepcion {
        if ("INGRESO".equals(tipo)) return saldoAnterior.add(monto);
        if (saldoAnterior.compareTo(monto) < 0) {
            throw error("SALDO_INSUFICIENTE", "La cuenta no tiene saldo suficiente para el retiro.", "monto");
        }
        return saldoAnterior.subtract(monto);
    }

    private TransaccionDTO crearMovimiento(String cuentaId, BigDecimal monto, String tipo,
            BigDecimal saldoAnterior, BigDecimal saldoPosterior, String creadoPor, LocalDateTime fecha) {
        TransaccionDTO movimiento = new TransaccionDTO();
        movimiento.setTransaccionId(UUID.randomUUID().toString());
        movimiento.setCuentaId(cuentaId);
        movimiento.setTransaccionSaldo(monto);
        movimiento.setTransaccionTipo(tipo);
        movimiento.setTransaccionSaldoAnterior(saldoAnterior);
        movimiento.setTransaccionSaldoPosterior(saldoPosterior);
        movimiento.setTransaccionCreadoEn(fecha);
        movimiento.setTransaccionCreadoPor(creadoPor);
        return movimiento;
    }

    private <T> T ejecutarEnTransaccion(TrabajoBD<T> trabajo) throws TransaccionExcepcion {
        try (Connection conexion = conexionBDD.getConexion()) {
            conexion.setAutoCommit(false);
            try {
                T resultado = trabajo.ejecutar(conexion);
                conexion.commit();
                return resultado;
            } catch (TransaccionExcepcion | SQLException e) {
                rollbackSilencioso(conexion);
                if (e instanceof TransaccionExcepcion) throw (TransaccionExcepcion) e;
                throw errorBaseDatos("No se pudo procesar la transacción.", (SQLException) e);
            } catch (RuntimeException e) {
                rollbackSilencioso(conexion);
                throw e;
            }
        } catch (SQLException e) {
            throw errorBaseDatos("No se pudo iniciar la transacción.", e);
        }
    }

    private void rollbackSilencioso(Connection conexion) {
        try { conexion.rollback(); } catch (SQLException ignored) { }
    }

    private TransaccionExcepcion error(String codigo, String mensaje, String detalle) {
        return new TransaccionExcepcion(mensaje, new RespuestaError(codigo, mensaje, detalle));
    }

    private TransaccionExcepcion errorBaseDatos(String mensaje, SQLException causa) {
        return new TransaccionExcepcion("Error de base de datos",
                new RespuestaError("ERROR_BASE_DATOS", mensaje, causa.getMessage()), causa);
    }
}
