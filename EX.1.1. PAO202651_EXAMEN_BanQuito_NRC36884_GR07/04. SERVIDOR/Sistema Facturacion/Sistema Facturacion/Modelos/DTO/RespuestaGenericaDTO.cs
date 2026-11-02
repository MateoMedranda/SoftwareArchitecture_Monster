namespace SISTEMA_FACTURACION.Modelos.DTO;

public sealed class RespuestaGenericaDTO
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}
