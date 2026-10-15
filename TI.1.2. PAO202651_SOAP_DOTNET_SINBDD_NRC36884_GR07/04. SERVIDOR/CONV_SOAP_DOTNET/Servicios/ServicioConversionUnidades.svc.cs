using System;
using System.Configuration;
using System.ServiceModel.Activation;
using CONV_SOAP_DOTNET.Contratos;
using CONV_SOAP_DOTNET.Servicios.Conversores;

namespace CONV_SOAP_DOTNET.Servicios
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ServicioConversionUnidades : IServicioConversionUnidades
    {
        private const string UsuarioPorDefecto = "MONSTER";
        private const string ContrasenaPorDefecto = "MONSTER9";

        private readonly IAlmacenTokens almacenTokens = AlmacenTokens.Instancia;
        private readonly RegistroConversores registro = RegistroConversores.Actual;

        public RespuestaInicioSesion IniciarSesion(SolicitudInicioSesion solicitud)
        {
            if (solicitud == null)
            {
                return new RespuestaInicioSesion
                {
                    Exito = false,
                    Token = null,
                    Mensaje = "La solicitud de inicio de sesión es obligatoria."
                };
            }

            string usuario = ConfigurationManager.AppSettings["UsuarioDemo"] ?? UsuarioPorDefecto;
            string contrasena = ConfigurationManager.AppSettings["ContrasenaDemo"] ?? ContrasenaPorDefecto;

            bool credencialesValidas =
                string.Equals(solicitud.NombreUsuario, usuario, StringComparison.Ordinal) &&
                string.Equals(solicitud.Contrasena, contrasena, StringComparison.Ordinal);

            if (!credencialesValidas)
            {
                return new RespuestaInicioSesion
                {
                    Exito = false,
                    Token = null,
                    Mensaje = "Credenciales inválidas"
                };
            }

            string token = almacenTokens.GenerarToken(solicitud.NombreUsuario);
            return new RespuestaInicioSesion
            {
                Exito = true,
                Token = token,
                Mensaje = "Inicio de sesión exitoso"
            };
        }

        public RespuestaConversion Convertir(SolicitudConversion solicitud)
        {
            if (solicitud == null)
            {
                return Error("La solicitud de conversión es obligatoria.");
            }

            if (!almacenTokens.ValidarToken(solicitud.Token))
            {
                return Error("Token inválido o ausente. Inicie sesión nuevamente.");
            }

            try
            {
                IConversorUnidad conversor = registro.Obtener(solicitud.Categoria);

                if (!conversor.EsUnidadValida(solicitud.UnidadOrigen))
                {
                    return Error(string.Format(
                        "La unidad de origen '{0}' no es válida para la categoría '{1}'.",
                        solicitud.UnidadOrigen,
                        solicitud.Categoria));
                }

                if (!conversor.EsUnidadValida(solicitud.UnidadDestino))
                {
                    return Error(string.Format(
                        "La unidad de destino '{0}' no es válida para la categoría '{1}'.",
                        solicitud.UnidadDestino,
                        solicitud.Categoria));
                }

                conversor.ValidarRango(solicitud.UnidadOrigen, solicitud.Valor);

                double valorConvertido = conversor.Convertir(
                    solicitud.UnidadOrigen,
                    solicitud.UnidadDestino,
                    solicitud.Valor);

                return new RespuestaConversion
                {
                    Exito = true,
                    ValorConvertido = valorConvertido,
                    Mensaje = "Conversión exitosa"
                };
            }
            catch (NotSupportedException excepcion)
            {
                return Error(excepcion.Message);
            }
            catch (ArgumentOutOfRangeException excepcion)
            {
                return Error(excepcion.Message);
            }
            catch (ArgumentException excepcion)
            {
                return Error(excepcion.Message);
            }
        }

        private static RespuestaConversion Error(string mensaje)
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
