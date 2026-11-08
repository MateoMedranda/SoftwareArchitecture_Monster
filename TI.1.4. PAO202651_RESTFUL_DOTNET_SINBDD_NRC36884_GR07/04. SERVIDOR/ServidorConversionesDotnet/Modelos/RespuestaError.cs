namespace ServidorConversionesDotnet.Modelos;

/// <summary>
/// Formato estándar de todas las respuestas de error del servidor.
/// </summary>
public sealed class RespuestaError
{
    /// <summary>Código HTTP asociado al error.</summary>
    public int Codigo { get; set; }

    /// <summary>Mensaje legible para mostrar directamente al cliente.</summary>
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>Detalle técnico ampliado del error.</summary>
    public string Detalle { get; set; } = string.Empty;
}
