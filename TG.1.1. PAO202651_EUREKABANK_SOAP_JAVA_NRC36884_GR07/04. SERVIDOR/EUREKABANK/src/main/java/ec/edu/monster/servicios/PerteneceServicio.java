package ec.edu.monster.servicios;

import ec.edu.monster.configuracion.TokenGenerador;
import ec.edu.monster.controladores.TokenFiltro;
import ec.edu.monster.modelos.DAO.CuentaDAO;
import ec.edu.monster.modelos.DAO.PerteneceDAO;
import ec.edu.monster.modelos.DAO.RolDAO;
import ec.edu.monster.modelos.DAO.UsuarioDAO;
import ec.edu.monster.modelos.DTO.CuentaDTO;
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
public class PerteneceServicio {

    @EJB private PerteneceDAO perteneceDAO;
    @EJB private CuentaDAO cuentaDAO;
    @EJB private UsuarioDAO usuarioDAO;
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

    public boolean asignarCuentaUsuario(String token, String cuentaId, String usuarioId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdminOCajero(claims)) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN o CAJERO pueden asignar cuentas.","token"));
        try { PerteneceDTO p = new PerteneceDTO(); p.setCuentaId(cuentaId); p.setUsuarioId(usuarioId); perteneceDAO.insertar(p); return true; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo asignar la cuenta.", e.getMessage()), e); }
    }

    public boolean desasignarCuentaUsuario(String token, String cuentaId, String usuarioId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdminOCajero(claims)) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN o CAJERO pueden desasignar cuentas.","token"));
        try { perteneceDAO.eliminar(cuentaId, usuarioId); return true; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo desasignar la cuenta.", e.getMessage()), e); }
    }

    public List<PerteneceDTO> listarPorCuenta(String token, String cuentaId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdminOCajero(claims)) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN o CAJERO pueden listar asignaciones.","token"));
        try { return perteneceDAO.listarPorCuenta(cuentaId); }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo listar las asignaciones.", e.getMessage()), e); }
    }

    public List<PerteneceDTO> listarPorUsuario(String token, String usuarioId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdminOCajero(claims) && !claims.getSubject().equals(usuarioId))
            throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","No puede consultar asignaciones de otro usuario.","usuarioId"));
        try { return perteneceDAO.listarPorUsuario(usuarioId); }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo listar las asignaciones.", e.getMessage()), e); }
    }

    public List<CuentaDTO> buscarCuentaPorUsuario(String token, String nombreOUsuario) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdminOCajero(claims)) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN o CAJERO pueden buscar cuentas por usuario.","token"));
        try {
            List<UsuarioDTO> usuarios = usuarioDAO.listar();
            List<CuentaDTO> resultado = new ArrayList<>();
            for (UsuarioDTO u : usuarios) {
                if (u.getUsuarioNombre().toLowerCase().contains(nombreOUsuario.toLowerCase()) ||
                    u.getUsuarioUsuario().toLowerCase().contains(nombreOUsuario.toLowerCase())) {
                    List<PerteneceDTO> asig = perteneceDAO.listarPorUsuario(u.getUsuarioId());
                    for (PerteneceDTO p : asig) {
                        CuentaDTO c = cuentaDAO.obtenerPorId(p.getCuentaId());
                        if (c != null) resultado.add(c);
                    }
                }
            }
            return resultado;
        } catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo buscar cuentas.", e.getMessage()), e); }
    }
}
