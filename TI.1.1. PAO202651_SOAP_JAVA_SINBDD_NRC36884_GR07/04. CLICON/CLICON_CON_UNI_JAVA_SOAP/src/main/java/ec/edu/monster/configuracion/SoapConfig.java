package ec.edu.monster.configuracion;

import io.github.cdimascio.dotenv.Dotenv;
import java.io.File;
import java.net.URISyntaxException;
import java.net.MalformedURLException;
import java.net.URL;

/** Lee la configuración del servicio sin dejar direcciones dentro del código. */
public final class SoapConfig {
    private static final String ARCHIVO_ENV = ".development.env";
    private static final Dotenv ENV = Dotenv.configure()
            .directory(ubicarDirectorio())
            .filename(ARCHIVO_ENV)
            .ignoreIfMissing()
            .load();

    private SoapConfig() { }

    private static String ubicarDirectorio() {
        if (new File(ARCHIVO_ENV).isFile()) return ".";
        try {
            var recurso = SoapConfig.class.getResource("/" + ARCHIVO_ENV);
            if (recurso != null && "file".equalsIgnoreCase(recurso.getProtocol())) {
                return new File(recurso.toURI()).getParent();
            }
        } catch (URISyntaxException ignored) {
            // Se probará el archivo de desarrollo a continuación.
        }
        File desarrollo = new File("src/main/recursos", ARCHIVO_ENV);
        return desarrollo.isFile() ? desarrollo.getParent() : ".";
    }

    private static String requerido(String clave) {
        String valor = ENV.get(clave);
        if (valor == null || valor.isBlank()) {
            throw new IllegalStateException("Falta la variable '" + clave + "' en " + ARCHIVO_ENV);
        }
        return valor;
    }

    public static String endpoint() { return requerido("SOAP_ENDPOINT"); }
    public static String targetNamespace() { return requerido("SOAP_TARGET_NAMESPACE"); }

    public static URL wsdl() {
        try {
            return new URL(requerido("SOAP_WSDL"));
        } catch (MalformedURLException ex) {
            throw new IllegalStateException("SOAP_WSDL no contiene una URL válida", ex);
        }
    }

    public static int connectTimeout() { return entero("SOAP_CONNECT_TIMEOUT"); }
    public static int readTimeout() { return entero("SOAP_READ_TIMEOUT"); }
    public static boolean insecureTls() { return Boolean.parseBoolean(ENV.get("SOAP_INSECURE_TLS", "false")); }

    private static int entero(String clave) {
        try {
            return Integer.parseInt(requerido(clave));
        } catch (NumberFormatException ex) {
            throw new IllegalStateException("La variable '" + clave + "' debe ser un entero", ex);
        }
    }
}
