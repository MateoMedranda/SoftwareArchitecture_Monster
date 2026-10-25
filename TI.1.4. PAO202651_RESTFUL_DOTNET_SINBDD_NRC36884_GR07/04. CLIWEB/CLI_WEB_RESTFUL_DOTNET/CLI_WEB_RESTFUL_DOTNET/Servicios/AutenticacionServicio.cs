using System.Net;
using System.Net.Http.Json;
using CLI_WEB_RESTFUL_DOTNET.Modelos;

namespace CLI_WEB_RESTFUL_DOTNET.Servicios;

public interface IAutenticacionServicio
{
    Task<ResultadoLogin> IniciarSesionAsync(CredencialesLogin credenciales);
}

/// <summary>
/// Consume POST /api/autenticacion/login y traduce la respuesta REST
/// a los modelos de la aplicación.
/// </summary>
public class AutenticacionServicio : IAutenticacionServicio
{
    private readonly HttpClient _http;
    private readonly ILogger<AutenticacionServicio> _log;
    private readonly string _rutaLogin;

    public AutenticacionServicio(IHttpClientFactory fabrica, ILogger<AutenticacionServicio> log)
    {
        var opciones = OpcionesApiRest.CargarDesdeEnv();

        _http = fabrica.CreateClient();
        _http.BaseAddress = new Uri(opciones.UrlBase);
        _http.Timeout = TimeSpan.FromSeconds(opciones.TimeoutSegundos);
        _rutaLogin = opciones.RutaLogin;
        _log = log;
    }

    public async Task<ResultadoLogin> IniciarSesionAsync(CredencialesLogin credenciales)
    {
        try
        {
            var solicitud = new SolicitudLogin
            {
                Usuario = credenciales.NombreUsuario,
                Contrasena = credenciales.Contrasena
            };

            using var respuesta = await _http.PostAsJsonAsync(_rutaLogin, solicitud);

            if (respuesta.IsSuccessStatusCode)
            {
                var datos = await respuesta.Content.ReadFromJsonAsync<RespuestaLogin>();

                if (!string.IsNullOrEmpty(datos?.Token))
                    return new ResultadoLogin(true, datos.Token, "Sesión iniciada.");

                return new ResultadoLogin(false, null, "El servidor devolvió una respuesta vacía.");
            }

            var error = await LeerErrorAsync(respuesta);

            if (respuesta.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.BadRequest)
                return new ResultadoLogin(false, null, error.Mensaje ?? "Credenciales incorrectas.");

            return new ResultadoLogin(false, null, error.Mensaje ?? "No se pudo iniciar sesión.");
        }
        catch (TaskCanceledException ex)
        {
            _log.LogError(ex, $"Tiempo de espera agotado al llamar a {_rutaLogin}");
            return new ResultadoLogin(false, null, "El servidor tardó demasiado en responder.");
        }
        catch (HttpRequestException ex)
        {
            _log.LogError(ex, "Error de comunicación con la API REST");
            return new ResultadoLogin(false, null, "No se pudo contactar con el servidor.");
        }
    }

    private static async Task<RespuestaError> LeerErrorAsync(HttpResponseMessage respuesta)
    {
        try
        {
            return await respuesta.Content.ReadFromJsonAsync<RespuestaError>() ?? new RespuestaError();
        }
        catch
        {
            return new RespuestaError();
        }
    }
}
