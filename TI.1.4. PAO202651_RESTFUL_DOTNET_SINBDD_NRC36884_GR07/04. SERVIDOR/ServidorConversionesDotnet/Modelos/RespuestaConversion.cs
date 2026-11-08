namespace ServidorConversionesDotnet.Modelos;

/// <summary>
/// Respuesta exitosa de un endpoint de conversión.
/// </summary>
public sealed class RespuestaConversion
{
    /// <summary>Resultado de la conversión.</summary>
    public double ValorConvertido { get; set; }

    /// <summary>Unidad de origen recibida en la solicitud.</summary>
    public string UnidadOrigen { get; set; } = string.Empty;

    /// <summary>Unidad de destino recibida en la solicitud.</summary>
    public string UnidadDestino { get; set; } = string.Empty;
}
