using Microsoft.Maui.Storage;

namespace CLIMOV_Facturacion.Config;

/// <summary>
/// Configuración del cliente móvil. En Android Emulator, 10.0.2.2 apunta
/// al equipo anfitrión donde se ejecuta el API.
/// </summary>
public static class AppConfig
{
    public const string UrlPorDefecto = "http://10.0.2.2:5226/";
    public const int TimeoutPorDefecto = 10;
    private const string ClaveUrlApi = "api_base_url";

    public static string ApiBaseUrl { get; private set; } = ObtenerUrlGuardada();
    public static int TimeoutSegundos { get; private set; } = TimeoutPorDefecto;

    public static bool IntentarActualizarUrl(string? valor, out string? error)
    {
        if (!Uri.TryCreate(valor?.Trim(), UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) ||
            string.IsNullOrWhiteSpace(uri.Host))
        {
            error = "Ingrese una URL válida, por ejemplo: http://192.168.1.20:5226/.";
            return false;
        }

        var normalizada = Normalizar(uri);
        ApiBaseUrl = normalizada;
        Preferences.Default.Set(ClaveUrlApi, normalizada);
        error = null;
        return true;
    }

    private static string ObtenerUrlGuardada()
    {
        var guardada = Preferences.Default.Get(ClaveUrlApi, UrlPorDefecto);
        return Uri.TryCreate(guardada, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
               !string.IsNullOrWhiteSpace(uri.Host)
            ? Normalizar(uri)
            : UrlPorDefecto;
    }

    private static string Normalizar(Uri uri)
    {
        var baseUri = new UriBuilder(uri)
        {
            Query = string.Empty,
            Fragment = string.Empty,
            Path = uri.AbsolutePath.TrimEnd('/') + "/"
        };

        return baseUri.Uri.AbsoluteUri;
    }
}
