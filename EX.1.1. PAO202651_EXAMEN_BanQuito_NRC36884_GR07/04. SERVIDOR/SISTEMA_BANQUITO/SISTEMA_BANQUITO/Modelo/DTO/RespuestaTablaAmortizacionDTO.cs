namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Respuesta JSON con las cuotas de un credito.
/// </summary>
public sealed class RespuestaTablaAmortizacionDTO
{
    public int NumCredito { get; set; }

    public List<CuotaDTO> Cuotas { get; set; } = new();
}
