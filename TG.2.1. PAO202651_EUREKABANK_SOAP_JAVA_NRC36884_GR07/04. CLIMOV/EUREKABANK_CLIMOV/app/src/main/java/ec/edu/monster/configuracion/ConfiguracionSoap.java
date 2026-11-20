package ec.edu.monster.configuracion;

import ec.edu.monster.BuildConfig;

/** Para emulador Android. En un teléfono físico reemplace 10.0.2.2 por la IP LAN del PC. */
public final class ConfiguracionSoap {
    private ConfiguracionSoap() {}
    private static String urlBase = BuildConfig.EUREKABANK_URL;

    public static synchronized String urlBase() {
        return urlBase;
    }

    public static synchronized void establecerUrl(String nuevaUrl) {
        if (nuevaUrl == null || nuevaUrl.isBlank()) {
            throw new IllegalArgumentException("La URL del servidor es obligatoria.");
        }
        if (!nuevaUrl.matches("https?://[^\\s/]+(?::\\d+)?(?:/.*)?")) {
            throw new IllegalArgumentException("Usa una URL válida, por ejemplo http://10.0.2.2:9080");
        }
        urlBase = nuevaUrl.replaceAll("/+$", "");
    }
}
