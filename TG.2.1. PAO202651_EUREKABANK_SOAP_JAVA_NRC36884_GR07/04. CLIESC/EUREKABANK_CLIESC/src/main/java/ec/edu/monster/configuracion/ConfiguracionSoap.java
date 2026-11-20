package ec.edu.monster.configuracion;

import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Properties;

/** Carga la URL desde .development.env y permite sobrescribirla por entorno. */
public final class ConfiguracionSoap {
    private static final Properties ENV = cargar();
    private ConfiguracionSoap() { }
    public static String baseUrl() {
        String url = System.getenv("EUREKABANK_URL");
        if (url == null || url.isBlank()) url = ENV.getProperty("EUREKABANK_URL", "http://localhost:9080");
        return url.replaceAll("/+$", "");
    }
    private static Properties cargar() {
        Properties p = new Properties();
        try (InputStream in = Files.exists(Path.of(".development.env"))
                ? Files.newInputStream(Path.of(".development.env")) : null) {
            if (in != null) p.load(in);
        } catch (Exception ignored) { }
        try (InputStream in = ConfiguracionSoap.class.getResourceAsStream("/.development.env")) {
            if (in != null) p.load(in);
        } catch (Exception ignored) { }
        return p;
    }
}
