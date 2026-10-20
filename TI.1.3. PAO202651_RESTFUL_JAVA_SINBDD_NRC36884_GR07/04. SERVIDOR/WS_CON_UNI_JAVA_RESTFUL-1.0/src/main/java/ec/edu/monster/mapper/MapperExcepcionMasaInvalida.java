/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.mapper;


import ec.edu.monster.exception.ExcepcionMasaInvalida;
import ec.edu.monster.model.RespuestaError;
import jakarta.ws.rs.core.Response;
import jakarta.ws.rs.ext.ExceptionMapper;
import jakarta.ws.rs.ext.Provider;

/**
 *
 * @author marpa
 */
@Provider
public class MapperExcepcionMasaInvalida implements ExceptionMapper<ExcepcionMasaInvalida>{
    @Override
    public Response toResponse(ExcepcionMasaInvalida excepcion) {
        RespuestaError error = new RespuestaError(
            Response.Status.BAD_REQUEST.getStatusCode(),
            "Masa Inválida",
            excepcion.getMessage()
        );
        return Response.status(Response.Status.BAD_REQUEST).entity(error).build();
    }
}
