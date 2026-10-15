package ec.edu.monster.security;

import ec.edu.monster.model.RespuestaError;
import jakarta.inject.Inject;
import jakarta.ws.rs.container.ContainerRequestContext;
import jakarta.ws.rs.container.ContainerRequestFilter;
import jakarta.ws.rs.core.Response;
import jakarta.ws.rs.ext.Provider;

import java.io.IOException;

@Provider
public class FiltroAutenticacionJwt implements ContainerRequestFilter {

    private static final String ENCABEZADO_AUTH = "Authorization";
    private static final String PREFIJO_BEARER = "Bearer ";
    private static final String[] RUTAS_PROTEGIDAS = {"/conversion/"};

    @Inject
    ProveedorTokenJwt proveedorTokenJwt;

    @Override
    public void filter(ContainerRequestContext requestContext) throws IOException {
        String ruta = requestContext.getUriInfo().getPath();

        if (!esRutaProtegida(ruta)) {
            return;
        }

        String authHeader = requestContext.getHeaderString(ENCABEZADO_AUTH);

        if (authHeader == null || !authHeader.startsWith(PREFIJO_BEARER)) {
            abortarConError(requestContext, Response.Status.UNAUTHORIZED, "Encabezado Authorization faltante o inválido");
            return;
        }

        String token = authHeader.substring(PREFIJO_BEARER.length()).trim();

        if (!proveedorTokenJwt.validarToken(token)) {
            abortarConError(requestContext, Response.Status.UNAUTHORIZED, "Token inválido o expirado");
        }
    }

    private boolean esRutaProtegida(String ruta) {
        for (String rutaProtegida : RUTAS_PROTEGIDAS) {
            if (ruta.startsWith(rutaProtegida)) {
                return true;
            }
        }
        return false;
    }

    private void abortarConError(ContainerRequestContext requestContext, Response.Status estado, String mensaje) {
        RespuestaError error = new RespuestaError(estado.getStatusCode(), estado.getReasonPhrase(), mensaje);
        requestContext.abortWith(Response.status(estado).entity(error).build());
    }
}