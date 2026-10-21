package ec.edu.monster.servicios;

import jakarta.enterprise.context.ApplicationScoped;
import jakarta.inject.Inject;
import jakarta.json.Json;

@ApplicationScoped
public class AutenticacionServicio {

    @Inject
    private ClienteRest clienteRest;

    public String login(String usuario, String contrasenia) {
        var solicitud = Json.createObjectBuilder()
                .add("usuario", usuario)
                .add("contrasena", contrasenia)
                .build();
        String token = clienteRest.post("/autenticacion/iniciar-sesion", solicitud, null)
                .getString("token", "");
        if (token.isBlank()) {
            throw new ApiRestException("El servidor no devolvió un token de sesión.");
        }
        return token;
    }
}
