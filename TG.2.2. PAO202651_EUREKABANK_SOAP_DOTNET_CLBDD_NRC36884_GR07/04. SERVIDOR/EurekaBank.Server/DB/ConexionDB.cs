using Microsoft.Data.SqlClient;

namespace EurekaBank.Server;

// Construye la cadena de conexión a SQL Server (AWS RDS) a partir de variables
// de entorno. Se usa SqlConnectionStringBuilder para escapar correctamente
// contraseñas con caracteres especiales (; = { }).
public static class ConexionDB
{
    public static string Build(IConfiguration config)
    {
        // 1) Override total: ConnectionStrings:EurekaBank (appsettings o variable ConnectionStrings__EurekaBank).
        var explicitCs = config.GetConnectionString("EurekaBank");
        if (!string.IsNullOrWhiteSpace(explicitCs)) return explicitCs;

        // 2) Construcción desde variables granulares.
        var host = Require("DB_HOST");
        var port = config["DB_PORT"] ?? "1433";
        var name = config["DB_NAME"] ?? "arquitectura_db";

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = $"{host},{port}",
            InitialCatalog = name,
            UserID = Require("DB_USER"),
            Password = Require("DB_PASSWORD"),
            Encrypt = SqlConnectionEncryptOption.Mandatory, // AWS RDS: tráfico cifrado
            TrustServerCertificate = true,                  // RDS usa certificado propio
            ConnectTimeout = 30
        };

        return builder.ConnectionString;
    }

    private static string Require(string key) =>
        Environment.GetEnvironmentVariable(key) is { Length: > 0 } value
            ? value
            : throw new InvalidOperationException($"Falta la variable de entorno obligatoria '{key}'.");
}
