package ec.edu.monster.servicios;

import ec.edu.monster.configuracion.TokenGenerador;
import ec.edu.monster.controladores.TokenFiltro;
import ec.edu.monster.modelos.DAO.CuentaDAO;
import ec.edu.monster.modelos.DAO.PerteneceDAO;
import ec.edu.monster.modelos.DAO.RolDAO;
import ec.edu.monster.modelos.DAO.UsuarioDAO;
import ec.edu.monster.modelos.DTO.CuentaDTO;
import ec.edu.monster.modelos.DTO.EstadoCuentaDTO;
import ec.edu.monster.modelos.DTO.PerteneceDTO;
import ec.edu.monster.modelos.DTO.RolDTO;
import ec.edu.monster.modelos.DTO.UsuarioDTO;
import ec.edu.monster.modelos.Respuestas.RespuestaError;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import io.jsonwebtoken.Claims;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

@Stateless
public class CuentaServicio {

    @EJB private CuentaDAO cuentaDAO;
    @EJB private PerteneceDAO perteneceDAO;
    @EJB private UsuarioDAO usuarioDAO;
    @EJB private RolDAO rolDAO;
    @EJB private TokenGenerador tokenGenerador;
    @EJB private DisponibilidadCuentaServicio disponibilidadCuentaServicio;

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

    /** Convierte el número público de diez dígitos en el UUID interno. */
    private String resolverCuentaId(String referencia) throws TransaccionExcepcion {
        try {
            String id = cuentaDAO.resolverId(referencia);
            if (id == null) throw new TransaccionExcepcion("No encontrada",
                    new RespuestaError("CUENTA_NO_ENCONTRADA", "El número de cuenta indicado no existe.", "cuentaNumero"));
            return id;
        } catch (SQLException e) {
            throw new TransaccionExcepcion("Error de base de datos",
                    new RespuestaError("ERROR_BASE_DATOS", "No se pudo resolver el número de cuenta.", e.getMessage()), e);
        }
    }

    public boolean insertarCuenta(String token, CuentaDTO cuenta) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdminOCajero(claims)) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN o CAJERO pueden insertar cuentas.","token"));
        if (cuenta == null || cuenta.getCuentaId() == null || cuenta.getCuentaId().trim().isEmpty())
            throw new TransaccionExcepcion("Cuenta requerida", new RespuestaError("CAMPO_REQUERIDO","El campo 'cuentaId' es obligatorio.","cuentaId"));
        if (cuenta.getCuentaNumero() == null || !cuenta.getCuentaNumero().matches("\\d{10}"))
            throw new TransaccionExcepcion("Número inválido", new RespuestaError("NUMERO_CUENTA_INVALIDO","El número de cuenta debe tener exactamente 10 dígitos.","cuentaNumero"));
        if (cuenta.getCuentaSaldo() == null || cuenta.getCuentaSaldo().compareTo(java.math.BigDecimal.ZERO) != 0)
            throw new TransaccionExcepcion("Saldo inicial inválido", new RespuestaError("SALDO_INICIAL_INVALIDO","Las cuentas nuevas deben crearse con saldo cero. Acredite fondos mediante una transacción.","cuentaSaldo"));
        try { cuentaDAO.insertar(cuenta); return true; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo insertar la cuenta.", e.getMessage()), e); }
    }

    public boolean actualizarCuenta(String token, CuentaDTO cuenta) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdminOCajero(claims)) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN o CAJERO pueden actualizar cuentas.","token"));
        if (cuenta == null || cuenta.getCuentaId() == null || cuenta.getCuentaId().trim().isEmpty())
            throw new TransaccionExcepcion("Cuenta requerida", new RespuestaError("CAMPO_REQUERIDO","El campo 'cuentaId' es obligatorio.","cuentaId"));
        try {
            // El saldo no se modifica desde mantenimiento: toda variación debe dejar
            // un movimiento contable generado por TransaccionServicio.
            cuentaDAO.actualizarDatosAdministrativos(cuenta);
            return true;
        }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo actualizar la cuenta.", e.getMessage()), e); }
    }

    public boolean eliminarCuenta(String token, String cuentaId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        String rn = rolNombre(claims.get("rol", String.class));
        if (!rn.equals("ADMIN")) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN puede eliminar cuentas.","token"));
        try { cuentaDAO.eliminar(cuentaId); return true; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo eliminar la cuenta.", e.getMessage()), e); }
    }

    public CuentaDTO obtenerCuenta(String token, String cuentaId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        String id = resolverCuentaId(cuentaId);
        if (!esAdminOCajero(claims) && !cuentaAsignada(id, claims.getSubject()))
            throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","No tiene acceso a esta cuenta.","cuentaId"));
        try {
            CuentaDTO c = cuentaDAO.obtenerPorId(id);
            if (c == null) throw new TransaccionExcepcion("No encontrada", new RespuestaError("CUENTA_NO_ENCONTRADA","La cuenta indicada no existe.","cuentaId"));
            return c;
        } catch (TransaccionExcepcion e) { throw e; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo obtener la cuenta.", e.getMessage()), e); }
    }

    public List<CuentaDTO> listarCuentas(String token) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        try {
            if (esAdminOCajero(claims)) return cuentaDAO.listar();
            // CLIENTE solo asignadas
            List<CuentaDTO> res = new ArrayList<>();
            List<PerteneceDTO> asig = perteneceDAO.listarPorUsuario(claims.getSubject());
            for (PerteneceDTO p : asig) {
                CuentaDTO c = cuentaDAO.obtenerPorId(p.getCuentaId());
                if (c != null) res.add(c);
            }
            return res;
        } catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo listar las cuentas.", e.getMessage()), e); }
    }

    public List<CuentaDTO> listarCuentasPorUsuario(String token, String usuarioId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdminOCajero(claims) && !claims.getSubject().equals(usuarioId))
            throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","No puede consultar cuentas de otro usuario.","usuarioId"));
        try {
            List<CuentaDTO> res = new ArrayList<>();
            List<PerteneceDTO> asig = perteneceDAO.listarPorUsuario(usuarioId);
            for (PerteneceDTO p : asig) {
                CuentaDTO c = cuentaDAO.obtenerPorId(p.getCuentaId());
                if (c != null) res.add(c);
            }
            return res;
        } catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo listar las cuentas.", e.getMessage()), e); }
    }

    /** Consulta el bloqueo temporal de una cuenta sin modificar la base de datos. */
    public EstadoCuentaDTO obtenerDisponibilidad(String token, String cuentaId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (cuentaId == null || cuentaId.trim().isEmpty())
            throw new TransaccionExcepcion("Cuenta requerida", new RespuestaError("CAMPO_REQUERIDO","El campo 'cuentaId' es obligatorio.","cuentaId"));
        String id = resolverCuentaId(cuentaId);
        if (!esAdminOCajero(claims) && !cuentaAsignada(id, claims.getSubject()))
            throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","No tiene acceso a esta cuenta.","cuentaId"));
        try {
            if (cuentaDAO.obtenerPorId(id) == null)
                throw new TransaccionExcepcion("No encontrada", new RespuestaError("CUENTA_NO_ENCONTRADA","La cuenta indicada no existe.","cuentaId"));
            return disponibilidadCuentaServicio.obtenerEstado(id);
        } catch (TransaccionExcepcion e) { throw e; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo consultar la cuenta.", e.getMessage()), e); }
    }

    /** Long polling SOAP para notificar a clientes cuando cambia la disponibilidad. */
    public EstadoCuentaDTO esperarCambioDisponibilidad(String token, String cuentaId,
            long versionConocida, long esperaMaximaMs) throws TransaccionExcepcion {
        // obtenerDisponibilidad también valida token, formato y autorización.
        EstadoCuentaDTO estado = obtenerDisponibilidad(token, cuentaId);
        try {
            return disponibilidadCuentaServicio.esperarCambio(estado.getCuentaId(), versionConocida, esperaMaximaMs);
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
            throw new TransaccionExcepcion("Espera interrumpida", new RespuestaError(
                    "ESPERA_INTERRUMPIDA", "La espera del cambio de disponibilidad fue interrumpida.", "cuentaId"), e);
        }
    }
}
