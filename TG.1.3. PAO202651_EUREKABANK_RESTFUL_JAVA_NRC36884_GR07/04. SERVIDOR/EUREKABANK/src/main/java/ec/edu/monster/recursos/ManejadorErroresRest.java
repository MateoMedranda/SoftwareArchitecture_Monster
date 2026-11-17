package ec.edu.monster.recursos;

import ec.edu.monster.modelos.Respuestas.LoginExcepcion;
import ec.edu.monster.modelos.Respuestas.RespuestaError;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;
import jakarta.ws.rs.ext.ExceptionMapper;
import jakarta.ws.rs.ext.Provider;

/**
 * Convierte los errores de negocio existentes en respuestas JSON y códigos
 * HTTP.
 */
@Provider
public class ManejadorErroresRest implements ExceptionMapper<Exception> {

    @Override
    public Response toResponse(Exception exception) {
        RespuestaError error = extraer(exception);
        int estado = estadoPara(error == null ? null : error.getCodigo());
        if (error == null) {
            error = new RespuestaError("ERROR_INTERNO", "Ocurrió un error inesperado.", null);
        }
        return Response.status(estado).type(MediaType.APPLICATION_JSON).entity(error).build();
    }

    private RespuestaError extraer(Exception e) {
        if (e instanceof LoginExcepcion) {
            return ((LoginExcepcion) e).getFaultInfo();
        }
        if (e instanceof TransaccionExcepcion) {
            return ((TransaccionExcepcion) e).getFaultInfo();
        }
        return null;
    }

    private int estadoPara(String codigo) {
        if (codigo == null) {
            return 500;
        }
        if (codigo.startsWith("TOKEN_")) {
            return 401;
        }
        if ("ACCESO_DENEGADO".equals(codigo)) {
            return 403;
        }
        if (codigo.endsWith("NO_ENCONTRADA") || codigo.endsWith("NO_ENCONTRADO")) {
            return 404;
        }
        if (codigo.startsWith("CAMPO_") || codigo.contains("INVALID")) {
            return 400;
        }
        if (codigo.contains("INMUTABLE") || codigo.contains("YA_") || codigo.contains("NO_ADMITIDA")) {
            return 409;
        }
        return 500;
    }
}
