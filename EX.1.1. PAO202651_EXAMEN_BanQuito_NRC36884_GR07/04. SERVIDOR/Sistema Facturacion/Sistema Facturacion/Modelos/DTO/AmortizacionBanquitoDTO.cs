namespace SISTEMA_FACTURACION.Modelos.DTO;

// Contratos espejo de los endpoints de BanQuito usados por el proxy de amortizacion.
public sealed class SolicitudTextoBanquitoDTO { public string Valor { get; set; } = string.Empty; }
public sealed class SolicitudNumeroBanquitoDTO { public int Valor { get; set; } }

public sealed class RespuestaClienteBanquitoDTO
{
    public int CodCliente { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

public sealed class RespuestaAmortizacionBanquitoDTO
{
    public int NumCredito { get; set; }
    public List<CuotaBanquitoDTO> Cuotas { get; set; } = new();
}

public sealed class CuotaBanquitoDTO
{
    public int NumCuota { get; set; }
    public decimal ValorCuota { get; set; }
    public decimal InteresPagado { get; set; }
    public decimal CapitalPagado { get; set; }
    public decimal Saldo { get; set; }
    public DateTime FechaPago { get; set; }
    public bool Pagada { get; set; }
}
