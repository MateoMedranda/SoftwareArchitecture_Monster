package ec.edu.monster.configuracion;

import io.github.cdimascio.dotenv.Dotenv;
import java.io.File;
import java.net.URISyntaxException;

/** Lee la configuración del servicio RESTful sin dejar direcciones dentro del código. */
public final class RestConfig {
    private static final String ARCHIVO_ENV = "development.env";
    private static final String[] RUTAS_ENV = {
        "src/main/java/ec/edu/monster/clicon_uni_java_restful/" + ARCHIVO_ENV,
        "src/main/recursos/" + ARCHIVO_ENV
    };
    private static final Dotenv ENV = cargar();

    private RestConfig() { }

    private static Dotenv cargar() {
        File archivo = ubicarArchivo();
        if (archivo != null) {
            return Dotenv.configure()
                    .directory(archivo.getParent())
                    .filename(ARCHIVO_ENV)
                    .ignoreIfMissing()
                    .load();
        }
        return Dotenv.configure()
                .filename(ARCHIVO_ENV)
                .ignoreIfMissing()
                .load();
    }

    private static File ubicarArchivo() {
        File local = new File(ARCHIVO_ENV);
        if (local.isFile()) return local;
        for (String recurso : new String[] {"/" + ARCHIVO_ENV,
                "/ec/edu/monster/clicon_uni_java_restful/" + ARCHIVO_ENV}) {
            File archivo = desdeRecurso(recurso);
            if (archivo != null) return archivo;
        }
        for (String ruta : RUTAS_ENV) {
            File archivo = new File(ruta);
            if (archivo.isFile()) return archivo;
        }
        return null;
    }

    private static File desdeRecurso(String ruta) {
        try {
            var recurso = RestConfig.class.getResource(ruta);
            if (recurso != null && "file".equalsIgnoreCase(recurso.getProtocol())) {
                return new File(recurso.toURI());
            }
        } catch (URISyntaxException ignored) {
            // Se intentará con las rutas del proyecto a continuación.
        }
        return null;
    }

    private static String requerido(String clave) {
        String valor = ENV.get(clave);
        if (valor == null || valor.isBlank()) {
            throw new IllegalStateException("Falta la variable '" + clave + "' en " + ARCHIVO_ENV);
        }
        return valor;
    }

    public static String baseUrl() {
        return requerido("RESTFUL_PROTOCOL") + "://"
                + requerido("RESTFUL_HOST") + ":"
                + requerido("RESTFUL_PORT")
                + requerido("RESTFUL_API");
    }

    public static String endpointAutenticacion() { return requerido("ENDPOINT_AUTENTICACION"); }
    public static String endpointConvertirLongitud() { return requerido("ENDPOINT_CONVERSION_LONGITUD"); }
    public static String endpointConvertirTemperatura() { return requerido("ENDPOINT_CONVERSION_TEMPERATURA"); }
    public static String endpointConvertirMasa() { return requerido("ENDPOINT_CONVERSION_MASA"); }

    public static int connectTimeout() { return entero("RESTFUL_CONNECT_TIMEOUT"); }
    public static int readTimeout() { return entero("RESTFUL_READ_TIMEOUT"); }
    public static boolean insecureTls() { return Boolean.parseBoolean(ENV.get("RESTFUL_INSECURE_TLS", "false")); }

    private static int entero(String clave) {
        try {
            return Integer.parseInt(requerido(clave));
        } catch (NumberFormatException ex) {
            throw new IllegalStateException("La variable '" + clave + "' debe ser un entero", ex);
        }
    }
}
