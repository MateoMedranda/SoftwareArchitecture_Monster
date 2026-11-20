package ec.edu.monster.configuracion;

import java.io.InputStream;
import java.util.Properties;

/** Configuración del gateway SOAP del WAR. Variable de entorno tiene prioridad. */
public final class ConfiguracionSoap {
    private static final Properties ENV = cargar();
    private ConfiguracionSoap() { }
    public static String baseUrl() {
        String url = System.getenv("EUREKABANK_URL");
        if (url == null || url.isBlank()) url = ENV.getProperty("EUREKABANK_URL", "http://localhost:8080");
        return url.replaceAll("/+$", "");
    }
    private static Properties cargar() {
        Properties p = new Properties();
        try (InputStream in = ConfiguracionSoap.class.getResourceAsStream("/.development.env")) {
            if (in != null) p.load(in);
        } catch (Exception ignored) { }
        return p;
    }
}
