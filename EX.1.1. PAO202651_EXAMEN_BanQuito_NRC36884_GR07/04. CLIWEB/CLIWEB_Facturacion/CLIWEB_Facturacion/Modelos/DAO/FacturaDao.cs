using CLIWEB_Facturacion.Modelos.DTO;

namespace CLIWEB_Facturacion.Modelos.DAO;

/// <summary>Acceso HTTP a los endpoints de facturas.</summary>
public sealed class FacturaDao(HttpClient http) : ApiDaoBase(http)
{
    public Task<List<FacturaDTO>> ListarAsync(CancellationToken ct = default) =>
        GetAsync<List<FacturaDTO>>("api/facturas", ct);

    public Task<List<FacturaDTO>> ListarPorCedulaAsync(string cedula, CancellationToken ct = default) =>
        GetAsync<List<FacturaDTO>>($"api/facturas?cedula={Uri.EscapeDataString(cedula)}", ct);

    public Task<FacturaDTO> PorNumeroAsync(int numero, CancellationToken ct = default) =>
        GetAsync<FacturaDTO>($"api/facturas/{numero}", ct);

    public Task<List<FacturaItemDTO>> ItemsAsync(int numero, CancellationToken ct = default) =>
        GetAsync<List<FacturaItemDTO>>($"api/facturas/{numero}/items", ct);

    public Task<FacturaDTO> CrearAsync(SolicitudFacturaDTO solicitud, CancellationToken ct = default) =>
        PostAsync<FacturaDTO>("api/facturas", solicitud, ct);
}
