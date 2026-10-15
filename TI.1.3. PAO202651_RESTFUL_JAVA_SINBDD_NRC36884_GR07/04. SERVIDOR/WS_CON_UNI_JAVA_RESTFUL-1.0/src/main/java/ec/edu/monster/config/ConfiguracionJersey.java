package ec.edu.monster.config;

import ec.edu.monster.mapper.MapperExcepcionAutenticacion;
import ec.edu.monster.mapper.MapperExcepcionTemperaturaInvalida;
import ec.edu.monster.security.FiltroAutenticacionJwt;
import org.glassfish.jersey.server.ResourceConfig;

import jakarta.ws.rs.ApplicationPath;

@ApplicationPath("/")
public class ConfiguracionJersey extends ResourceConfig {

    public ConfiguracionJersey() {
        packages("ec.edu.monster.controller");
        register(FiltroAutenticacionJwt.class);
        register(MapperExcepcionAutenticacion.class);
        register(MapperExcepcionTemperaturaInvalida.class);
    }
}