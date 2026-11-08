using ServidorConversionesDotnet.Modelos;
using ServidorConversionesDotnet.Modelos.Enumeraciones;
using ServidorConversionesDotnet.Utilidades;

namespace ServidorConversionesDotnet.Servicios;

/// <summary>
/// Aplica las reglas de validación y las fórmulas de conversión de longitud
/// (unidad base: metro).
/// </summary>
public sealed class ServicioConversionLongitud
{
    private static readonly Dictionary<UnidadLongitud, double> FactorAMetro = new()
    {
        [UnidadLongitud.Pie] = 0.3048,
        [UnidadLongitud.Centimetro] = 0.01,
        [UnidadLongitud.Metro] = 1,
        [UnidadLongitud.Milla] = 1609.34,
        [UnidadLongitud.Yarda] = 0.9144
    };

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

        if (!ConversorEnum.IntentarConvertir(solicitud.UnidadOrigen, out UnidadLongitud origen) ||
            !ConversorEnum.IntentarConvertir(solicitud.UnidadDestino, out UnidadLongitud destino))
        {
            return ResultadoServicio<RespuestaConversion>.ConError(new RespuestaError
            {
                Codigo = 400,
                Mensaje = "La unidad especificada no es válida.",
                Detalle = $"Las unidades '{solicitud.UnidadOrigen}' y '{solicitud.UnidadDestino}' " +
                          "no corresponden a unidades de longitud admitidas."
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

        if (solicitud.Valor < 0)
        {
            return ResultadoServicio<RespuestaConversion>.ConError(new RespuestaError
            {
                Codigo = 400,
                Mensaje = "El valor ingresado no puede ser negativo.",
                Detalle = "El campo 'valor' debe ser mayor o igual a 0."
            });
        }

        try
        {
            var valorConvertido = solicitud.Valor.Value *
                                  FactorAMetro[origen] /
                                  FactorAMetro[destino];

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
}
