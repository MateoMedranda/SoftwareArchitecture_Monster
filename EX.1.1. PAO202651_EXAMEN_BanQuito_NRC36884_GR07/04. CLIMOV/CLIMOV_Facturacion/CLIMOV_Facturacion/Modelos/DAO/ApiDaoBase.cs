using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CLIMOV_Facturacion.Servicios;

namespace CLIMOV_Facturacion.Modelos.DAO;

/// <summary>
/// Base de los DAOs. Encapsula HttpClient, la deserialización JSON
/// y la traducción de errores del API a <see cref="ApiException"/>.
/// </summary>
public abstract class ApiDaoBase(HttpClient http)
{
    private static readonly JsonSerializerOptions Opciones = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    protected HttpClient Http { get; } = http;

    protected Task<T> GetAsync<T>(string ruta, CancellationToken ct = default) =>
        EnviarAsync<T>(() => Http.GetAsync(ruta, ct), ct);

    protected Task<T> PostAsync<T>(string ruta, object cuerpo, CancellationToken ct = default) =>
        EnviarAsync<T>(() => Http.PostAsJsonAsync(ruta, cuerpo, Opciones, ct), ct);

    protected Task<T> PutAsync<T>(string ruta, object cuerpo, CancellationToken ct = default) =>
        EnviarAsync<T>(() => Http.PutAsJsonAsync(ruta, cuerpo, Opciones, ct), ct);

    protected Task<T> DeleteAsync<T>(string ruta, CancellationToken ct = default) =>
        EnviarAsync<T>(() => Http.DeleteAsync(ruta, ct), ct);

    private async Task<T> EnviarAsync<T>(Func<Task<HttpResponseMessage>> accion, CancellationToken ct)
    {
        HttpResponseMessage respuesta;
        try
        {
            respuesta = await accion();
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new ApiException(408, "El servidor tardó demasiado en responder.");
        }
        catch (HttpRequestException)
        {
            throw new ApiException(503, "No se pudo conectar con el API de facturación. Verifique que esté en ejecución.");
        }

        using (respuesta)
        {
            if (!respuesta.IsSuccessStatusCode)
            {
                throw await CrearExcepcionAsync(respuesta, ct);
            }

            var json = await respuesta.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(json))
            {
                return default!;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(json, Opciones)
                    ?? throw new ApiException((int)respuesta.StatusCode, "El servidor devolvió una respuesta vacía.");
            }
            catch (JsonException)
            {
                throw new ApiException((int)respuesta.StatusCode, "El servidor devolvió una respuesta con formato inválido.");
            }
        }
    }

    private static async Task<ApiException> CrearExcepcionAsync(HttpResponseMessage respuesta, CancellationToken ct)
    {
        var estado = (int)respuesta.StatusCode;
        var mensaje = estado switch
        {
            400 => "La solicitud contiene datos inválidos.",
            404 => "El registro solicitado no existe.",
            409 => "La operación no se puede completar por una regla de negocio.",
            500 => "Ocurrió un error interno en el servidor.",
            503 => "El servicio no está disponible.",
            _ => $"No se pudo completar la solicitud ({estado})."
        };

        try
        {
            var json = await respuesta.Content.ReadAsStringAsync(ct);
            if (!string.IsNullOrWhiteSpace(json))
            {
                using var documento = JsonDocument.Parse(json);
                var raiz = documento.RootElement;

                if (raiz.TryGetProperty("mensaje", out var propiedadMensaje) &&
                    propiedadMensaje.ValueKind == JsonValueKind.String &&
                    !string.IsNullOrWhiteSpace(propiedadMensaje.GetString()))
                {
                    mensaje = propiedadMensaje.GetString()!;
                }
                else if (raiz.TryGetProperty("errors", out var errores) && errores.ValueKind == JsonValueKind.Object)
                {
                    var lista = new List<string>();
                    foreach (var propiedad in errores.EnumerateObject())
                    {
                        if (propiedad.Value.ValueKind != JsonValueKind.Array)
                        {
                            continue;
                        }

                        foreach (var item in propiedad.Value.EnumerateArray())
                        {
                            if (item.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(item.GetString()))
                            {
                                lista.Add(item.GetString()!);
                            }
                        }
                    }

                    if (lista.Count > 0)
                    {
                        mensaje = string.Join(" ", lista);
                    }
                }
                else if (raiz.TryGetProperty("title", out var titulo) &&
                         titulo.ValueKind == JsonValueKind.String &&
                         !string.IsNullOrWhiteSpace(titulo.GetString()))
                {
                    mensaje = titulo.GetString()!;
                }
            }
        }
        catch (JsonException)
        {
            // Se conserva el mensaje por defecto.
        }

        if (respuesta.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            mensaje = "El servicio BanQuito no está disponible. Verifique que esté en ejecución.";
        }

        return new ApiException(estado, mensaje);
    }
}
