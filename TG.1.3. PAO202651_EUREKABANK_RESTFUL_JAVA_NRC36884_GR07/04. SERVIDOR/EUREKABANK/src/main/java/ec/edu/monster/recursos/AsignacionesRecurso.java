package ec.edu.monster.recursos;

import ec.edu.monster.modelos.DTO.CuentaDTO;
import ec.edu.monster.modelos.DTO.PerteneceDTO;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import ec.edu.monster.servicios.PerteneceServicio;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import jakarta.ws.rs.*;
import jakarta.ws.rs.core.MediaType;
import java.util.List;

@Stateless
@Path("asignaciones")
@Produces(MediaType.APPLICATION_JSON)
public class AsignacionesRecurso {

    @EJB
    private PerteneceServicio perteneceServicio;

    private String token(String a) {
        return Tokens.extraer(a);
    }

    @POST
    @Consumes(MediaType.APPLICATION_JSON)
    public boolean asignar(@HeaderParam("Authorization") String a, SolicitudAsignacion s) throws TransaccionExcepcion {
        return perteneceServicio.asignarCuentaUsuario(token(a), s == null ? null : s.getCuentaId(), s == null ? null : s.getUsuarioId());
    }

    @DELETE
    public boolean desasignar(@HeaderParam("Authorization") String a, @QueryParam("cuentaId") String cuentaId, @QueryParam("usuarioId") String usuarioId) throws TransaccionExcepcion {
        return perteneceServicio.desasignarCuentaUsuario(token(a), cuentaId, usuarioId);
    }

    @GET
    @Path("cuentas")
    public List<CuentaDTO> buscarCuentas(@HeaderParam("Authorization") String a, @QueryParam("nombreOUsuario") String busqueda) throws TransaccionExcepcion {
        return perteneceServicio.buscarCuentaPorUsuario(token(a), busqueda);
    }

    @GET
    public List<PerteneceDTO> listar(@HeaderParam("Authorization") String a, @QueryParam("cuentaId") String cuentaId, @QueryParam("usuarioId") String usuarioId) throws TransaccionExcepcion {
        if (cuentaId != null && !cuentaId.isBlank()) {
            return perteneceServicio.listarPorCuenta(token(a), cuentaId);
        }
        if (usuarioId != null && !usuarioId.isBlank()) {
            return perteneceServicio.listarPorUsuario(token(a), usuarioId);
        }
        throw new TransaccionExcepcion("Parámetro requerido", new ec.edu.monster.modelos.Respuestas.RespuestaError("CAMPO_REQUERIDO", "Indique cuentaId o usuarioId.", null));
    }
}
