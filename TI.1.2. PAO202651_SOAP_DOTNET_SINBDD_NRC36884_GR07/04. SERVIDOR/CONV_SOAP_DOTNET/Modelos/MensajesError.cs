using System;
using System.Globalization;

namespace CONV_SOAP_DOTNET.Modelos
{
    /// <summary>
    /// Mensajes de error y aviso propios del dominio de conversión de unidades.
    /// Se centralizan aquí para que cliente y servidor hablen el mismo idioma.
    /// </summary>
    public static class MensajesError
    {
        // Inicio de sesión.
        public const string SolicitudInicioSesionObligatoria =
            "La solicitud de inicio de sesión es obligatoria.";
        public const string CredencialesInvalidas =
            "Credenciales inválidas. Verifica tu usuario y contraseña.";
        public const string InicioSesionExitoso =
            "Inicio de sesión exitoso";

        // Conversión.
        public const string SolicitudConversionObligatoria =
            "La solicitud de conversión es obligatoria.";
        public const string TokenInvalido =
            "Token inválido o ausente. Inicie sesión nuevamente.";
        public const string ConversionExitosa =
            "Conversión exitosa.";

        // Categoría.
        public const string CategoriaInvalida =
            "La categoría '{0}' no es válida. Las categorías permitidas son: Temperatura, Longitud y Masa.";

        // Unidades.
        public const string UnidadInvalidaCategoria =
            "La unidad '{0}' no es válida para la categoría '{1}'.";
        public const string UnidadOrigenInvalida =
            "La unidad de origen '{0}' no es válida para la categoría '{1}'.";
        public const string UnidadDestinoInvalida =
            "La unidad de destino '{0}' no es válida para la categoría '{1}'.";

        // Rangos.
        public const string ValorNoNumerico =
            "El valor '{0}' no es un número válido.";
        public const string ValorFueraDeRango =
            "El valor {0} está fuera del rango permitido para {1} ({2} a {3}).";
        public const string ValorDemasiadoAlto =
            "El valor {0} es demasiado alto para {1}. El máximo permitido es {2}.";

        // Errores técnicos.
        public const string ErrorConversion =
            "Ocurrió un error inesperado durante la conversión. Intenta nuevamente.";
        public const string ErrorConexion =
            "No fue posible conectar con el servicio de conversión. Verifica tu conexión e inténtalo de nuevo.";
        public const string ServicioNoDisponible =
            "El servicio tardó demasiado en responder. Verifica que esté iniciado e inténtalo nuevamente.";

        public static string Formatear(string plantilla, params object[] argumentos)
        {
            // Cultura invariante para que los números se muestren igual
            // sin importar la configuración regional del servidor.
            return string.Format(CultureInfo.InvariantCulture, plantilla, argumentos);
        }
    }
}
