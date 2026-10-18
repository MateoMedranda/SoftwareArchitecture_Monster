package ec.edu.monster.controller;

import ec.edu.monster.model.RespuestaConversion;
import ec.edu.monster.model.SolicitudConversion;
import ec.edu.monster.service.ServicioTemperatura;
import jakarta.inject.Inject;
import jakarta.validation.Valid;
import jakarta.ws.rs.Consumes;
import jakarta.ws.rs.POST;
import jakarta.ws.rs.Path;
import jakarta.ws.rs.Produces;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;

@Path("/conversion/temperatura")
@Produces(MediaType.APPLICATION_JSON)
@Consumes(MediaType.APPLICATION_JSON)
public class ControladorTemperatura {

    private final ServicioTemperatura servicioTemperatura;

    @Inject
    public ControladorTemperatura(ServicioTemperatura servicioTemperatura) {
        this.servicioTemperatura = servicioTemperatura;
    }

    @POST
    public Response convertir(@Valid SolicitudConversion solicitud) {
        RespuestaConversion respuesta = servicioTemperatura.convertir(solicitud);
        return Response.ok(respuesta).build();
    }
}