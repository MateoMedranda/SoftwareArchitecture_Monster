package ec.edu.monster.servicios;

import ec.edu.monster.configuracion.TokenGenerador;
import ec.edu.monster.controladores.TokenFiltro;
import ec.edu.monster.modelos.DAO.RolDAO;
import ec.edu.monster.modelos.DAO.UsuarioDAO;
import ec.edu.monster.modelos.DTO.RolDTO;
import ec.edu.monster.modelos.DTO.UsuarioDTO;
import ec.edu.monster.modelos.Respuestas.RespuestaError;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import io.jsonwebtoken.Claims;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import java.sql.SQLException;
import java.util.List;

@Stateless
public class UsuarioServicio {

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

    private boolean esAdmin(Claims claims) { return rolNombre(claims.get("rol", String.class)).equals("ADMIN"); }
    private boolean esAdminOCajero(Claims claims) {
        String rn = rolNombre(claims.get("rol", String.class));
        return rn.equals("ADMIN") || rn.equals("CAJERO");
    }

    public boolean insertarUsuario(String token, UsuarioDTO usuario) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdmin(claims)) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN puede insertar usuarios.","token"));
        try { usuarioDAO.insertar(usuario); return true; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo insertar el usuario.", e.getMessage()), e); }
    }

    public boolean actualizarUsuario(String token, UsuarioDTO usuario) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdmin(claims)) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN puede actualizar usuarios.","token"));
        try { usuarioDAO.actualizar(usuario); return true; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo actualizar el usuario.", e.getMessage()), e); }
    }

    public boolean eliminarUsuario(String token, String usuarioId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdmin(claims)) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN puede eliminar usuarios.","token"));
        try { usuarioDAO.eliminar(usuarioId); return true; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo eliminar el usuario.", e.getMessage()), e); }
    }

    public UsuarioDTO obtenerUsuario(String token, String usuarioId) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdminOCajero(claims) && !claims.getSubject().equals(usuarioId))
            throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","No tiene acceso a este usuario.","usuarioId"));
        try {
            UsuarioDTO u = usuarioDAO.obtenerPorId(usuarioId);
            if (u == null) throw new TransaccionExcepcion("No encontrado", new RespuestaError("USUARIO_NO_ENCONTRADO","El usuario indicado no existe.","usuarioId"));
            u.setUsuarioContraseania(null); // no exponer hash
            return u;
        } catch (TransaccionExcepcion e) { throw e; }
        catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo obtener el usuario.", e.getMessage()), e); }
    }

    public List<UsuarioDTO> listarUsuarios(String token) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdminOCajero(claims)) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN o CAJERO pueden listar usuarios.","token"));
        try {
            List<UsuarioDTO> lista = usuarioDAO.listar();
            for (UsuarioDTO u : lista) u.setUsuarioContraseania(null);
            return lista;
        } catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo listar los usuarios.", e.getMessage()), e); }
    }

    public List<UsuarioDTO> buscarUsuarioPorNombre(String token, String nombre) throws TransaccionExcepcion {
        Claims claims = validarToken(token);
        if (!esAdminOCajero(claims)) throw new TransaccionExcepcion("Acceso denegado", new RespuestaError("ACCESO_DENEGADO","Solo ADMIN o CAJERO pueden buscar usuarios.","token"));
        try {
            List<UsuarioDTO> all = usuarioDAO.listar();
            List<UsuarioDTO> res = new java.util.ArrayList<>();
            for (UsuarioDTO u : all) {
                if (u.getUsuarioNombre().toLowerCase().contains(nombre.toLowerCase()) ||
                    u.getUsuarioUsuario().toLowerCase().contains(nombre.toLowerCase())) {
                    u.setUsuarioContraseania(null);
                    res.add(u);
                }
            }
            return res;
        } catch (SQLException e) { throw new TransaccionExcepcion("Error de base de datos", new RespuestaError("ERROR_BASE_DATOS","No se pudo buscar usuarios.", e.getMessage()), e); }
    }
}
