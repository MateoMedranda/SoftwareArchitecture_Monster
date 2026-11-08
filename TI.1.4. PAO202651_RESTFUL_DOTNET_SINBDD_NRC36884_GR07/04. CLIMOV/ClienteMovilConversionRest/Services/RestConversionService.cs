using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ClienteMovilConversionRest.Models;

namespace ClienteMovilConversionRest.Services;

/// <summary>Cliente HTTP para la API REST de conversiones protegida con JWT.</summary>
public sealed class RestConversionService : IRestConversionService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient httpClient = new() { Timeout = TimeSpan.FromSeconds(15) };
    private string endpoint = ConfiguracionServicio.ObtenerEndpointRest();

    public string Endpoint
    {
        get => endpoint;
        set
        {
            endpoint = value.Trim().TrimEnd('/');
            if (Uri.TryCreate(endpoint, UriKind.Absolute, out _))
                ConfiguracionServicio.GuardarEndpointRest(endpoint);
        }
    }

    public async Task<LoginResponse> SignInAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (!TryGetUri("api/autenticacion/login", out var uri, out var error))
            return new LoginResponse { Message = error };

        try
        {
            using var response = await httpClient.PostAsJsonAsync(uri, new { usuario = request.UserName, contrasena = request.Password }, JsonOptions, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new LoginResponse { Message = await ReadErrorAsync(response, cancellationToken) };

            var payload = await response.Content.ReadFromJsonAsync<ApiLoginResponse>(JsonOptions, cancellationToken);
            return string.IsNullOrWhiteSpace(payload?.Token)
                ? new LoginResponse { Message = "El servidor no devolvió un token de acceso." }
                : new LoginResponse { IsSuccessful = true, Token = payload.Token, Message = "Inicio de sesión correcto." };
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            return new LoginResponse { Message = Explain(exception) };
        }
    }

    public async Task<ConversionResponse> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken = default)
    {
        if (!TryGetUri($"api/conversion/{request.CategoryEndpoint}", out var uri, out var error))
            return new ConversionResponse { Message = error };

        try
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(new { valor = request.Value, unidadOrigen = request.SourceUnit, unidadDestino = request.TargetUnit }, options: JsonOptions)
            };
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);
            using var response = await httpClient.SendAsync(message, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new ConversionResponse { Message = await ReadErrorAsync(response, cancellationToken) };

            var payload = await response.Content.ReadFromJsonAsync<ApiConversionResponse>(JsonOptions, cancellationToken);
            return payload is null
                ? new ConversionResponse { Message = "El servidor devolvió una respuesta vacía." }
                : new ConversionResponse
                {
                    IsSuccessful = true,
                    ConvertedValue = payload.ValorConvertido,
                    Message = $"{payload.UnidadOrigen} a {payload.UnidadDestino}"
                };
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            return new ConversionResponse { Message = Explain(exception) };
        }
    }

    private bool TryGetUri(string route, out Uri uri, out string error)
    {
        if (!Uri.TryCreate(Endpoint, UriKind.Absolute, out var baseUri))
        {
            uri = null!;
            error = "La URL del servidor REST no es válida.";
            return false;
        }

        uri = new Uri(baseUri.ToString().TrimEnd('/') + "/" + route);
        error = string.Empty;
        return true;
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions, cancellationToken);
            if (!string.IsNullOrWhiteSpace(error?.Mensaje))
                return error.Mensaje;
        }
        catch (JsonException) { }

        return $"El servidor respondió con HTTP {(int)response.StatusCode} ({response.ReasonPhrase}).";
    }

    private static string Explain(Exception exception) => exception switch
    {
        TaskCanceledException => "El servidor tardó demasiado en responder. Verifica que esté iniciado y que la URL sea correcta.",
        HttpRequestException => $"No fue posible conectar con el servidor REST. {exception.InnerException?.Message ?? exception.Message}",
        _ => exception.Message
    };
}
