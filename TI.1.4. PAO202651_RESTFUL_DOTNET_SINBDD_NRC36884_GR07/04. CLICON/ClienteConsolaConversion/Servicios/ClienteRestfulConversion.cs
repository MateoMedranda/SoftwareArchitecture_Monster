using System.Net.Http.Json;
using System.Text.Json;
using ClienteConsolaConversion.Modelos;

namespace ClienteConsolaConversion.Servicios
{
    /// <summary>Cliente HTTP para la API RESTful de conversión de unidades.</summary>
    public class ClienteRestfulConversion
    {
        public const string MensajeConexion = "No se pudo conectar con el servidor";
        public const string MensajeTiempoAgotado = "El servicio tardó demasiado en responder. Verifica que esté iniciado e inténtalo de nuevo.";
        public const string MensajeRespuestaVacia = "El servidor no devolvió una respuesta válida.";

        private readonly HttpClient _httpClient;
        private readonly string _urlBase;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ClienteRestfulConversion(string urlBase)
        {
            if (string.IsNullOrWhiteSpace(urlBase))
            {
                throw new ArgumentException("La URL base es obligatoria.", nameof(urlBase));
            }

            _urlBase = urlBase.TrimEnd('/');
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_urlBase + "/"),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public string UrlBase => _urlBase;

        public async Task<RespuestaInicioSesion> IniciarSesionAsync(string usuario, string contrasena)
        {
            try
            {
                var solicitud = new
                {
                    Usuario = usuario,
                    Contrasena = contrasena
                };

                var respuestaHttp = await _httpClient.PostAsJsonAsync("api/autenticacion/login", solicitud, _jsonOptions);
                var contenido = await respuestaHttp.Content.ReadAsStringAsync();

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var respuestaLogin = JsonSerializer.Deserialize<RespuestaLoginRest>(contenido, _jsonOptions);
                    if (respuestaLogin is null || string.IsNullOrWhiteSpace(respuestaLogin.Token))
                    {
                        return NuevaRespuestaInicioSesion(MensajeRespuestaVacia);
                    }

                    return new RespuestaInicioSesion
                    {
                        Exito = true,
                        Token = respuestaLogin.Token,
                        Mensaje = string.Empty
                    };
                }

                var error = JsonSerializer.Deserialize<RespuestaErrorRest>(contenido, _jsonOptions);
                if (error is not null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    return NuevaRespuestaInicioSesion(error.Mensaje);
                }

                return NuevaRespuestaInicioSesion(MensajeRespuestaVacia);
            }
            catch (HttpRequestException)
            {
                return NuevaRespuestaInicioSesion(MensajeConexion);
            }
            catch (TaskCanceledException)
            {
                return NuevaRespuestaInicioSesion(MensajeTiempoAgotado);
            }
            catch
            {
                return NuevaRespuestaInicioSesion(MensajeConexion);
            }
        }

        public async Task<RespuestaConversion> ConvertirAsync(
            string token,
            CategoriaUnidad categoria,
            string unidadOrigen,
            string unidadDestino,
            double valor)
        {
            try
            {
                var ruta = categoria switch
                {
                    CategoriaUnidad.Temperatura => "api/conversion/temperatura",
                    CategoriaUnidad.Longitud => "api/conversion/longitud",
                    CategoriaUnidad.Masa => "api/conversion/masa",
                    _ => "api/conversion/temperatura"
                };

                var solicitud = new
                {
                    UnidadOrigen = unidadOrigen,
                    UnidadDestino = unidadDestino,
                    Valor = valor
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, ruta);
                request.Headers.Add("Authorization", "Bearer " + token);
                request.Content = JsonContent.Create(solicitud, options: _jsonOptions);

                var respuestaHttp = await _httpClient.SendAsync(request);
                var contenido = await respuestaHttp.Content.ReadAsStringAsync();

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var respuestaConversion = JsonSerializer.Deserialize<RespuestaConversionRest>(contenido, _jsonOptions);
                    if (respuestaConversion is null)
                    {
                        return NuevaRespuestaConversion(MensajeRespuestaVacia);
                    }

                    return new RespuestaConversion
                    {
                        Exito = true,
                        ValorConvertido = respuestaConversion.ValorConvertido,
                        Mensaje = string.Empty
                    };
                }

                var error = JsonSerializer.Deserialize<RespuestaErrorRest>(contenido, _jsonOptions);
                if (error is not null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    return NuevaRespuestaConversion(error.Mensaje);
                }

                return NuevaRespuestaConversion(MensajeRespuestaVacia);
            }
            catch (HttpRequestException)
            {
                return NuevaRespuestaConversion(MensajeConexion);
            }
            catch (TaskCanceledException)
            {
                return NuevaRespuestaConversion(MensajeTiempoAgotado);
            }
            catch
            {
                return NuevaRespuestaConversion(MensajeConexion);
            }
        }

        private static RespuestaInicioSesion NuevaRespuestaInicioSesion(string mensaje)
        {
            return new RespuestaInicioSesion
            {
                Exito = false,
                Mensaje = mensaje
            };
        }

        private static RespuestaConversion NuevaRespuestaConversion(string mensaje)
        {
            return new RespuestaConversion
            {
                Exito = false,
                ValorConvertido = 0.0,
                Mensaje = mensaje
            };
        }

        private class RespuestaLoginRest
        {
            public string Token { get; set; } = string.Empty;
            public DateTime Expiracion { get; set; }
        }

        private class RespuestaConversionRest
        {
            public double ValorConvertido { get; set; }
            public string? Mensaje { get; set; }
        }

        private class RespuestaErrorRest
        {
            public int Codigo { get; set; }
            public string? Mensaje { get; set; }
            public string? Detalle { get; set; }
        }
    }
}
