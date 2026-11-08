package ec.edu.monster.configuracion;

import ec.edu.monster.BuildConfig;

/**
 * URL ajustable del servidor REST en Android.
 */
public final class ConfiguracionRest {

    private static String url = normalizar(BuildConfig.EUREKABANK_URL);

    private ConfiguracionRest() {
    }

    /**
     * Nombre uniforme usado por todos los clientes REST.
     */
    public static String baseUrl() {
        return url;
    }

    /**
     * Conserva compatibilidad con la pantalla de configuración existente.
     */
    public static String urlBase() {
        return baseUrl();
    }

    public static void establecerUrl(String valor) {
        if (valor == null || valor.isBlank()) {
            throw new IllegalArgumentException("La URL no puede estar vacía.");
        }
        url = normalizar(valor);
    }

    private static String normalizar(String valor) {
        valor = valor.replaceAll("/+$", "");
        return valor.endsWith("/api") ? valor : valor + "/EUREKABANK-RESTFUL/api";
    }
}
