using System.Globalization;
using System.ServiceModel;
using ClienteConsolaConversion.Servicio;

namespace ClienteConsolaConversion.Cliente
{
    /// <summary>Categorías de conversión publicadas por el servidor.</summary>
    public enum CategoriaUnidad
    {
        Temperatura,
        Longitud,
        Masa
    }

    /// <summary>
    /// Wrapper del cliente WCF generado con dotnet-svcutil desde el WSDL del servidor.
    /// Traduce las excepciones de red a respuestas con Exito = false y un mensaje amigable,
    /// pero nunca altera los mensajes de negocio: los propaga tal cual.
    /// </summary>
    public class ClienteSoapConversion
    {
        public const string MensajeConexion =
            "No se pudo conectar con el servidor";

        public const string MensajeTiempoAgotado =
            "El servicio tardó demasiado en responder. Verifica que esté iniciado e inténtalo de nuevo.";

        public const string MensajeRespuestaVacia =
            "El servidor no devolvió una respuesta válida.";

        private readonly string _urlEndpoint;

        public ClienteSoapConversion(string urlEndpoint)
        {
            if (string.IsNullOrWhiteSpace(urlEndpoint))
            {
                throw new ArgumentException("La URL del endpoint es obligatoria.", nameof(urlEndpoint));
            }

            _urlEndpoint = urlEndpoint;
        }

        public string UrlEndpoint => _urlEndpoint;

        public async Task<RespuestaInicioSesion> IniciarSesionAsync(string usuario, string contrasena)
        {
            ServicioConversionUnidadesClient? cliente = CrearCliente();

            try
            {
                var solicitud = new SolicitudInicioSesion
                {
                    NombreUsuario = usuario,
                    Contrasena = contrasena
                };

                return await cliente.IniciarSesionAsync(solicitud);
            }
            catch (CommunicationException)
            {
                return NuevaRespuestaInicioSesion(MensajeConexion);
            }
            catch (TimeoutException)
            {
                return NuevaRespuestaInicioSesion(MensajeTiempoAgotado);
            }
            finally
            {
                Finalizar(cliente);
            }
        }

        public async Task<RespuestaConversion> ConvertirAsync(
            string token,
            CategoriaUnidad categoria,
            string unidadOrigen,
            string unidadDestino,
            double valor)
        {
            // El valor viaja como texto en cultura invariante: el servidor
            // también parsea con CultureInfo.InvariantCulture.
            var solicitud = new SolicitudConversion
            {
                Categoria = categoria.ToString(),
                UnidadOrigen = unidadOrigen,
                UnidadDestino = unidadDestino,
                Valor = valor.ToString(CultureInfo.InvariantCulture)
            };

            ServicioConversionUnidadesClient? cliente = CrearCliente();

            try
            {
                RespuestaConversionMensaje respuesta = await cliente.ConvertirAsync(token, solicitud);

                if (respuesta?.ConvertirResult is null)
                {
                    return NuevaRespuestaConversion(MensajeRespuestaVacia);
                }

                return respuesta.ConvertirResult;
            }
            catch (CommunicationException)
            {
                return NuevaRespuestaConversion(MensajeConexion);
            }
            catch (TimeoutException)
            {
                return NuevaRespuestaConversion(MensajeTiempoAgotado);
            }
            finally
            {
                Finalizar(cliente);
            }
        }

        private ServicioConversionUnidadesClient CrearCliente()
        {
            return new ServicioConversionUnidadesClient(
                ServicioConversionUnidadesClient.EndpointConfiguration.BasicHttpBinding_IServicioConversionUnidades,
                _urlEndpoint);
        }

        private static void Finalizar(ServicioConversionUnidadesClient? cliente)
        {
            if (cliente is null)
            {
                return;
            }

            try
            {
                if (cliente.State == CommunicationState.Faulted)
                {
                    cliente.Abort();
                }
                else
                {
                    cliente.Close();
                }
            }
            catch (CommunicationException)
            {
                cliente.Abort();
            }
            catch (TimeoutException)
            {
                cliente.Abort();
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
    }
}
