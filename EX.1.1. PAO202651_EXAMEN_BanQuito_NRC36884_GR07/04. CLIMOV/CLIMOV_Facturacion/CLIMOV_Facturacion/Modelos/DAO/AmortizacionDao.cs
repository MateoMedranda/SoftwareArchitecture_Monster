using CLIMOV_Facturacion.Modelos.DTO;

namespace CLIMOV_Facturacion.Modelos.DAO;

/// <summary>Acceso HTTP al proxy de amortizaciones del API de facturación.</summary>
public sealed class AmortizacionDao(HttpClient http) : ApiDaoBase(http)
{
    public Task<AmortizacionDTO> PorFacturaAsync(int numFactura, CancellationToken ct = default) =>
        GetAsync<AmortizacionDTO>($"api/amortizaciones/{numFactura}", ct);

    public Task<AmortizacionDTO> PorCedulaAsync(string cedula, CancellationToken ct = default) =>
        GetAsync<AmortizacionDTO>($"api/amortizaciones/por-cedula/{Uri.EscapeDataString(cedula)}", ct);
}
