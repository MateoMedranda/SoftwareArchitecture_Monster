namespace CLIMOV_Facturacion.Config;

/// <summary>
/// Configuración del cliente móvil. En Android Emulator, 10.0.2.2 apunta
/// al equipo anfitrión donde se ejecuta el API.
/// </summary>
public static class AppConfig
{
    public const string UrlPorDefecto = "http://10.0.2.2:5226/";
    public const int TimeoutPorDefecto = 10;

    public static string ApiBaseUrl { get; } = UrlPorDefecto;
    public static int TimeoutSegundos { get; private set; } = TimeoutPorDefecto;
}
