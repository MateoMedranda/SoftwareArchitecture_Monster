using System.Text.Json;
using Microsoft.Maui.Storage;

namespace ClienteMovilConversionRest.Services;

internal static class ConfiguracionServicio
{
    private const string UrlPredeterminada = "http://10.0.2.2:60980";
    private const string ClaveUrlPersonalizada = "servicio_rest_url";

    public static string ObtenerEndpointRest()
    {
        var urlAppSettings = LeerUrlAppSettings();
        return Preferences.Default.Get(ClaveUrlPersonalizada, urlAppSettings);
    }

    public static void GuardarEndpointRest(string url) => Preferences.Default.Set(ClaveUrlPersonalizada, url);

    private static string LeerUrlAppSettings()
    {
        try
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").GetAwaiter().GetResult();
            var configuracion = JsonSerializer.Deserialize<ConfiguracionAplicacion>(stream);
            var url = configuracion?.ServicioRest?.Url;
            return Uri.TryCreate(url, UriKind.Absolute, out _) ? url : UrlPredeterminada;
        }
        catch
        {
            return UrlPredeterminada;
        }
    }

    private sealed class ConfiguracionAplicacion
    {
        public ServicioRestConfiguracion? ServicioRest { get; init; }
    }

    private sealed class ServicioRestConfiguracion
    {
        public string? Url { get; init; }
    }
}
