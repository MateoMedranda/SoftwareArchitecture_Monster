package ec.edu.monster.configuracion;

import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Properties;

/**
 * Configura la URL del servidor REST del cliente de escritorio.
 */
public final class ConfiguracionRest {

    private static final Properties ENV = cargar();

    private ConfiguracionRest() {
    }

    public static String baseUrl() {
        String url = System.getenv("EUREKABANK_URL");
        if (url == null || url.isBlank()) {
            url = ENV.getProperty("EUREKABANK_URL", "http://localhost:9080/EUREKABANK-RESTFUL/api");
        }
        url = url.replaceAll("/+$", "");
        return url.endsWith("/api") ? url : url + "/EUREKABANK-RESTFUL/api";
    }

    private static Properties cargar() {
        Properties properties = new Properties();
        try (InputStream input = Files.exists(Path.of(".development.env")) ? Files.newInputStream(Path.of(".development.env")) : null) {
            if (input != null) {
                properties.load(input);
            }
        } catch (Exception ignored) {
        }
        try (InputStream input = ConfiguracionRest.class.getResourceAsStream("/.development.env")) {
            if (input != null) {
                properties.load(input);
            }
        } catch (Exception ignored) {
        }
        return properties;
    }
}
