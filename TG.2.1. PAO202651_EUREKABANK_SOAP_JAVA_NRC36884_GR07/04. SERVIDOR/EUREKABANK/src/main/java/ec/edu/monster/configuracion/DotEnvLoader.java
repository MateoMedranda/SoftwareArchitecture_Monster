package ec.edu.monster.configuracion;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.util.Collections;
import java.util.HashMap;
import java.util.Map;
import java.util.logging.Logger;

/**
 * Cargador de variables de configuración desde el archivo .development.env.
 *
 * Orden de búsqueda del archivo:
 *   1. Propiedad del sistema: -DENV_FILE=/ruta/al/.development.env  (Payara JVM options)
 *   2. Variable de entorno:    ENV_FILE=/ruta/al/.development.env
 *   3. Directorio de trabajo del servidor: user.dir/.development.env
 *   4. Directorio padre del directorio de trabajo
 *
 * Si no se encuentra el archivo, se usan las variables del sistema operativo
 * o los valores por defecto definidos en cada llamada a obtener().
 *
 * @author MONSTER GR07
 */
public final class DotEnvLoader {

    private static final Logger LOG = Logger.getLogger(DotEnvLoader.class.getName());
    private static final String NOMBRE_ARCHIVO = ".development.env";

    /** Caché en memoria — se carga una sola vez (patrón lazy singleton) */
    private static volatile Map<String, String> valores = null;

    private DotEnvLoader() {
        // Clase utilitaria — no instanciar
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  API pública
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Retorna el mapa completo de propiedades del archivo .development.env.
     * La carga es lazy y thread-safe; el resultado queda en caché.
     * @return 
     */
    public static Map<String, String> cargar() {
        if (valores == null) {
            synchronized (DotEnvLoader.class) {
                if (valores == null) {
                    valores = leerArchivo();
                }
            }
        }
        return Collections.unmodifiableMap(valores);
    }

    /**
     * Obtiene el valor de una clave, retornando {@code null} si no existe.
     *
     * @param clave Nombre de la variable (p.ej. "MYSQL_HOST")
     * @return valor o null
     */
    public static String obtener(String clave) {
        return obtener(clave, null);
    }

    /**
     * Obtiene el valor de una clave con valor por defecto.
     * Orden de prioridad: archivo .env → variable OS → propiedad JVM → defecto.
     *
     * @param clave         Nombre de la variable
     * @param valorDefecto  Valor a retornar si no se encuentra en ninguna fuente
     * @return valor encontrado o valorDefecto
     */
    public static String obtener(String clave, String valorDefecto) {
        cargar();

        // 1. Archivo .env
        String valor = valores.get(clave);
        // 2. Variable de entorno del SO
        if (valor == null) valor = System.getenv(clave);
        // 3. Propiedad JVM (-Dclave=valor)
        if (valor == null) valor = System.getProperty(clave);
        // 4. Valor por defecto
        return valor != null ? valor : valorDefecto;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Métodos privados
    // ─────────────────────────────────────────────────────────────────────────

    private static Map<String, String> leerArchivo() {
        Map<String, String> mapa = new HashMap<>();
        File archivo = encontrarArchivo();

        if (archivo == null || !archivo.exists()) {
            // Intentar classpath
            try (java.io.InputStream is = DotEnvLoader.class.getResourceAsStream("/" + NOMBRE_ARCHIVO)) {
                if (is != null) {
                    LOG.info("[DotEnvLoader] Cargando configuracion desde classpath: /" + NOMBRE_ARCHIVO);
                    return leerDesdeInputStream(is);
                }
            } catch (IOException e) {
                LOG.severe("[DotEnvLoader] Error al leer classpath: " + e.getMessage());
            }
            LOG.warning("[DotEnvLoader] Archivo " + NOMBRE_ARCHIVO
                    + " no encontrado. Usando variables del sistema.");
            return mapa;
        }

        LOG.info("[DotEnvLoader] Cargando configuracion desde: " + archivo.getAbsolutePath());

        try (BufferedReader lector = new BufferedReader(
                new FileReader(archivo, StandardCharsets.UTF_8))) {

            String linea;
            while ((linea = lector.readLine()) != null) {
                linea = linea.trim();
                // Ignorar líneas vacías y comentarios
                if (linea.isEmpty() || linea.startsWith("#")) continue;

                int idx = linea.indexOf('=');
                if (idx > 0) {
                    String clave = linea.substring(0, idx).trim();
                    String valor = linea.substring(idx + 1).trim();
                    mapa.put(clave, valor);
                }
            }
            LOG.info("[DotEnvLoader] " + mapa.size() + " variables cargadas.");

        } catch (IOException e) {
            LOG.severe("[DotEnvLoader] Error al leer " + archivo.getAbsolutePath()
                    + ": " + e.getMessage());
        }
        return mapa;
    }

    private static File encontrarArchivo() {
        // 1. Propiedad del sistema ENV_FILE (configurable en Payara JVM Options)
        String ruta = System.getProperty("ENV_FILE");
        if (ruta != null) {
            File f = new File(ruta);
            if (f.exists()) return f;
        }

        // 2. Variable de entorno ENV_FILE
        ruta = System.getenv("ENV_FILE");
        if (ruta != null) {
            File f = new File(ruta);
            if (f.exists()) return f;
        }

        // 3. Directorio de trabajo actual del servidor
        File f = new File(System.getProperty("user.dir"), NOMBRE_ARCHIVO);
        if (f.exists()) return f;

        // 4. Directorio padre del directorio de trabajo
        File padre = new File(System.getProperty("user.dir")).getParentFile();
        if (padre != null) {
            f = new File(padre, NOMBRE_ARCHIVO);
            if (f.exists()) return f;
        }

        LOG.warning("[DotEnvLoader] No se encontro el archivo en ninguna ubicacion conocida.");
        return null;
    }

    private static Map<String, String> leerDesdeInputStream(java.io.InputStream is) throws IOException {
        Map<String, String> mapa = new HashMap<>();
        try (java.io.BufferedReader lector = new java.io.BufferedReader(
                new java.io.InputStreamReader(is, StandardCharsets.UTF_8))) {
            String linea;
            while ((linea = lector.readLine()) != null) {
                linea = linea.trim();
                if (linea.isEmpty() || linea.startsWith("#")) continue;
                int idx = linea.indexOf('=');
                if (idx > 0) {
                    String clave = linea.substring(0, idx).trim();
                    String valor = linea.substring(idx + 1).trim();
                    mapa.put(clave, valor);
                }
            }
        }
        LOG.info("[DotEnvLoader] " + mapa.size() + " variables cargadas desde classpath.");
        return mapa;
    }
}
