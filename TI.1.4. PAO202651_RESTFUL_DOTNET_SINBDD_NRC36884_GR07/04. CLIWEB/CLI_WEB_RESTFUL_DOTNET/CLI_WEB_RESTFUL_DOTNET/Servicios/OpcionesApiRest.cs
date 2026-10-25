using CLI_WEB_RESTFUL_DOTNET.Modelos.Utilidades;

namespace CLI_WEB_RESTFUL_DOTNET.Servicios;

/// <summary>
/// Configuración de la API RESTful consumida por la aplicación. Los valores se
/// leen del archivo .development.env mediante <see cref="CargadorEnv"/>.
/// </summary>
public class OpcionesApiRest
{
    public const string ClaveUrlBase = "BASE_URL";
    public const string ClaveTimeout = "TIMEOUT_SEGUNDOS";
    public const string ClaveRutaLogin = "LOGIN_ENDPOINT";
    public const string ClaveRutaConversion = "CONVERSION_ENDPOINT";

    /// <summary>URL base de la API, por ejemplo https://localhost:5001.</summary>
    public string UrlBase { get; set; } = "https://localhost:44329";

    public int TimeoutSegundos { get; set; } = 30;

    /// <summary>Ruta del endpoint de autenticación.</summary>
    public string RutaLogin { get; set; } = "/api/autenticacion/login";

    /// <summary>Ruta base del endpoint de conversión (sin la categoría).</summary>
    public string RutaConversion { get; set; } = "/api/conversion";

    /// <summary>
    /// Carga las variables desde '.development.env' (buscándolo desde la carpeta
    /// indicada y sus padres) y construye las opciones. Si una variable no existe
    /// o es inválida se conserva el valor por defecto.
    /// </summary>
    public static OpcionesApiRest CargarDesdeEnv(string? rutaInicio = null)
    {
        var variables = CargadorEnv.Cargar(rutaInicio ?? Directory.GetCurrentDirectory());
        var opciones = new OpcionesApiRest();

        if (variables.TryGetValue(ClaveUrlBase, out var url) && !string.IsNullOrWhiteSpace(url))
            opciones.UrlBase = url;

        if (variables.TryGetValue(ClaveTimeout, out var timeout) &&
            int.TryParse(timeout, out var segundos) && segundos > 0)
            opciones.TimeoutSegundos = segundos;

        if (variables.TryGetValue(ClaveRutaLogin, out var rutaLogin) && !string.IsNullOrWhiteSpace(rutaLogin))
            opciones.RutaLogin = rutaLogin;

        if (variables.TryGetValue(ClaveRutaConversion, out var rutaConversion) && !string.IsNullOrWhiteSpace(rutaConversion))
            opciones.RutaConversion = rutaConversion;

        return opciones;
    }
}
