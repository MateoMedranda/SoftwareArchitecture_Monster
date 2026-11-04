using CLIWEB_Facturacion.Modelos.DTO;

namespace CLIWEB_Facturacion.Modelos.DAO;

/// <summary>Acceso HTTP a los endpoints de clientes.</summary>
public sealed class ClienteDao(HttpClient http) : ApiDaoBase(http)
{
    public Task<List<ClienteDTO>> ListarAsync(CancellationToken ct = default) =>
        GetAsync<List<ClienteDTO>>("api/clientes", ct);

    public Task<ClienteDTO> PorCodigoAsync(int codigo, CancellationToken ct = default) =>
        GetAsync<ClienteDTO>($"api/clientes/{codigo}", ct);

    public Task<ClienteDTO> PorCedulaAsync(string cedula, CancellationToken ct = default) =>
        GetAsync<ClienteDTO>($"api/clientes/por-cedula/{Uri.EscapeDataString(cedula)}", ct);

    public Task<RespuestaGenericaDTO> CrearAsync(SolicitudClienteDTO solicitud, CancellationToken ct = default) =>
        PostAsync<RespuestaGenericaDTO>("api/clientes", solicitud, ct);

    public Task<RespuestaGenericaDTO> ActualizarAsync(int codigo, SolicitudClienteDTO solicitud, CancellationToken ct = default) =>
        PutAsync<RespuestaGenericaDTO>($"api/clientes/{codigo}", solicitud, ct);

    public Task<RespuestaGenericaDTO> EliminarAsync(int codigo, CancellationToken ct = default) =>
        DeleteAsync<RespuestaGenericaDTO>($"api/clientes/{codigo}", ct);
}
