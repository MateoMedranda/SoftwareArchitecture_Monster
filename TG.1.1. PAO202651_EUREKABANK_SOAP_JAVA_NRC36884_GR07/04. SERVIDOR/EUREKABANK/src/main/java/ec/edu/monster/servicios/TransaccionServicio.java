package ec.edu.monster.servicios;

import ec.edu.monster.configuracion.TokenGenerador;
import ec.edu.monster.controladores.TokenFiltro;
import ec.edu.monster.modelos.DAO.CuentaDAO;
import ec.edu.monster.modelos.DAO.PerteneceDAO;
import ec.edu.monster.modelos.DAO.RolDAO;
import ec.edu.monster.modelos.DAO.TransaccionDAO;
import ec.edu.monster.modelos.DAO.UsuarioDAO;
import ec.edu.monster.modelos.DTO.CuentaDTO;
import ec.edu.monster.modelos.DTO.PerteneceDTO;
import ec.edu.monster.modelos.DTO.RolDTO;
import ec.edu.monster.modelos.DTO.TransaccionDTO;
import ec.edu.monster.modelos.DTO.UsuarioDTO;
import ec.edu.monster.modelos.Respuestas.RespuestaError;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import io.jsonwebtoken.Claims;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import java.math.BigDecimal;
import java.sql.SQLException;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Stateless
public class TransaccionServicio {

    @EJB private CuentaDAO cuentaDAO;
    @EJB private TransaccionDAO transaccionDAO;
    @EJB private UsuarioDAO usuarioDAO;
    @EJB private PerteneceDAO perteneceDAO;
    @EJB private RolDAO rolDAO;
    @EJB private TokenGenerador tokenGenerador;

    private Claims validarToken(String token) throws TransaccionExcepcion {
        try { return TokenFiltro.verificar(token, tokenGenerador); }
        catch (ec.edu.monster.modelos.Respuestas.LoginExcepcion e) { throw new TransaccionExcepcion(e.getMessage(), e.getFaultInfo()); }
    }

    private String rolNombre(String rolId) {
        try { RolDTO r = rolDAO.obtenerPorId(rolId); return (r != null) ? r.getRolNombre() : ""; }
        catch (SQLException e) { return ""; }
    }

    private boolean esAdminOCajero(Claims claims) {
        String rn = rolNombre(claims.get("rol", String.class));
        return rn.equals("ADMIN") || rn.equals("CAJERO");
    }

    private boolean cuentaAsignada(String cuentaId, String usuarioId) {
        try {
            List<PerteneceDTO> asig = perteneceDAO.listarPorUsuario(usuarioId);
            for (PerteneceDTO p : asig) if (p.getCuentaId().equals(cuentaId)) return true;
        } catch (SQLException ignored) {}
        return false;
    }

    private void validarPermisoCuenta(String cuentaId, Claims claims) throws TransaccionExcepcion {
        try {
            CuentaDTO cuenta = cuentaDAO.obtenerPorId(cuentaId);
            if (cuenta == null) throw new TransaccionExcepcion("Cuenta no encontrada", new RespuestaError("CUENTA_NO_ENCONTRADA","La cuenta indicada no existe.","cuentaId"));
            if (esAdminOCajero(claims)) return;
            if (!cuentaAsignada(cuentaId, claims.getSubject()))
                throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","No tiene permiso para operar sobre esta cuenta.","cuentaId"));
        } catch (TransaccionExcepcion e) { throw e; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo validar permisos.", e.getMessage()), e); }
    }

    private void validarBasico(String tipo, String cuentaId, BigDecimal monto) throws TransaccionExcepcion {
        if (tipo == null || tipo.trim().isEmpty()) throw new TransaccionExcepcion("Tipo requerido", new RespuestaError("CAMPO_REQUERIDO","El campo 'tipo' es obligatorio.","tipo"));
        String t = tipo.trim().toUpperCase();
        if (!t.equals("INGRESO") && !t.equals("RETIRO")) throw new TransaccionExcepcion("Tipo invalido", new RespuestaError("TIPO_INVALIDO","El tipo debe ser INGRESO o RETIRO.","tipo"));
        if (cuentaId == null || cuentaId.trim().isEmpty()) throw new TransaccionExcepcion("Cuenta requerida", new RespuestaError("CAMPO_REQUERIDO","El campo 'cuentaId' es obligatorio.","cuentaId"));
        if (monto == null || monto.compareTo(BigDecimal.ZERO) <= 0) throw new TransaccionExcepcion("Monto invalido", new RespuestaError("MONTO_INVALIDO","El monto debe ser mayor a cero.","monto"));
    }

    public TransaccionDTO generarTransaccion(String token, String tipo, String cuentaId, BigDecimal monto) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        validarBasico(tipo, cuentaId, monto);
        String tipoN = tipo.trim().toUpperCase();
        String creadoPor = claims.getSubject();
        try {
            CuentaDTO cuenta = cuentaDAO.obtenerPorId(cuentaId.trim());
            if (cuenta == null) throw new TransaccionExcepcion("Cuenta no encontrada", new RespuestaError("CUENTA_NO_ENCONTRADA","La cuenta indicada no existe.","cuentaId"));
            validarPermisoCuenta(cuentaId.trim(), claims);
            if (!"ACTIVA".equalsIgnoreCase(cuenta.getCuentaEstado())) throw new TransaccionExcepcion("Cuenta inactiva", new RespuestaError("CUENTA_INACTIVA","La cuenta no esta activa.","cuentaId"));
            BigDecimal saldoAnterior = cuenta.getCuentaSaldo();
            BigDecimal saldoPosterior;
            if (tipoN.equals("INGRESO")) saldoPosterior = saldoAnterior.add(monto);
            else {
                if (saldoAnterior.compareTo(monto) < 0) throw new TransaccionExcepcion("Saldo insuficiente", new RespuestaError("SALDO_INSUFICIENTE","La cuenta no tiene saldo suficiente para el retiro.","monto"));
                saldoPosterior = saldoAnterior.subtract(monto);
            }
            TransaccionDTO t = new TransaccionDTO();
            t.setTransaccionId(UUID.randomUUID().toString());
            t.setCuentaId(cuentaId.trim());
            t.setTransaccionSaldo(monto);
            t.setTransaccionTipo(tipoN);
            t.setTransaccionSaldoAnterior(saldoAnterior);
            t.setTransaccionSaldoPosterior(saldoPosterior);
            t.setTransaccionCreadoEn(LocalDateTime.now());
            t.setTransaccionCreadoPor(creadoPor);
            transaccionDAO.insertar(t);
            cuenta.setCuentaSaldo(saldoPosterior);
            cuentaDAO.actualizar(cuenta);
            return t;
        } catch (TransaccionExcepcion e) { throw e; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo procesar la transaccion.", e.getMessage()), e); }
    }

    public TransaccionDTO obtenerTransaccion(String token, String transaccionId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (transaccionId == null || transaccionId.trim().isEmpty()) throw new TransaccionExcepcion("Id requerido", new RespuestaError("CAMPO_REQUERIDO","El campo 'transaccionId' es obligatorio.","transaccionId"));
        try {
            TransaccionDTO t = transaccionDAO.obtenerPorId(transaccionId.trim());
            if (t == null) throw new TransaccionExcepcion("No encontrada", new RespuestaError("TRANSACCION_NO_ENCONTRADA","La transaccion indicada no existe.","transaccionId"));
            // CLIENTE solo si la transaccion es de su cuenta
            if (!esAdminOCajero(claims)) {
                CuentaDTO cuenta = cuentaDAO.obtenerPorId(t.getCuentaId());
                if (cuenta == null || !cuentaAsignada(t.getCuentaId(), claims.getSubject()))
                    throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","No tiene permiso para ver esta transaccion.","transaccionId"));
            }
            return t;
        } catch (TransaccionExcepcion e) { throw e; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo obtener la transaccion.", e.getMessage()), e); }
    }

    public List<TransaccionDTO> listarTransacciones(String token) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        try {
            List<TransaccionDTO> all = transaccionDAO.listar();
            if (esAdminOCajero(claims)) return all;
            // CLIENTE solo las de sus cuentas
            List<TransaccionDTO> res = new ArrayList<>();
            for (TransaccionDTO t : all) {
                if (cuentaAsignada(t.getCuentaId(), claims.getSubject())) res.add(t);
            }
            return res;
        } catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo listar las transacciones.", e.getMessage()), e); }
    }

    public List<TransaccionDTO> listarPorCuenta(String token, String cuentaId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (cuentaId == null || cuentaId.trim().isEmpty()) throw new TransaccionExcepcion("Cuenta requerida", new RespuestaError("CAMPO_REQUERIDO","El campo 'cuentaId' es obligatorio.","cuentaId"));
        validarPermisoCuenta(cuentaId.trim(), claims);
        try { return transaccionDAO.listarPorCuenta(cuentaId.trim()); }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo listar las transacciones.", e.getMessage()), e); }
    }

    public List<TransaccionDTO> listarPorUsuario(String token, String usuarioId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (usuarioId == null || usuarioId.trim().isEmpty()) throw new TransaccionExcepcion("Usuario requerido", new RespuestaError("CAMPO_REQUERIDO","El campo 'usuarioId' es obligatorio.","usuarioId"));
        if (!esAdminOCajero(claims) && !claims.getSubject().equals(usuarioId.trim()))
            throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","No puede consultar transacciones de otro usuario.","usuarioId"));
        try {
            List<TransaccionDTO> all = transaccionDAO.listar();
            List<TransaccionDTO> res = new ArrayList<>();
            for (TransaccionDTO t : all) if (usuarioId.trim().equals(t.getTransaccionCreadoPor())) res.add(t);
            return res;
        } catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo listar las transacciones.", e.getMessage()), e); }
    }

    public boolean eliminarTransaccion(String token, String transaccionId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (transaccionId == null || transaccionId.trim().isEmpty()) throw new TransaccionExcepcion("Id requerido", new RespuestaError("CAMPO_REQUERIDO","El campo 'transaccionId' es obligatorio.","transaccionId"));
        if (!esAdminOCajero(claims)) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN o CAJERO pueden eliminar transacciones.","transaccionId"));
        try {
            TransaccionDTO t = transaccionDAO.obtenerPorId(transaccionId.trim());
            if (t == null) throw new TransaccionExcepcion("No encontrada", new RespuestaError("TRANSACCION_NO_ENCONTRADA","La transaccion indicada no existe.","transaccionId"));
            transaccionDAO.eliminar(transaccionId.trim());
            return true;
        } catch (TransaccionExcepcion e) { throw e; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo eliminar la transaccion.", e.getMessage()), e); }
    }
}
