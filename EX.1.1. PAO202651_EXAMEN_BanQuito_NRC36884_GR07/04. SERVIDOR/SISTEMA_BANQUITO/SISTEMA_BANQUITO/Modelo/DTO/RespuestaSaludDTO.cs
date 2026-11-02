namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Respuesta JSON del endpoint de diagnostico de conexion.
/// </summary>
public sealed class RespuestaSaludDTO
{
    public bool Conectado { get; set; }

    public string BaseDatos { get; set; } = string.Empty;

    public string? Detalle { get; set; }
}
