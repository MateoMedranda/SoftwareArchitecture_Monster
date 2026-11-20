package ec.edu.monster.conexion;

import ec.edu.monster.configuracion.DotEnvLoader;
import com.zaxxer.hikari.HikariConfig;
import com.zaxxer.hikari.HikariDataSource;
import jakarta.annotation.PostConstruct;
import jakarta.annotation.PreDestroy;
import jakarta.ejb.Singleton;
import jakarta.ejb.Startup;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.security.GeneralSecurityException;
import java.security.KeyStore;
import java.sql.Connection;
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
 * Las conexiones se sirven desde un pool HikariCP configurado al arrancar;
 * cada llamada a getConexion() toma una conexión del pool y el llamador es
 * responsable de devolverla cerrándola (preferiblemente con try-with-resources).
 *
 * Variables leídas del .development.env:
 *   MYSQL_HOST, MYSQL_PUERTO, MYSQL_DATABASE,
 *   MYSQL_USER, MYSQL_PASSWORD,
 *   MYSQL_ALLOW_PUBLIC_KEY_RETRIEVAL,
 *   MYSQL_SSL_MODE, MYSQL_CONN_TIMEOUT, MYSQL_SOCKET_TIMEOUT,
 *   MYSQL_CLIENT_KEYSTORE_URL (opcional),
 *   MYSQL_POOL_MAX, MYSQL_POOL_MIN, MYSQL_POOL_IDLE_TIMEOUT,
 *   MYSQL_POOL_MAX_LIFETIME, MYSQL_POOL_KEEPALIVE
 *
 * MYSQL_SSL_MODE usa los valores de Connector/J: DISABLED, PREFERRED,
 * REQUIRED, VERIFY_CA o VERIFY_IDENTITY. Por defecto REQUIRED (cifrado
 * sin validar la CA; necesario para RDS si el truststore del JRE no
 * incluye la CA de Amazon).
 *
 * @author MONSTER GR07
 */
@Singleton
@Startup
public class ConexionBDD {

    private static final Logger LOG = Logger.getLogger(ConexionBDD.class.getName());

    /** Contraseña del keystore de cliente vacío que se genera al arrancar. */
    private static final String CLIENT_KEYSTORE_PASSWORD = "changeit";

    private String urlJdbc;
    private String usuario;
    private String contraseania;

    /** Pool de conexiones físicas reutilizables hacia MySQL/RDS. */
    private HikariDataSource dataSource;

    /**
     * Keystore de cliente vacío. Payara define javax.net.ssl.keyStore a nivel de
     * JVM y Connector/J lo usa como certificado de cliente; MySQL/RDS rechaza
     * ese certificado con el alerta TLS "unknown_ca". Apuntando a un keystore
     * sin entradas evitamos que se envíe un certificado de cliente.
     */
    private File clientKeyStore;

    // ─────────────────────────────────────────────────────────────────────────
    //  Inicialización
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Carga la configuración desde .development.env al arrancar el servidor.
     * También registra el driver MySQL en el DriverManager.
     */
    @PostConstruct
    private void inicializar() {
        String host          = DotEnvLoader.obtener("MYSQL_HOST");
        String puerto        = DotEnvLoader.obtener("MYSQL_PUERTO");
        String base          = DotEnvLoader.obtener("MYSQL_DATABASE");
        String pki           = DotEnvLoader.obtener("MYSQL_ALLOW_PUBLIC_KEY_RETRIEVAL", "true");
        String sslMode       = DotEnvLoader.obtener("MYSQL_SSL_MODE", "REQUIRED");
        String connTimeout   = DotEnvLoader.obtener("MYSQL_CONN_TIMEOUT", "10000");
        String socketTimeout = DotEnvLoader.obtener("MYSQL_SOCKET_TIMEOUT", "30000");
        String clientKsUrl   = DotEnvLoader.obtener("MYSQL_CLIENT_KEYSTORE_URL");

        usuario      = DotEnvLoader.obtener("MYSQL_USER");
        contraseania = DotEnvLoader.obtener("MYSQL_PASSWORD");

        StringBuilder url = new StringBuilder("jdbc:mysql://")
                .append(host).append(":").append(puerto).append("/").append(base)
                .append("?allowPublicKeyRetrieval=").append(pki)
                .append("&sslMode=").append(sslMode)
                .append("&serverTimezone=America/Guayaquil")
                .append("&characterEncoding=UTF-8")
                .append("&connectTimeout=").append(connTimeout)
                .append("&socketTimeout=").append(socketTimeout);

        // Evitar que Connector/J use el keystore de cliente de Payara.
        if (clientKsUrl == null || clientKsUrl.isBlank()) {
            try {
                clientKeyStore = crearKeyStoreClienteVacio();
                clientKsUrl = clientKeyStore.toURI().toString();
            } catch (IOException | GeneralSecurityException e) {
                LOG.warning("[ConexionBDD] No se pudo crear el keystore de cliente vacio: "
                        + e.getMessage());
            }
        }
        if (clientKsUrl != null && !clientKsUrl.isBlank()) {
            url.append("&clientCertificateKeyStoreUrl=").append(clientKsUrl)
               .append("&clientCertificateKeyStoreType=PKCS12")
               .append("&clientCertificateKeyStorePassword=").append(CLIENT_KEYSTORE_PASSWORD);
        }

        urlJdbc = url.toString();

        LOG.info("[ConexionBDD] URL JDBC configurada: jdbc:mysql://"
                + host + ":" + puerto + "/" + base + " (sslMode=" + sslMode + ")");

        // Cargar el driver de forma explícita (necesario en algunos contenedores)
        try {
            Class.forName("com.mysql.cj.jdbc.Driver");
            LOG.info("[ConexionBDD] Driver MySQL registrado correctamente.");
        } catch (ClassNotFoundException e) {
            LOG.severe("[ConexionBDD] Driver MySQL no encontrado en el classpath: " + e.getMessage());
        }

        inicializarPool();
    }

    /**
     * Crea el pool HikariCP con un tamaño pensado para pocos clientes
     * concurrentes (por defecto 5). Las conexiones físicas se abren una sola
     * vez y se reutilizan, evitando el handshake TCP/TLS por cada consulta.
     */
    private void inicializarPool() {
        int poolMax = entero("MYSQL_POOL_MAX", 5, 1);
        int poolMin = entero("MYSQL_POOL_MIN", 2, 0);
        if (poolMin > poolMax) poolMin = poolMax;

        long connTimeoutMs   = largo("MYSQL_CONN_TIMEOUT", 10_000L, 250L);
        long idleTimeoutMs   = largo("MYSQL_POOL_IDLE_TIMEOUT", 300_000L, 10_000L);
        long maxLifetimeMs   = largo("MYSQL_POOL_MAX_LIFETIME", 1_500_000L, 30_000L);
        long keepaliveMs     = largo("MYSQL_POOL_KEEPALIVE", 120_000L, 30_000L);

        HikariConfig config = new HikariConfig();
        config.setJdbcUrl(urlJdbc);
        config.setUsername(usuario);
        config.setPassword(contraseania);
        config.setPoolName("EurekaBankPool");
        config.setMaximumPoolSize(poolMax);
        config.setMinimumIdle(poolMin);
        config.setConnectionTimeout(connTimeoutMs);
        config.setValidationTimeout(5_000L);
        config.setIdleTimeout(idleTimeoutMs);
        config.setMaxLifetime(maxLifetimeMs);
        config.setKeepaliveTime(keepaliveMs);
        config.setAutoCommit(true);
        // No bloquear el despliegue si RDS está temporalmente inaccesible.
        config.setInitializationFailTimeout(-1L);

        // Optimizaciones de Connector/J aplicadas a cada conexión del pool.
        config.addDataSourceProperty("cachePrepStmts", "true");
        config.addDataSourceProperty("useServerPrepStmts", "true");
        config.addDataSourceProperty("prepStmtCacheSize", "250");
        config.addDataSourceProperty("prepStmtCacheSqlLimit", "2048");
        config.addDataSourceProperty("tcpKeepAlive", "true");
        config.addDataSourceProperty("rewriteBatchedStatements", "true");

        dataSource = new HikariDataSource(config);
        LOG.info("[ConexionBDD] Pool HikariCP iniciado: max=" + poolMax
                + ", min=" + poolMin + ", idleTimeout=" + idleTimeoutMs + "ms");
    }

    /** Lee un entero del .development.env con valor por defecto y mínimo. */
    private int entero(String clave, int valorDefecto, int minimo) {
        try {
            int valor = Integer.parseInt(DotEnvLoader.obtener(clave, String.valueOf(valorDefecto)).trim());
            return Math.max(valor, minimo);
        } catch (NumberFormatException e) {
            LOG.warning("[ConexionBDD] Valor invalido para " + clave + "; se usa " + valorDefecto);
            return valorDefecto;
        }
    }

    /** Lee un long del .development.env con valor por defecto y mínimo. */
    private long largo(String clave, long valorDefecto, long minimo) {
        try {
            long valor = Long.parseLong(DotEnvLoader.obtener(clave, String.valueOf(valorDefecto)).trim());
            return Math.max(valor, minimo);
        } catch (NumberFormatException e) {
            LOG.warning("[ConexionBDD] Valor invalido para " + clave + "; se usa " + valorDefecto);
            return valorDefecto;
        }
    }

    /**
     * Crea en el directorio temporal un keystore PKCS12 sin entradas.
     * Se usa como "certificado de cliente" para que Connector/J no envíe
     * el certificado configurado por Payara en javax.net.ssl.keyStore.
     */
    private File crearKeyStoreClienteVacio() throws IOException, GeneralSecurityException {
        KeyStore keyStore = KeyStore.getInstance("PKCS12");
        keyStore.load(null, null);
        File archivo = File.createTempFile("eurekabank-client-", ".p12");
        try (OutputStream salida = new FileOutputStream(archivo)) {
            keyStore.store(salida, CLIENT_KEYSTORE_PASSWORD.toCharArray());
        }
        archivo.deleteOnExit();
        return archivo;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  API pública
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Toma una conexión del pool HikariCP.
     *
     * <pre>
     * Uso recomendado:
     *   try (Connection conn = conexionBDD.getConexion()) {
     *       // usar conn ...
     *   }
     * </pre>
     *
     * @return Connection listo para usar
     * @throws SQLException si el pool no está inicializado o no puede entregar
     *                      una conexión dentro del timeout configurado
     */
    public Connection getConexion() throws SQLException {
        if (dataSource == null) {
            throw new SQLException("El pool de conexiones no esta inicializado.");
        }
        return dataSource.getConnection();
    }

    /** Cierra el pool cuando Payara detiene o redespliega la aplicación. */
    @PreDestroy
    private void cerrarPool() {
        if (dataSource != null) {
            dataSource.close();
            LOG.info("[ConexionBDD] Pool HikariCP cerrado.");
        }
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
