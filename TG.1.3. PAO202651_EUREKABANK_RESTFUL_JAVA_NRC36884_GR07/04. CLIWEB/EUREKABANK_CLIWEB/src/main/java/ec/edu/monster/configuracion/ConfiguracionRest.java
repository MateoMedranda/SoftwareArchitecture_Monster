package ec.edu.monster.configuracion;

public final class ConfiguracionRest {

    private static final String BASE_LOCAL_DOCKER = "http://127.0.0.1:8080/EUREKABANK-RESTFUL/api";

    private ConfiguracionRest() {
    }

    public static String baseUrl() {
        // Permite sustituir el servidor local por uno remoto si se requiere.
        String configurada = System.getenv("EUREKABANK_URL");
        if (configurada != null && !configurada.isBlank()) {
            return normalizar(configurada);
        }
        return BASE_LOCAL_DOCKER;
    }

    private static String normalizar(String url) {
        url = url.replaceAll("/+$", "");
        return url.endsWith("/api") ? url : url + "/EUREKABANK-RESTFUL/api";
    }
}
