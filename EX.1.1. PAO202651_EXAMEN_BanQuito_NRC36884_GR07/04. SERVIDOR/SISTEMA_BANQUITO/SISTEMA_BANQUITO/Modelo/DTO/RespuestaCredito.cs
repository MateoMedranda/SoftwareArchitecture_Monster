namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Respuesta con los datos de un credito. El mapeo inverso lo realiza
/// <see cref="Mapeo.MapeoCredito"/> a partir de la entidad Credito.
/// </summary>
public sealed class RespuestaCredito
{
    public int NumCredito { get; set; }

    public string NumCuenta { get; set; } = string.Empty;

    public decimal CreditoTotal { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }
}
