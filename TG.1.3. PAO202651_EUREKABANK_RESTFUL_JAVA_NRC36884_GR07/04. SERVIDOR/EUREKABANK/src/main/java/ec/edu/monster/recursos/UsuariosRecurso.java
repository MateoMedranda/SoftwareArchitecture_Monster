package ec.edu.monster.recursos;

import ec.edu.monster.modelos.DTO.PerfilBancarioDTO;
import ec.edu.monster.modelos.DTO.UsuarioDTO;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import ec.edu.monster.servicios.UsuarioServicio;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import jakarta.ws.rs.*;
import jakarta.ws.rs.core.MediaType;
import java.util.List;

@Stateless @Path("usuarios") @Produces(MediaType.APPLICATION_JSON)
public class UsuariosRecurso {
    @EJB private UsuarioServicio usuarioServicio;
    private String token(String a) { return Tokens.extraer(a); }

    @POST @Consumes(MediaType.APPLICATION_JSON)
    public boolean crear(@HeaderParam("Authorization") String a, UsuarioDTO usuario) throws TransaccionExcepcion { return usuarioServicio.insertarUsuario(token(a), usuario); }
    @PUT @Path("{id}") @Consumes(MediaType.APPLICATION_JSON)
    public boolean actualizar(@HeaderParam("Authorization") String a, @PathParam("id") String id, UsuarioDTO usuario) throws TransaccionExcepcion { usuario.setUsuarioId(id); return usuarioServicio.actualizarUsuario(token(a), usuario); }
    @DELETE @Path("{id}")
    public boolean eliminar(@HeaderParam("Authorization") String a, @PathParam("id") String id) throws TransaccionExcepcion { return usuarioServicio.eliminarUsuario(token(a), id); }
    @GET @Path("{id}/perfil")
    public PerfilBancarioDTO perfil(@HeaderParam("Authorization") String a, @PathParam("id") String id) throws TransaccionExcepcion { return usuarioServicio.obtenerPerfilBancario(token(a), id); }
    @GET @Path("{id}")
    public UsuarioDTO obtener(@HeaderParam("Authorization") String a, @PathParam("id") String id) throws TransaccionExcepcion { return usuarioServicio.obtenerUsuario(token(a), id); }
    @GET
    public List<UsuarioDTO> listar(@HeaderParam("Authorization") String a, @QueryParam("nombre") String nombre) throws TransaccionExcepcion {
        return nombre == null || nombre.isBlank() ? usuarioServicio.listarUsuarios(token(a)) : usuarioServicio.buscarUsuarioPorNombre(token(a), nombre);
    }
}
