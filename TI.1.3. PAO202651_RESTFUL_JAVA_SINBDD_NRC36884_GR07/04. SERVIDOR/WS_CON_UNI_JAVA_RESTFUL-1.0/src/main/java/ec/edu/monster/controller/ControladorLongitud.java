package ec.edu.monster.controller;

import ec.edu.monster.model.RespuestaConversion;
import ec.edu.monster.model.RespuestaLogin;
import ec.edu.monster.model.SolicitudConversion;
import ec.edu.monster.service.ServicioLongitud;
import jakarta.inject.Inject;
import jakarta.validation.Valid;
import jakarta.ws.rs.Consumes;
import jakarta.ws.rs.POST;
import jakarta.ws.rs.Path;
import jakarta.ws.rs.Produces;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;
import org.eclipse.microprofile.openapi.annotations.Operation;
import org.eclipse.microprofile.openapi.annotations.enums.SecuritySchemeType;
import org.eclipse.microprofile.openapi.annotations.parameters.RequestBody;
import org.eclipse.microprofile.openapi.annotations.responses.APIResponse;
import org.eclipse.microprofile.openapi.annotations.responses.APIResponses;
import org.eclipse.microprofile.openapi.annotations.media.Content;
import org.eclipse.microprofile.openapi.annotations.media.Schema;
import org.eclipse.microprofile.openapi.annotations.security.SecurityRequirement;
import org.eclipse.microprofile.openapi.annotations.security.SecurityScheme;

@Path("/conversion/longitud")
@Produces(MediaType.APPLICATION_JSON)
@Consumes(MediaType.APPLICATION_JSON)
public class ControladorLongitud {

    private final ServicioLongitud servicioLongitud;

    @Inject
    public ControladorLongitud(ServicioLongitud servicioLongitud) {
        this.servicioLongitud = servicioLongitud;
    }

    @POST
    @SecurityRequirement(name = "bearerAuth")
    @Operation(
        summary = "Convertir unidades de longitud",
        description = "Convierte un valor en una unidad de longitud de salida, y devuelve su valor en una unidad de destino."
    )
    @APIResponses({
        @APIResponse(
            responseCode = "200",
            description = "Conversión Exitosa"
        ),
        @APIResponse(
            responseCode = "403",
            description = "Credenciales Inválidas"
        ),
        @APIResponse(
            responseCode = "400",
            description = "Datos de entrada Inválidos"
        )
    })
    public Response convertir(@RequestBody(
            description = "Datos para la conversión", required = true) @Valid SolicitudConversion solicitud) {
        RespuestaConversion respuesta = servicioLongitud.convertir(solicitud);
        return Response.ok(respuesta).build();
    }
}