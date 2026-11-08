package ec.edu.monster.mapper;

import ec.edu.monster.exception.ExcepcionTemperaturaInvalida;
import ec.edu.monster.model.RespuestaError;
import jakarta.ws.rs.core.Response;
import jakarta.ws.rs.ext.ExceptionMapper;
import jakarta.ws.rs.ext.Provider;

@Provider
public class MapperExcepcionTemperaturaInvalida implements ExceptionMapper<ExcepcionTemperaturaInvalida> {

    @Override
    public Response toResponse(ExcepcionTemperaturaInvalida excepcion) {
        RespuestaError error = new RespuestaError(
            Response.Status.BAD_REQUEST.getStatusCode(),
            "Temperatura Inválida",
            excepcion.getMessage()
        );
        return Response.status(Response.Status.BAD_REQUEST).entity(error).build();
    }
}