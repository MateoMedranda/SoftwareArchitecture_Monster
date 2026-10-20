using ServidorConversionesDotnet.Modelos;
using ServidorConversionesDotnet.Modelos.Enumeraciones;
using ServidorConversionesDotnet.Utilidades;

namespace ServidorConversionesDotnet.Servicios;

/// <summary>
/// Aplica las reglas de validación y las fórmulas de conversión de temperatura
/// (unidad base: Kelvin). El valor recibido se convierte a Kelvin para
/// verificar el rango permitido de 0 °K a 373.15 °K.
/// </summary>
public sealed class ServicioConversionTemperatura
{
    private const double KelvinMinimo = 0;
    private const double KelvinMaximo = 373.15;

    /// <summary>
    /// Convierte el valor recibido de la unidad de origen a la unidad de destino.
    /// </summary>
    public ResultadoServicio<RespuestaConversion> Convertir(SolicitudConversion solicitud)
    {
        if (solicitud.Valor is null ||
            string.IsNullOrWhiteSpace(solicitud.UnidadOrigen) ||
            string.IsNullOrWhiteSpace(solicitud.UnidadDestino))
        {
            return ResultadoServicio<RespuestaConversion>.ConError(new RespuestaError
            {
                Codigo = 400,
                Mensaje = "Debe ingresar el valor, la unidad de origen y la unidad de destino.",
                Detalle = "Los campos 'valor', 'unidadOrigen' y 'unidadDestino' son obligatorios."
            });
        }

        if (!ConversorEnum.IntentarConvertir(solicitud.UnidadOrigen, out UnidadTemperatura origen) ||
            !ConversorEnum.IntentarConvertir(solicitud.UnidadDestino, out UnidadTemperatura destino))
        {
            return ResultadoServicio<RespuestaConversion>.ConError(new RespuestaError
            {
                Codigo = 400,
                Mensaje = "La unidad especificada no es válida.",
                Detalle = $"Las unidades '{solicitud.UnidadOrigen}' y '{solicitud.UnidadDestino}' " +
                          "no corresponden a unidades de temperatura admitidas."
            });
        }

        if (origen == destino)
        {
            return ResultadoServicio<RespuestaConversion>.ConError(new RespuestaError
            {
                Codigo = 400,
                Mensaje = "La unidad de origen y destino no pueden ser iguales.",
                Detalle = $"Ambas unidades son '{solicitud.UnidadOrigen}'."
            });
        }

        var kelvin = AKelvin(solicitud.Valor.Value, origen);

        if (kelvin < KelvinMinimo || kelvin > KelvinMaximo)
        {
            return ResultadoServicio<RespuestaConversion>.ConError(new RespuestaError
            {
                Codigo = 400,
                Mensaje = "El valor de temperatura debe estar entre 0 °K y 373.15 °K.",
                Detalle = $"El valor recibido equivale a {kelvin:0.##} K, fuera del rango permitido."
            });
        }

        try
        {
            var valorConvertido = DeKelvin(kelvin, destino);

            return ResultadoServicio<RespuestaConversion>.ConExito(new RespuestaConversion
            {
                ValorConvertido = valorConvertido,
                UnidadOrigen = solicitud.UnidadOrigen,
                UnidadDestino = solicitud.UnidadDestino
            });
        }
        catch (Exception excepcion)
        {
            return ResultadoServicio<RespuestaConversion>.ConError(new RespuestaError
            {
                Codigo = 500,
                Mensaje = "No fue posible resolver la conversión solicitada. Intente nuevamente más tarde.",
                Detalle = excepcion.Message
            });
        }
    }

    /// <summary>Convierte cualquier escala a Kelvin.</summary>
    private static double AKelvin(double valor, UnidadTemperatura unidad) => unidad switch
    {
        UnidadTemperatura.Celsius => valor + 273.15,
        UnidadTemperatura.Fahrenheit => (valor - 32) * 5 / 9 + 273.15,
        UnidadTemperatura.Kelvin => valor,
        UnidadTemperatura.Rankine => valor * 5 / 9,
        _ => throw new ArgumentOutOfRangeException(nameof(unidad), unidad, "Escala de temperatura desconocida.")
    };

    /// <summary>Convierte de Kelvin a la escala indicada.</summary>
    private static double DeKelvin(double kelvin, UnidadTemperatura unidad) => unidad switch
    {
        UnidadTemperatura.Celsius => kelvin - 273.15,
        UnidadTemperatura.Fahrenheit => (kelvin - 273.15) * 9 / 5 + 32,
        UnidadTemperatura.Kelvin => kelvin,
        UnidadTemperatura.Rankine => kelvin * 9 / 5,
        _ => throw new ArgumentOutOfRangeException(nameof(unidad), unidad, "Escala de temperatura desconocida.")
    };
}
