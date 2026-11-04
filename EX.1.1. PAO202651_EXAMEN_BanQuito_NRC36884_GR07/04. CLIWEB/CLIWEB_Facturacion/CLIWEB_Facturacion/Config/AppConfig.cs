using DotNetEnv;

namespace CLIWEB_Facturacion.Config;

/// <summary>
/// Configuración del cliente leída desde el archivo .env.
/// Si el archivo no existe o una variable no está definida, se usan
/// los valores por defecto que apuntan al API de facturación local.
/// </summary>
public static class AppConfig
{
    public const string UrlPorDefecto = "http://localhost:5226/";
    public const int TimeoutPorDefecto = 10;

    public static string ApiBaseUrl { get; private set; } = UrlPorDefecto;
    public static int TimeoutSegundos { get; private set; } = TimeoutPorDefecto;

    public static void Cargar(string raizContenido)
    {
        var rutaEnv = Path.Combine(raizContenido, ".env");
        if (File.Exists(rutaEnv))
        {
            Env.Load(rutaEnv);
        }

        var url = Environment.GetEnvironmentVariable("API_BASE_URL");
        if (!string.IsNullOrWhiteSpace(url))
        {
            url = url.Trim();
            if (url.Length >= 2 &&
                ((url[0] == '"' && url[^1] == '"') || (url[0] == '\'' && url[^1] == '\'')))
            {
                url = url[1..^1];
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                ApiBaseUrl = url.TrimEnd('/') + "/";
            }
        }

        if (int.TryParse(Environment.GetEnvironmentVariable("API_TIMEOUT_SECONDS"), out var segundos) && segundos > 0)
        {
            TimeoutSegundos = segundos;
        }
    }
}
