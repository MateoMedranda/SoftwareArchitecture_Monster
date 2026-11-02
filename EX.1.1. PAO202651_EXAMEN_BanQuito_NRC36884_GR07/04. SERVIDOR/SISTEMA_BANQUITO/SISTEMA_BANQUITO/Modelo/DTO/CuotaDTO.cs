namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Una cuota de la tabla de amortizacion.
/// </summary>
public sealed class CuotaDTO
{
    public int NumCuota { get; set; }

    public decimal ValorCuota { get; set; }

    public decimal InteresPagado { get; set; }

    public decimal CapitalPagado { get; set; }

    public decimal Saldo { get; set; }

    public DateTime FechaPago { get; set; }

    public bool Pagada { get; set; }
}
