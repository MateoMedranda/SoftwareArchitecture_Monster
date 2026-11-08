package ec.edu.monster.controller;

import ec.edu.monster.model.RespuestaLogin;
import ec.edu.monster.model.RespuestaError;
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
import org.eclipse.microprofile.openapi.annotations.Operation;
import org.eclipse.microprofile.openapi.annotations.parameters.RequestBody;
import org.eclipse.microprofile.openapi.annotations.responses.APIResponse;
import org.eclipse.microprofile.openapi.annotations.responses.APIResponses;
import org.eclipse.microprofile.openapi.annotations.media.Content;
import org.eclipse.microprofile.openapi.annotations.media.Schema;

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
    @Operation(
        summary = "Inicio de sesión",
        description = "Recibe credenciales y autentica para devolver token de sesión."
    )
    @APIResponses({
        @APIResponse(
            responseCode = "200",
            description = "Inicio de Sesión realizado exitosamente",
            content = @Content(mediaType = MediaType.APPLICATION_JSON,
                    schema = @Schema(implementation = RespuestaLogin.class)
                    )
        ),
        @APIResponse(
            responseCode = "401",
            description = "Credenciales inválidas",
            content = @Content(mediaType = MediaType.APPLICATION_JSON,
                    schema = @Schema(implementation = RespuestaError.class)
            )
        )
    })
    public Response iniciarSesion(@RequestBody(
            description = "Credenciales de inicio de sesión", required = true)
     @Valid SolicitudLogin solicitud) {
        RespuestaLogin respuesta = servicioAutenticacion.iniciarSesion(solicitud);
        return Response.ok(respuesta).build();
    }
}