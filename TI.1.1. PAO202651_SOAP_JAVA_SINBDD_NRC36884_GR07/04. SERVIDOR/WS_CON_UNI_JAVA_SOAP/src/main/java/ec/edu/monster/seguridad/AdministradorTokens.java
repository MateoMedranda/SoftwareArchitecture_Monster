package ec.edu.monster.seguridad;

import jakarta.ejb.Singleton;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;
import java.util.UUID;
import ec.edu.monster.seguridad.Sesion;
import java.time.Instant;

@Singleton
public class AdministradorTokens {
    
    private final Map<String, Sesion> registroSesiones = new ConcurrentHashMap<>();
    private final static int TTL_MINUTOS = 15;

    public String generarToken() {
        String token = UUID.randomUUID().toString();
        Sesion sesionActiva = new Sesion(token, TTL_MINUTOS);
        registroSesiones.put(token, sesionActiva);
        return token;
    }

    public boolean validarToken(String token) {
        Sesion sesionRecuperada = registroSesiones.get(token);
        if(sesionRecuperada==null)
        {
            return false;
        }

        boolean sesionCaducada = Instant.now().isAfter(sesionRecuperada.getExpiracion());
        if(sesionCaducada)
        {
            registroSesiones.remove(token);
        }
        return !sesionCaducada;
    }
}