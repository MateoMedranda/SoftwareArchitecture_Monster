namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Respuesta JSON del servicio de sujeto de credito.
/// </summary>
public sealed class RespuestaSujetoCreditoDTO
{
    public bool SujetoCredito { get; set; }

    public int? CodCliente { get; set; }

    public string? Motivo { get; set; }
}
