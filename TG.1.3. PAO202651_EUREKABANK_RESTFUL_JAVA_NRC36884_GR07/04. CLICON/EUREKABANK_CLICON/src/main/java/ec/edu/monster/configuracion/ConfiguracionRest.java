package ec.edu.monster.configuracion;

import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Properties;

/**
 * Obtiene la URL base del servidor REST; admite EUREKABANK_URL.
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
        Properties p = new Properties();
        try (InputStream in = Files.exists(Path.of(".development.env")) ? Files.newInputStream(Path.of(".development.env")) : null) {
            if (in != null) {
                p.load(in);
            }
        } catch (Exception ignored) {
        }
        try (InputStream in = ConfiguracionRest.class.getResourceAsStream("/.development.env")) {
            if (in != null) {
                p.load(in);
            }
        } catch (Exception ignored) {
        }
        return p;
    }
}
