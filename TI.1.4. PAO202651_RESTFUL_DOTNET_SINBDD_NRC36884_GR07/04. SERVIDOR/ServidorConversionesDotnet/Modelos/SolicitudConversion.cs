namespace ServidorConversionesDotnet.Modelos;

/// <summary>
/// Cuerpo de la solicitud para los endpoints de conversión.
/// </summary>
public sealed class SolicitudConversion
{
    /// <summary>Valor numérico a convertir.</summary>
    public double? Valor { get; set; }

    /// <summary>Unidad de origen (nombre del enum, sin distinción de mayúsculas/minúsculas).</summary>
    public string? UnidadOrigen { get; set; }

    /// <summary>Unidad de destino (nombre del enum, sin distinción de mayúsculas/minúsculas).</summary>
    public string? UnidadDestino { get; set; }
}
