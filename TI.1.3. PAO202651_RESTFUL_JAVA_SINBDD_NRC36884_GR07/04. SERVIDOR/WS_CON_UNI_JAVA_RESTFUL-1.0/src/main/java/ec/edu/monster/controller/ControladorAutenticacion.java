package ec.edu.monster.controller;

import ec.edu.monster.model.RespuestaLogin;
import ec.edu.monster.model.SolicitudLogin;
import ec.edu.monster.service.ServicioAutenticacion;
import jakarta.inject.Inject;
import jakarta.validation.Valid;
import jakarta.ws.rs.Consumes;
import jakarta.ws.rs.POST;
import jakarta.ws.rs.Path;
import jakarta.ws.rs.Produces;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;

@Path("/autenticacion")
@Produces(MediaType.APPLICATION_JSON)
@Consumes(MediaType.APPLICATION_JSON)
public class ControladorAutenticacion {

    private final ServicioAutenticacion servicioAutenticacion;

    @Inject
    public ControladorAutenticacion(ServicioAutenticacion servicioAutenticacion) {
        this.servicioAutenticacion = servicioAutenticacion;
    }

    @POST
    @Path("/iniciar-sesion")
    public Response iniciarSesion(@Valid SolicitudLogin solicitud) {
        RespuestaLogin respuesta = servicioAutenticacion.iniciarSesion(solicitud);
        return Response.ok(respuesta).build();
    }
}