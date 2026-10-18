package ec.edu.monster.mapper;

import ec.edu.monster.exception.ExcepcionAutenticacion;
import ec.edu.monster.model.RespuestaError;
import jakarta.ws.rs.core.Response;
import jakarta.ws.rs.ext.ExceptionMapper;
import jakarta.ws.rs.ext.Provider;

@Provider
public class MapperExcepcionAutenticacion implements ExceptionMapper<ExcepcionAutenticacion> {

    @Override
    public Response toResponse(ExcepcionAutenticacion excepcion) {
        RespuestaError error = new RespuestaError(
            Response.Status.UNAUTHORIZED.getStatusCode(),
            "Error de Autenticación",
            excepcion.getMessage()
        );
        return Response.status(Response.Status.UNAUTHORIZED).entity(error).build();
    }
}