namespace EurekaBank.Mobile.Servicios;

/// <summary>Conserva el endpoint elegido por el usuario solo en el dispositivo.</summary>
public static class ConfiguracionServidor
{
    private const string ClaveUrl = "EurekaBank.SoapEndpoint";
    public const string UrlPredeterminada = "http://10.0.2.2:62556/EurekaBankSoap.svc";

    public static string Url => Preferences.Default.Get(ClaveUrl,
        Environment.GetEnvironmentVariable("EUREKABANK_URL") ?? UrlPredeterminada);

    public static bool IntentarGuardar(string? entrada, out string url, out string error)
    {
        var valor = (entrada ?? string.Empty).Trim().TrimEnd('/');
        if (!Uri.TryCreate(valor, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            url = Url;
            error = "Escribe una URL HTTP o HTTPS válida.";
            return false;
        }

        url = uri.AbsolutePath == "/" || string.IsNullOrEmpty(uri.AbsolutePath)
            ? $"{valor}/EurekaBankSoap.svc"
            : valor;

        Preferences.Default.Set(ClaveUrl, url);
        error = string.Empty;
        return true;
    }
}
