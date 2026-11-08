/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.controller;

import ec.edu.monster.model.RespuestaConversion;
import ec.edu.monster.model.RespuestaError;
import ec.edu.monster.model.SolicitudConversion;
import ec.edu.monster.service.ServicioMasa;
import jakarta.inject.Inject;
import jakarta.validation.Valid;
import jakarta.ws.rs.Consumes;
import jakarta.ws.rs.POST;
import jakarta.ws.rs.Path;
import jakarta.ws.rs.Produces;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;
import org.eclipse.microprofile.openapi.annotations.Operation;
import org.eclipse.microprofile.openapi.annotations.media.Content;
import org.eclipse.microprofile.openapi.annotations.media.Schema;
import org.eclipse.microprofile.openapi.annotations.parameters.RequestBody;
import org.eclipse.microprofile.openapi.annotations.responses.APIResponse;
import org.eclipse.microprofile.openapi.annotations.responses.APIResponses;
import org.eclipse.microprofile.openapi.annotations.security.SecurityRequirement;

/**
 *
 * @author marpa
 */

@Path("/conversion/masa")
@Produces(MediaType.APPLICATION_JSON)
@Consumes(MediaType.APPLICATION_JSON)
public class ControladorMasa {
    private final ServicioMasa servicioMasa;

    @Inject
    public ControladorMasa(ServicioMasa servicioMasa) {
        this.servicioMasa = servicioMasa;
    }

    @POST
    @SecurityRequirement(name = "bearerAuth")
    @Operation(
        operationId = "convertirMasa",
        summary = "Convertir unidades de masa",
        description = "Convierte un valor de una unidad de masa a otra."
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
            description = "Datos de entrada o unidad de masa inválidos",
            content = @Content(mediaType = MediaType.APPLICATION_JSON,
                    schema = @Schema(implementation = RespuestaError.class)
            )
        )
    })
    public Response convertir(@RequestBody(
            description = "Datos para la conversión de masa",
            required = true,
            content = @Content(mediaType = MediaType.APPLICATION_JSON,
                    schema = @Schema(implementation = SolicitudConversion.class)
            )
    ) @Valid SolicitudConversion solicitud) {
        RespuestaConversion respuesta = servicioMasa.convertir(solicitud);
        return Response.ok(respuesta).build();
    }
}
