using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CLI_WEB_RESTFUL_DOTNET.Modelos;
using CLI_WEB_RESTFUL_DOTNET.Modelos.Conversiones;

namespace CLI_WEB_RESTFUL_DOTNET.Servicios;

public interface IConversionServicio
{
    Task<ResultadoConversionModelo> ConvertirAsync(string token, SolicitudConversionModelo solicitud);
}

/// <summary>
/// Consume POST /api/conversion/{categoria} enviando el token JWT en la
/// cabecera Authorization y traduce la respuesta REST a los modelos de la aplicación.
/// </summary>
public class ConversionServicio : IConversionServicio
{
    private readonly HttpClient _http;
    private readonly ILogger<ConversionServicio> _log;
    private readonly string _rutaConversion;

    public ConversionServicio(IHttpClientFactory fabrica, ILogger<ConversionServicio> log)
    {
        var opciones = OpcionesApiRest.CargarDesdeEnv();

        _http = fabrica.CreateClient();
        _http.BaseAddress = new Uri(opciones.UrlBase);
        _http.Timeout = TimeSpan.FromSeconds(opciones.TimeoutSegundos);
        _rutaConversion = opciones.RutaConversion;
        _log = log;
    }

    public async Task<ResultadoConversionModelo> ConvertirAsync(string token, SolicitudConversionModelo solicitud)
    {
        try
        {
            var cuerpo = new SolicitudConversion
            {
                Valor = (double?)solicitud.Valor,
                UnidadOrigen = solicitud.UnidadOrigen,
                UnidadDestino = solicitud.UnidadDestino
            };

            using var peticion = new HttpRequestMessage(HttpMethod.Post, UrlDeConversion(solicitud.Categoria))
            {
                Content = JsonContent.Create(cuerpo)
            };
            peticion.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var respuesta = await _http.SendAsync(peticion);

            if (respuesta.IsSuccessStatusCode)
            {
                var datos = await respuesta.Content.ReadFromJsonAsync<RespuestaConversion>();

                return datos is null
                    ? new ResultadoConversionModelo(false, null, "El servidor devolvió una respuesta vacía.")
                    : new ResultadoConversionModelo(true, datos.ValorConvertido, string.Empty);
            }

            var error = await LeerErrorAsync(respuesta);

            if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
                return new ResultadoConversionModelo(false, null, "La sesión ha expirado.");

            if (respuesta.StatusCode == HttpStatusCode.BadRequest)
                return new ResultadoConversionModelo(false, null,
                    error.Mensaje ?? error.Detalle ?? "La solicitud de conversión no es válida.");

            return new ResultadoConversionModelo(false, null,
                error.Mensaje ?? "No se pudo realizar la conversión.");
        }
        catch (TaskCanceledException ex)
        {
            _log.LogError(ex, "Tiempo de espera agotado al llamar a la API de conversión");
            return new ResultadoConversionModelo(false, null, "El servidor tardó demasiado en responder.");
        }
        catch (HttpRequestException ex)
        {
            _log.LogError(ex, "Error de comunicación con la API REST");
            return new ResultadoConversionModelo(false, null, "Error de comunicación con el servidor.");
        }
    }

    /// <summary>La categoría de la vista coincide con el segmento del endpoint REST.</summary>
    private string UrlDeConversion(string categoria) =>
        $"{_rutaConversion.TrimEnd('/')}/{categoria.ToLowerInvariant()}";

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
