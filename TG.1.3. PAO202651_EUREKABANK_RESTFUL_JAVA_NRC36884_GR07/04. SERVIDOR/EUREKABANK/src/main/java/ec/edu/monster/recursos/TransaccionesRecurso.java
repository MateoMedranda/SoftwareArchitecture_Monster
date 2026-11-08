package ec.edu.monster.recursos;

import ec.edu.monster.modelos.DTO.TransaccionDTO;
import ec.edu.monster.modelos.DTO.TransferenciaDTO;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import ec.edu.monster.servicios.TransaccionServicio;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import jakarta.ws.rs.*;
import jakarta.ws.rs.core.MediaType;
import java.util.List;

@Stateless
@Path("transacciones")
@Produces(MediaType.APPLICATION_JSON)
public class TransaccionesRecurso {

    @EJB
    private TransaccionServicio transaccionServicio;

    private String token(String a) {
        return Tokens.extraer(a);
    }

    @POST
    @Path("movimientos")
    @Consumes(MediaType.APPLICATION_JSON)
    public TransaccionDTO movimiento(@HeaderParam("Authorization") String a, SolicitudMovimiento s) throws TransaccionExcepcion {
        return transaccionServicio.generarTransaccion(token(a), s == null ? null : s.getTipo(), s == null ? null : s.getCuentaId(), s == null ? null : s.getMonto());
    }

    @POST
    @Path("transferencias")
    @Consumes(MediaType.APPLICATION_JSON)
    public TransferenciaDTO transferencia(@HeaderParam("Authorization") String a, SolicitudTransferencia s) throws TransaccionExcepcion {
        return transaccionServicio.realizarTransferencia(token(a), s == null ? null : s.getCuentaOrigenId(), s == null ? null : s.getCuentaDestinoId(), s == null ? null : s.getMonto());
    }

    @POST
    @Path("{id}/reversa")
    public TransaccionDTO reversar(@HeaderParam("Authorization") String a, @PathParam("id") String id) throws TransaccionExcepcion {
        return transaccionServicio.reversarTransaccion(token(a), id);
    }

    @DELETE
    @Path("{id}")
    public boolean eliminar(@HeaderParam("Authorization") String a, @PathParam("id") String id) throws TransaccionExcepcion {
        return transaccionServicio.eliminarTransaccion(token(a), id);
    }

    @GET
    @Path("{id}")
    public TransaccionDTO obtener(@HeaderParam("Authorization") String a, @PathParam("id") String id) throws TransaccionExcepcion {
        return transaccionServicio.obtenerTransaccion(token(a), id);
    }

    @GET
    public List<TransaccionDTO> listar(@HeaderParam("Authorization") String a, @QueryParam("cuentaId") String cuentaId, @QueryParam("usuarioId") String usuarioId) throws TransaccionExcepcion {
        if (cuentaId != null && !cuentaId.isBlank()) {
            return transaccionServicio.listarPorCuenta(token(a), cuentaId);
        }
        if (usuarioId != null && !usuarioId.isBlank()) {
            return transaccionServicio.listarPorUsuario(token(a), usuarioId);
        }
        return transaccionServicio.listarTransacciones(token(a));
    }
}
