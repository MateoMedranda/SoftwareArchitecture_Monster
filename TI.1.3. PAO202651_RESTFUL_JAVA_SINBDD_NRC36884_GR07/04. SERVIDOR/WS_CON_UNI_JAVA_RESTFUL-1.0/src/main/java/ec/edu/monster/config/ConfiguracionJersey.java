package ec.edu.monster.config;

import ec.edu.monster.mapper.MapperExcepcionAutenticacion;
import ec.edu.monster.mapper.MapperExcepcionLongitudInvalida;
import ec.edu.monster.mapper.MapperExcepcionTemperaturaInvalida;
import ec.edu.monster.security.FiltroAutenticacionJwt;
import org.glassfish.jersey.server.ResourceConfig;

import org.eclipse.microprofile.openapi.annotations.OpenAPIDefinition;
import org.eclipse.microprofile.openapi.annotations.info.Contact;
import org.eclipse.microprofile.openapi.annotations.info.Info;
import org.eclipse.microprofile.openapi.annotations.servers.Server;
import jakarta.ws.rs.core.Application;

import jakarta.ws.rs.ApplicationPath;
import org.eclipse.microprofile.openapi.annotations.enums.SecuritySchemeType;
import org.eclipse.microprofile.openapi.annotations.security.SecurityScheme;

@ApplicationPath("/api")
@OpenAPIDefinition(
        info = @Info(
        title = "Convertidor de Unidades Monster RESTful JAVA",
        version = "1.0.0",
        description = "Servicio web RESTful en Java para conversión de unidades de temperatura", 
        contact = @Contact(name= "Grupo 7 Arquitectura")))
@SecurityScheme(
    securitySchemeName  = "bearerAuth",
    type = SecuritySchemeType.HTTP,
    scheme = "bearer",
    bearerFormat = "JWT")

public class ConfiguracionJersey extends Application {

    public ConfiguracionJersey() {
    }
}