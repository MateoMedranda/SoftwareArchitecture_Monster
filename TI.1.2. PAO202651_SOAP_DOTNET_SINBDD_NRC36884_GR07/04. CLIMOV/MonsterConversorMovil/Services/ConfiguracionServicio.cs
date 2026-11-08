using System.Text.Json;
using Microsoft.Maui.Storage;

namespace MonsterConversorMovil.Services;

/// <summary>Lee el endpoint incluido en appsettings.json dentro del paquete MAUI.</summary>
internal static class ConfiguracionServicio
{
    private const string EndpointPredeterminado = "http://10.0.2.2:49159/Servicios/ServicioConversionUnidades.svc";

    public static string ObtenerEndpointSoap()
    {
        try
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").GetAwaiter().GetResult();
            var configuracion = JsonSerializer.Deserialize<ConfiguracionAplicacion>(stream);
            var endpoint = configuracion?.ServicioSoap?.Url;

            return Uri.TryCreate(endpoint, UriKind.Absolute, out _) ? endpoint : EndpointPredeterminado;
        }
        catch (Exception)
        {
            return EndpointPredeterminado;
        }
    }

    private sealed class ConfiguracionAplicacion
    {
        public ServicioSoapConfiguracion? ServicioSoap { get; init; }
    }

    private sealed class ServicioSoapConfiguracion
    {
        public string? Url { get; init; }
    }
}
