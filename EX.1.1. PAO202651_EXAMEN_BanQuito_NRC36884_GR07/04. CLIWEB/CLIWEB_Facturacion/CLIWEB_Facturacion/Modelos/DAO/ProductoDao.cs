using CLIWEB_Facturacion.Modelos.DTO;

namespace CLIWEB_Facturacion.Modelos.DAO;

/// <summary>Acceso HTTP a los endpoints del catálogo de productos.</summary>
public sealed class ProductoDao(HttpClient http) : ApiDaoBase(http)
{
    public Task<List<ProductoDTO>> ListarAsync(CancellationToken ct = default) =>
        GetAsync<List<ProductoDTO>>("api/productos", ct);

    public Task<ProductoDTO> PorCodigoAsync(int codigo, CancellationToken ct = default) =>
        GetAsync<ProductoDTO>($"api/productos/{codigo}", ct);

    public Task<RespuestaGenericaDTO> CrearAsync(SolicitudProductoDTO solicitud, CancellationToken ct = default) =>
        PostAsync<RespuestaGenericaDTO>("api/productos", solicitud, ct);

    public Task<RespuestaGenericaDTO> ActualizarAsync(int codigo, SolicitudProductoDTO solicitud, CancellationToken ct = default) =>
        PutAsync<RespuestaGenericaDTO>($"api/productos/{codigo}", solicitud, ct);

    public Task<RespuestaGenericaDTO> EliminarAsync(int codigo, CancellationToken ct = default) =>
        DeleteAsync<RespuestaGenericaDTO>($"api/productos/{codigo}", ct);
}
