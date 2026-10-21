using System;
using System.Configuration;
using System.Globalization;
using System.ServiceModel.Activation;
using CONV_SOAP_DOTNET.Contratos;
using CONV_SOAP_DOTNET.Modelos;
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
                    Mensaje = MensajesError.SolicitudInicioSesionObligatoria
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
                    Mensaje = MensajesError.CredencialesInvalidas
                };
            }

            string token = almacenTokens.GenerarToken(solicitud.NombreUsuario);
            return new RespuestaInicioSesion
            {
                Exito = true,
                Token = token,
                Mensaje = MensajesError.InicioSesionExitoso
            };
        }

        public RespuestaConversionMensaje Convertir(SolicitudConversionMensaje solicitudMensaje)
        {
            if (solicitudMensaje == null || solicitudMensaje.Conversion == null)
            {
                return Error(MensajesError.SolicitudConversionObligatoria);
            }

            if (!almacenTokens.ValidarToken(solicitudMensaje.Token))
            {
                return Error(MensajesError.TokenInvalido);
            }

            SolicitudConversion solicitud = solicitudMensaje.Conversion;

            CategoriaUnidad categoria;
            if (!CategoriasUnidad.TryResolver(solicitud.Categoria, out categoria))
            {
                return Error(MensajesError.Formatear(MensajesError.CategoriaInvalida, solicitud.Categoria));
            }

            IConversorUnidad conversor;
            try
            {
                conversor = registro.Obtener(categoria);
            }
            catch (ArgumentOutOfRangeException)
            {
                return Error(MensajesError.Formatear(MensajesError.CategoriaInvalida, solicitud.Categoria));
            }

            if (!conversor.EsUnidadValida(solicitud.UnidadOrigen))
            {
                return Error(MensajesError.Formatear(
                    MensajesError.UnidadOrigenInvalida,
                    solicitud.UnidadOrigen,
                    categoria));
            }

            if (!conversor.EsUnidadValida(solicitud.UnidadDestino))
            {
                return Error(MensajesError.Formatear(
                    MensajesError.UnidadDestinoInvalida,
                    solicitud.UnidadDestino,
                    categoria));
            }

            // El valor viaja como texto para poder dar mensajes propios
            // (por ejemplo "1e309" que excede el rango de double) en lugar de un Fault de WCF.
            double valor;
            if (!double.TryParse(
                    solicitud.Valor,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out valor))
            {
                // Si el texto tiene forma de número pero no cabe en double,
                // el problema es el tamaño, no el formato.
                if (EsNumeroSintactico(solicitud.Valor))
                {
                    return Error(MensajesError.Formatear(
                        MensajesError.ValorDemasiadoAlto,
                        solicitud.Valor,
                        categoria,
                        double.MaxValue));
                }

                return Error(MensajesError.Formatear(MensajesError.ValorNoNumerico, solicitud.Valor));
            }

            // Validación explícita de valores demasiado altos antes de convertir.
            if (double.IsInfinity(valor) || double.IsNaN(valor))
            {
                return Error(MensajesError.Formatear(
                    MensajesError.ValorDemasiadoAlto,
                    solicitud.Valor,
                    categoria,
                    double.MaxValue));
            }

            try
            {
                conversor.ValidarRango(solicitud.UnidadOrigen, valor);

                double valorConvertido = conversor.Convertir(
                    solicitud.UnidadOrigen,
                    solicitud.UnidadDestino,
                    valor);

                if (double.IsInfinity(valorConvertido) || double.IsNaN(valorConvertido))
                {
                    return Error(MensajesError.Formatear(
                        MensajesError.ValorDemasiadoAlto,
                        solicitud.Valor,
                        categoria,
                        double.MaxValue));
                }

                return new RespuestaConversionMensaje
                {
                    Resultado = new RespuestaConversion
                    {
                        Exito = true,
                        ValorConvertido = valorConvertido,
                        Mensaje = MensajesError.ConversionExitosa
                    }
                };
            }
            catch (ArgumentOutOfRangeException excepcion)
            {
                return Error(excepcion.Message);
            }
            catch (ArgumentException excepcion)
            {
                return Error(excepcion.Message);
            }
            catch (OverflowException)
            {
                return Error(MensajesError.Formatear(
                    MensajesError.ValorDemasiadoAlto,
                    solicitud.Valor,
                    categoria,
                    double.MaxValue));
            }
            catch (Exception)
            {
                return Error(MensajesError.ErrorConversion);
            }
        }

        /// <summary>
        /// Indica si el texto recibido tiene forma de número (signo, dígitos, punto y exponente),
        /// sin importar que su magnitud no quepa en un double.
        /// </summary>
        private static bool EsNumeroSintactico(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return false;
            }

            string texto = valor.Trim();
            bool digito = false;
            bool punto = false;
            bool exponente = false;

            for (int i = 0; i < texto.Length; i++)
            {
                char caracter = texto[i];

                if (caracter >= '0' && caracter <= '9')
                {
                    digito = true;
                    continue;
                }

                if ((caracter == '+' || caracter == '-') && i == 0)
                {
                    continue;
                }

                if (caracter == '.' && !punto && !exponente)
                {
                    punto = true;
                    continue;
                }

                if ((caracter == 'e' || caracter == 'E') && digito && !exponente)
                {
                    exponente = true;
                    // El exponente puede empezar con signo.
                    if (i + 1 < texto.Length && (texto[i + 1] == '+' || texto[i + 1] == '-'))
                    {
                        i++;
                    }

                    continue;
                }

                return false;
            }

            return digito;
        }

        private static RespuestaConversionMensaje Error(string mensaje)
        {
            return new RespuestaConversionMensaje
            {
                Resultado = new RespuestaConversion
                {
                    Exito = false,
                    ValorConvertido = 0.0,
                    Mensaje = mensaje
                }
            };
        }
    }
}
