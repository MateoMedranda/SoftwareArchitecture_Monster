using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using CLIESC_Facturacion.Config;
using CLIESC_Facturacion.Modelos;

namespace CLIESC_Facturacion.Servicios
{
    /// <summary>
    /// Cliente HTTP comun para el API de facturacion.
    /// La URL base y el timeout se cargan desde .env (ver AppConfig).
    /// </summary>
    public sealed class ApiService
    {
        private readonly HttpClient _cliente;

        public ApiService()
        {
            var baseUrl = AppConfig.ApiBaseUrl;
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            _cliente = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(AppConfig.TimeoutSeconds)
            };
            _cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AutenticacionMock.Token);
        }

        public Task<T> GetAsync<T>(string ruta)
        {
            return EnviarAsync<T>(new HttpRequestMessage(HttpMethod.Get, ruta));
        }

        public Task<T> PostAsync<T>(string ruta, object cuerpo)
        {
            var solicitud = new HttpRequestMessage(HttpMethod.Post, ruta)
            {
                Content = CrearContenido(cuerpo)
            };
            return EnviarAsync<T>(solicitud);
        }

        public Task<T> PutAsync<T>(string ruta, object cuerpo)
        {
            var solicitud = new HttpRequestMessage(HttpMethod.Put, ruta)
            {
                Content = CrearContenido(cuerpo)
            };
            return EnviarAsync<T>(solicitud);
        }

        public async Task DeleteAsync(string ruta)
        {
            using (var respuesta = await _cliente.DeleteAsync(ruta).ConfigureAwait(false))
            {
                if (!respuesta.IsSuccessStatusCode)
                {
                    throw await CrearExcepcionAsync(respuesta).ConfigureAwait(false);
                }
            }
        }

        private async Task<T> EnviarAsync<T>(HttpRequestMessage solicitud)
        {
            using (solicitud)
            using (var respuesta = await _cliente.SendAsync(solicitud).ConfigureAwait(false))
            {
                if (!respuesta.IsSuccessStatusCode)
                {
                    throw await CrearExcepcionAsync(respuesta).ConfigureAwait(false);
                }

                var json = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);
                return Deserializar<T>(json);
            }
        }

        private static HttpContent CrearContenido(object cuerpo)
        {
            var serializador = new DataContractJsonSerializer(cuerpo.GetType());
            using (var memoria = new MemoryStream())
            {
                serializador.WriteObject(memoria, cuerpo);
                var json = Encoding.UTF8.GetString(memoria.ToArray());
                return new StringContent(json, Encoding.UTF8, "application/json");
            }
        }

        private static T Deserializar<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }

            var serializador = new DataContractJsonSerializer(typeof(T));
            using (var memoria = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (T)serializador.ReadObject(memoria);
            }
        }

        private static async Task<ApiException> CrearExcepcionAsync(HttpResponseMessage respuesta)
        {
            var mensaje = "No se pudo completar la solicitud al servidor.";

            try
            {
                var json = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);
                var error = Deserializar<RespuestaApi>(json);
                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    mensaje = error.Mensaje;
                }
            }
            catch (Exception)
            {
                // Si el cuerpo no es JSON, se conserva el mensaje generico.
            }

            if (respuesta.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                mensaje = "El API de facturacion no esta disponible. Verifique que el servidor este en ejecucion.";
            }

            return new ApiException((int)respuesta.StatusCode, mensaje);
        }
    }
}
