using CLIMOV_Facturacion.Modelos.DAO;
using CLIMOV_Facturacion.Modelos.DTO;

namespace CLIMOV_Facturacion.Servicios;

/// <summary>Reglas del cliente para el mantenimiento del catálogo.</summary>
public sealed class CatalogoServicio(ProductoDao dao)
{
    public Task<List<ProductoDTO>> ListarAsync(CancellationToken ct = default) =>
        dao.ListarAsync(ct);

    public Task<ProductoDTO> PorCodigoAsync(int codigo, CancellationToken ct = default) =>
        dao.PorCodigoAsync(codigo, ct);

    public Task<RespuestaGenericaDTO> CrearAsync(string nombre, decimal precio, CancellationToken ct = default)
    {
        nombre = NormalizarNombre(nombre);
        Validar(nombre, precio);

        return dao.CrearAsync(new SolicitudProductoDTO { Nombre = nombre, Precio = precio }, ct);
    }

    public Task<RespuestaGenericaDTO> ActualizarAsync(int codigo, string nombre, decimal precio, CancellationToken ct = default)
    {
        nombre = NormalizarNombre(nombre);
        Validar(nombre, precio);

        return dao.ActualizarAsync(codigo, new SolicitudProductoDTO { Nombre = nombre, Precio = precio }, ct);
    }

    public Task<RespuestaGenericaDTO> EliminarAsync(int codigo, CancellationToken ct = default) =>
        dao.EliminarAsync(codigo, ct);

    private static string NormalizarNombre(string? nombre) => (nombre ?? string.Empty).Trim();

    private static void Validar(string nombre, decimal precio)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ApiException(400, "El nombre del producto es obligatorio.");
        }

        if (nombre.Length > 100)
        {
            throw new ApiException(400, "El nombre del producto no debe superar los 100 caracteres.");
        }

        if (precio <= 0)
        {
            throw new ApiException(400, "El precio del producto debe ser mayor que cero.");
        }
    }
}
