namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Respuesta JSON estandar para errores.
/// </summary>
public sealed class RespuestaErrorDTO
{
    public string Mensaje { get; set; } = string.Empty;

    public string? Detalle { get; set; }
}
