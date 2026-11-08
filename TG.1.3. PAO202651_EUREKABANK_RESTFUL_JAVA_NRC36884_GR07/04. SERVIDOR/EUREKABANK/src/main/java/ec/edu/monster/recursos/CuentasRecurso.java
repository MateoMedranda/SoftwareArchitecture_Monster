package ec.edu.monster.recursos;

import ec.edu.monster.modelos.DTO.ClienteCuentaDTO;
import ec.edu.monster.modelos.DTO.CuentaDTO;
import ec.edu.monster.modelos.DTO.EstadoCuentaDTO;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import ec.edu.monster.servicios.CuentaServicio;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import jakarta.ws.rs.*;
import jakarta.ws.rs.core.MediaType;
import java.util.List;

@Stateless
@Path("cuentas")
@Produces(MediaType.APPLICATION_JSON)
public class CuentasRecurso {

    @EJB
    private CuentaServicio cuentaServicio;

    private String token(String a) {
        return Tokens.extraer(a);
    }

    @POST
    @Consumes(MediaType.APPLICATION_JSON)
    public boolean crear(@HeaderParam("Authorization") String a, CuentaDTO cuenta) throws TransaccionExcepcion {
        return cuentaServicio.insertarCuenta(token(a), cuenta);
    }

    @POST
    @Path("para-usuario/{usuarioId}")
    public CuentaDTO crearParaUsuario(@HeaderParam("Authorization") String a, @PathParam("usuarioId") String usuarioId) throws TransaccionExcepcion {
        return cuentaServicio.crearCuentaParaUsuario(token(a), usuarioId);
    }

    @GET
    @Path("clientes")
    public List<ClienteCuentaDTO> clientes(@HeaderParam("Authorization") String a) throws TransaccionExcepcion {
        return cuentaServicio.listarClientesParaCuenta(token(a));
    }

    @PUT
    @Path("{id}")
    @Consumes(MediaType.APPLICATION_JSON)
    public boolean actualizar(@HeaderParam("Authorization") String a, @PathParam("id") String id, CuentaDTO cuenta) throws TransaccionExcepcion {
        cuenta.setCuentaId(id);
        return cuentaServicio.actualizarCuenta(token(a), cuenta);
    }

    @DELETE
    @Path("{id}")
    public boolean eliminar(@HeaderParam("Authorization") String a, @PathParam("id") String id) throws TransaccionExcepcion {
        return cuentaServicio.eliminarCuenta(token(a), id);
    }

    @GET
    @Path("disponibilidades")
    public List<EstadoCuentaDTO> disponibilidades(@HeaderParam("Authorization") String a) throws TransaccionExcepcion {
        return cuentaServicio.listarDisponibilidades(token(a));
    }

    @GET
    @Path("{id}/disponibilidad")
    public EstadoCuentaDTO disponibilidad(@HeaderParam("Authorization") String a, @PathParam("id") String id) throws TransaccionExcepcion {
        return cuentaServicio.obtenerDisponibilidad(token(a), id);
    }

    @GET
    @Path("{id}/disponibilidad/esperar")
    public EstadoCuentaDTO esperar(@HeaderParam("Authorization") String a, @PathParam("id") String id, @DefaultValue("0") @QueryParam("versionConocida") long version, @DefaultValue("30000") @QueryParam("esperaMaximaMs") long espera) throws TransaccionExcepcion {
        return cuentaServicio.esperarCambioDisponibilidad(token(a), id, version, espera);
    }

    @POST
    @Path("{id}/disponibilidad/recuperar")
    public EstadoCuentaDTO recuperar(@HeaderParam("Authorization") String a, @PathParam("id") String id) throws TransaccionExcepcion {
        return cuentaServicio.recuperarDisponibilidad(token(a), id);
    }

    @GET
    @Path("{id}")
    public CuentaDTO obtener(@HeaderParam("Authorization") String a, @PathParam("id") String id) throws TransaccionExcepcion {
        return cuentaServicio.obtenerCuenta(token(a), id);
    }

    @GET
    public List<CuentaDTO> listar(@HeaderParam("Authorization") String a, @QueryParam("usuarioId") String usuarioId) throws TransaccionExcepcion {
        return usuarioId == null || usuarioId.isBlank() ? cuentaServicio.listarCuentas(token(a)) : cuentaServicio.listarCuentasPorUsuario(token(a), usuarioId);
    }
}
