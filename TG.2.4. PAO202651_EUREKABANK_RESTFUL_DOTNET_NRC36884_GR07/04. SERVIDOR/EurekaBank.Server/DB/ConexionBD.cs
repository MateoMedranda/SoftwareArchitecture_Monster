using Microsoft.Data.SqlClient;

namespace EurekaBank.Server;

// Construye la cadena de conexión a SQL Server (AWS RDS) a partir de las
// variables cargadas desde .development.env. Se usa SqlConnectionStringBuilder
// para escapar correctamente contraseñas con caracteres especiales (; = { }).
//
// Precedencia:
//   1) Override de emergencia: ConnectionStrings__EurekaBank (variable de entorno).
//   2) Construcción con las variables DB_* del .development.env.
//   3) Respaldo: ConnectionStrings:EurekaBank de appsettings.json.
public static class ConexionDB
{
    public static string Build(IConfiguration config)
    {
        // 1) Override de emergencia (por ejemplo, en IIS o ante un failover de RDS).
        var emergency = Environment.GetEnvironmentVariable("ConnectionStrings__EurekaBank");
        if (!string.IsNullOrWhiteSpace(emergency)) return emergency;

        // 2) Construcción desde los parámetros del .development.env.
        var built = TryBuildFromVariables(config);
        if (built is not null) return built;

        // 3) Respaldo en appsettings.json (entorno local / docker compose).
        var fallback = config.GetConnectionString("EurekaBank");
        if (!string.IsNullOrWhiteSpace(fallback)) return fallback;

        throw new InvalidOperationException(
            "No hay cadena de conexión: defina DB_HOST/DB_USER/DB_PASSWORD en .development.env " +
            "o ConnectionStrings:EurekaBank en appsettings.json.");
    }

    private static string? TryBuildFromVariables(IConfiguration config)
    {
        var host = config["DB_HOST"];
        var user = config["DB_USER"];
        var password = config["DB_PASSWORD"];

        // Si el .env no trae los datos mínimos, se usa el respaldo de appsettings.json.
        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(user) ||
            string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var port = config["DB_PORT"] ?? "1433";
        var name = config["DB_NAME"] ?? "arquitectura_db";

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = $"{host},{port}",
            InitialCatalog = name,
            UserID = user,
            Password = password,
            Encrypt = SqlConnectionEncryptOption.Mandatory, // AWS RDS: tráfico cifrado
            TrustServerCertificate = true,                  // RDS usa certificado propio
            ConnectTimeout = ReadInt(config, "DB_CONNECT_TIMEOUT", 30),

            // Pool pequeño para una instancia RDS gratuita (poca memoria y CPU).
            Pooling = true,
            MinPoolSize = ReadInt(config, "DB_MIN_POOL", 1),
            MaxPoolSize = ReadInt(config, "DB_MAX_POOL", 6),
            // Recicla conexiones para sobrevivir a failovers/reinicios de RDS.
            LoadBalanceTimeout = ReadInt(config, "DB_CONNECTION_LIFETIME", 300),

            ApplicationName = "EurekaBank.Server",
            MultipleActiveResultSets = false,
            PersistSecurityInfo = false
        };

        return builder.ConnectionString;
    }

    private static int ReadInt(IConfiguration config, string key, int defaultValue) =>
        int.TryParse(config[key], out var value) && value >= 0 ? value : defaultValue;
}
