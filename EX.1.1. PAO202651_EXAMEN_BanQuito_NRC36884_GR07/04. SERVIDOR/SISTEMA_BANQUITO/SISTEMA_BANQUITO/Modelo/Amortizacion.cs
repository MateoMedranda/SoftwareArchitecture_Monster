namespace SISTEMA_BANQUITO.Modelo;

/// <summary>
/// Representa una cuota de la tabla AMORTIZACION.
/// PAGADA indica si la cuota ya fue cancelada por el cliente.
/// </summary>
public sealed class Amortizacion
{
    public int NumCuota { get; set; }

    public int NumCredito { get; set; }

    public decimal ValorCuota { get; set; }

    public decimal InteresPagado { get; set; }

    public decimal CapitalPagado { get; set; }

    public decimal Saldo { get; set; }

    public DateTime FechaPago { get; set; }

    public bool Pagada { get; set; }
}
