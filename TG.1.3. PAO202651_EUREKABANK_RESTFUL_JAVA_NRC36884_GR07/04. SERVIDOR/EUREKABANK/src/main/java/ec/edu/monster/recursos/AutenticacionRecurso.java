package ec.edu.monster.recursos;

import ec.edu.monster.modelos.Respuestas.LoginExcepcion;
import ec.edu.monster.modelos.Respuestas.RespuestaLogin;
import ec.edu.monster.servicios.LoginServicio;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import jakarta.ws.rs.Consumes;
import jakarta.ws.rs.GET;
import jakarta.ws.rs.POST;
import jakarta.ws.rs.Path;
import jakarta.ws.rs.Produces;
import jakarta.ws.rs.HeaderParam;
import jakarta.ws.rs.core.MediaType;

@Stateless
@Path("auth")
@Produces(MediaType.APPLICATION_JSON)
public class AutenticacionRecurso {
    @EJB private LoginServicio loginServicio;

    @POST @Path("login") @Consumes(MediaType.APPLICATION_JSON)
    public RespuestaLogin login(SolicitudLogin solicitud) throws LoginExcepcion {
        return loginServicio.login(solicitud == null ? null : solicitud.getUsuario(),
                solicitud == null ? null : solicitud.getContrasenia());
    }

    @POST @Path("logout")
    public boolean logout(@HeaderParam("Authorization") String authorization) throws LoginExcepcion {
        return loginServicio.logout(Tokens.extraer(authorization));
    }

    @GET @Path("verificar")
    public RespuestaLogin verificar(@HeaderParam("Authorization") String authorization) throws LoginExcepcion {
        return loginServicio.verificarToken(Tokens.extraer(authorization));
    }
}
