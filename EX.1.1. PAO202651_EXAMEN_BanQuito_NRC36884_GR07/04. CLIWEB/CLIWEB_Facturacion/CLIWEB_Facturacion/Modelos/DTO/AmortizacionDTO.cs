namespace CLIWEB_Facturacion.Modelos.DTO;

/// <summary>Tabla de amortización devuelta por el API de facturación.</summary>
public sealed class AmortizacionDTO
{
    public int NumCredito { get; set; }
    public List<CuotaDTO> Cuotas { get; set; } = new();
}

/// <summary>Una cuota de la tabla de amortización.</summary>
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
