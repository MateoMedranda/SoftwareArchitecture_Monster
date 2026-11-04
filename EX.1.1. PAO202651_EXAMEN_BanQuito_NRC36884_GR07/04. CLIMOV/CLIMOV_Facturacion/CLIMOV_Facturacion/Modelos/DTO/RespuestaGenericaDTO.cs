namespace CLIMOV_Facturacion.Modelos.DTO;

/// <summary>Respuesta genérica de operaciones de escritura.</summary>
public sealed class RespuestaGenericaDTO
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}
