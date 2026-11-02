namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Respuesta JSON al otorgar un credito, incluye la tabla de amortizacion.
/// </summary>
public sealed class RespuestaOtorgarCreditoDTO
{
    public int IdCredito { get; set; }

    public bool Aprobado { get; set; }

    public string? Motivo { get; set; }

    public List<CuotaDTO> TablaAmortizacion { get; set; } = new();
}
