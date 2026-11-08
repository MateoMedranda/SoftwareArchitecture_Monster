package ec.edu.monster.mapper;

import ec.edu.monster.exception.ExcepcionLongitudInvalida;
import ec.edu.monster.model.RespuestaError;
import jakarta.ws.rs.core.Response;
import jakarta.ws.rs.ext.ExceptionMapper;
import jakarta.ws.rs.ext.Provider;

@Provider
public class MapperExcepcionLongitudInvalida implements ExceptionMapper<ExcepcionLongitudInvalida> {

    @Override
    public Response toResponse(ExcepcionLongitudInvalida excepcion) {
        RespuestaError error = new RespuestaError(
            Response.Status.BAD_REQUEST.getStatusCode(),
            "Longitud Inválida",
            excepcion.getMessage()
        );
        return Response.status(Response.Status.BAD_REQUEST).entity(error).build();
    }
}