package ec.edu.monster.controller;

import ec.edu.monster.model.RespuestaConversion;
import ec.edu.monster.model.RespuestaError;
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
import org.eclipse.microprofile.openapi.annotations.Operation;
import org.eclipse.microprofile.openapi.annotations.parameters.RequestBody;
import org.eclipse.microprofile.openapi.annotations.responses.APIResponse;
import org.eclipse.microprofile.openapi.annotations.responses.APIResponses;
import org.eclipse.microprofile.openapi.annotations.media.Content;
import org.eclipse.microprofile.openapi.annotations.media.Schema;
import org.eclipse.microprofile.openapi.annotations.security.SecurityRequirement;

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
    @SecurityRequirement(name = "bearerAuth")
    @Operation(
        operationId = "convertirTemperatura",
        summary = "Convertir unidades de temperatura",
        description = "Convierte un valor de una unidad de temperatura a otra."
    )
    @APIResponses({
        @APIResponse(
            responseCode = "200",
            description = "Conversión realizada exitosamente",
            content = @Content(mediaType = MediaType.APPLICATION_JSON,
                    schema = @Schema(implementation = RespuestaConversion.class)
            )
        ),
        @APIResponse(
            responseCode = "401",
            description = "Token de autenticación ausente o inválido",
            content = @Content(mediaType = MediaType.APPLICATION_JSON,
                    schema = @Schema(implementation = RespuestaError.class)
            )
        ),
        @APIResponse(
            responseCode = "400",
            description = "Datos de entrada o unidad de temperatura inválidos",
            content = @Content(mediaType = MediaType.APPLICATION_JSON,
                    schema = @Schema(implementation = RespuestaError.class)
            )
        )
    })
    public Response convertir(@RequestBody(
            description = "Datos para la conversión de temperatura",
            required = true,
            content = @Content(mediaType = MediaType.APPLICATION_JSON,
                    schema = @Schema(implementation = SolicitudConversion.class)
            )
    ) @Valid SolicitudConversion solicitud) {
        RespuestaConversion respuesta = servicioTemperatura.convertir(solicitud);
        return Response.ok(respuesta).build();
    }
}