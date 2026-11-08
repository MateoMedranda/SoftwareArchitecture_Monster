package ec.edu.monster.conexion;

import ec.edu.monster.configuracion.DotEnvLoader;
import jakarta.annotation.PostConstruct;
import jakarta.ejb.Singleton;
import jakarta.ejb.Startup;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;
import java.util.logging.Logger;

/**
 * Gestiona la configuración de acceso a la base de datos MySQL.
 *
 * Implementado como EJB Singleton (@Singleton + @Startup) para garantizar
 * que exista una única instancia durante toda la vida de la aplicación y
 * que la configuración se cargue al arrancar el servidor.
 *
 * La configuración se lee desde .development.env a través de DotEnvLoader.
 * Cada llamada a getConexion() abre una nueva conexión JDBC; el llamador
 * es responsable de cerrarla (preferiblemente con try-with-resources).
 *
 * Variables leídas del .development.env:
 *   MYSQL_HOST, MYSQL_PUERTO, MYSQL_DATABASE,
 *   MYSQL_USER, MYSQL_PASSWORD,
 *   MYSQL_ALLOW_PUBLIC_KEY_RETRIEVAL, MYSQL_USE_SSL
 *
 * @author MONSTER GR07
 */
@Singleton
@Startup
public class ConexionBDD {

    private static final Logger LOG = Logger.getLogger(ConexionBDD.class.getName());

    private String urlJdbc;
    private String usuario;
    private String contraseania;

    // ─────────────────────────────────────────────────────────────────────────
    //  Inicialización
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Carga la configuración desde .development.env al arrancar el servidor.
     * También registra el driver MySQL en el DriverManager.
     */
    @PostConstruct
    private void inicializar() {
        String host   = DotEnvLoader.obtener("MYSQL_HOST");
        String puerto = DotEnvLoader.obtener("MYSQL_PUERTO");
        String base   = DotEnvLoader.obtener("MYSQL_DATABASE");
        String pki    = DotEnvLoader.obtener("MYSQL_ALLOW_PUBLIC_KEY_RETRIEVAL");
        String ssl    = DotEnvLoader.obtener("MYSQL_USE_SSL");

        usuario      = DotEnvLoader.obtener("MYSQL_USER");
        contraseania = DotEnvLoader.obtener("MYSQL_PASSWORD");

        urlJdbc = "jdbc:mysql://" + host + ":" + puerto + "/" + base
                + "?allowPublicKeyRetrieval=" + pki
                + "&useSSL=" + ssl
                + "&serverTimezone=America/Guayaquil"
                + "&characterEncoding=UTF-8"
                + "&autoReconnect=true";

        LOG.info("[ConexionBDD] URL JDBC configurada: jdbc:mysql://"
                + host + ":" + puerto + "/" + base);

        // Cargar el driver de forma explícita (necesario en algunos contenedores)
        try {
            Class.forName("com.mysql.cj.jdbc.Driver");
            LOG.info("[ConexionBDD] Driver MySQL registrado correctamente.");
        } catch (ClassNotFoundException e) {
            LOG.severe("[ConexionBDD] Driver MySQL no encontrado en el classpath: " + e.getMessage());
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  API pública
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Abre y retorna una nueva conexión JDBC a la base de datos.
     *
     * <pre>
     * Uso recomendado:
     *   try (Connection conn = conexionBDD.getConexion()) {
     *       // usar conn ...
     *   }
     * </pre>
     *
     * @return Connection listo para usar
     * @throws SQLException si no se puede establecer la conexión
     */
    public Connection getConexion() throws SQLException {
        return DriverManager.getConnection(urlJdbc, usuario, contraseania);
    }

    /**
     * Comprueba si la base de datos es alcanzable (timeout 3 segundos).
     *
     * @return true si la conexión es válida, false en caso contrario
     */
    public boolean estaDisponible() {
        try (Connection conn = getConexion()) {
            boolean valida = conn.isValid(3);
            LOG.info("[ConexionBDD] Estado de la base de datos: " + (valida ? "DISPONIBLE" : "NO DISPONIBLE"));
            return valida;
        } catch (SQLException e) {
            LOG.warning("[ConexionBDD] Base de datos no disponible: " + e.getMessage());
            return false;
        }
    }

    /**
     * Retorna la URL JDBC configurada (sin credenciales), útil para diagnóstico.
     */
    public String getUrlJdbc() {
        return urlJdbc;
    }
}
