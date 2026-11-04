using CLIWEB_Facturacion.Modelos;
using CLIWEB_Facturacion.Modelos.DAO;
using CLIWEB_Facturacion.Modelos.DTO;

namespace CLIWEB_Facturacion.Servicios;

/// <summary>
/// Lógica del proceso de facturación: carrito temporal, descuento de
/// efectivo, validaciones y creación de la factura en el API.
/// </summary>
public sealed class FacturacionServicio(FacturaDao facturaDao, AmortizacionDao amortizacionDao)
{
    private const decimal FactorEfectivo = 0.58m;

    public List<ItemCarrito> Carrito { get; } = new();

    public ClienteDTO? Cliente { get; private set; }

    public decimal Subtotal =>
        Math.Round(Carrito.Sum(item => item.Subtotal), 2, MidpointRounding.AwayFromZero);

    /// <summary>Vista previa del total con el 42 % de descuento por efectivo.</summary>
    public decimal TotalEfectivo =>
        Math.Round(Subtotal * FactorEfectivo, 2, MidpointRounding.AwayFromZero);

    public void SeleccionarCliente(ClienteDTO cliente) => Cliente = cliente;

    public void QuitarCliente() => Cliente = null;

    public void Agregar(ProductoDTO producto, int cantidad)
    {
        if (producto is null)
        {
            throw new ApiException(400, "Debe seleccionar un producto.");
        }

        if (cantidad <= 0)
        {
            throw new ApiException(400, "La cantidad debe ser mayor que cero.");
        }

        var existente = Carrito.FirstOrDefault(item => item.Producto.NumInventario == producto.NumInventario);
        if (existente is not null)
        {
            existente.Cantidad += cantidad;
            return;
        }

        Carrito.Add(new ItemCarrito { Producto = producto, Cantidad = cantidad });
    }

    public void Quitar(int numInventario) =>
        Carrito.RemoveAll(item => item.Producto.NumInventario == numInventario);

    public void Vaciar()
    {
        Carrito.Clear();
        QuitarCliente();
    }

    public async Task<FacturaDTO> CrearAsync(string tipoPago, int? numCuotas, CancellationToken ct = default)
    {
        if (Cliente is null)
        {
            throw new ApiException(400, "Debe buscar y seleccionar un cliente.");
        }

        if (Carrito.Count == 0)
        {
            throw new ApiException(400, "Agregue al menos un producto antes de facturar.");
        }

        var tipo = (tipoPago ?? string.Empty).Trim().ToUpperInvariant();
        if (tipo is not ("EFECTIVO" or "CREDITO"))
        {
            throw new ApiException(400, "El tipo de pago debe ser EFECTIVO o CREDITO.");
        }

        if (tipo == "CREDITO" && (numCuotas is null or < 3 or > 18))
        {
            throw new ApiException(400, "El plazo del crédito debe estar entre 3 y 18 meses.");
        }

        var solicitud = new SolicitudFacturaDTO
        {
            CodCliente = Cliente.CodCliente,
            TipoPago = tipo,
            NumCuotas = tipo == "CREDITO" ? numCuotas : null,
            Items = Carrito
                .Select(item => new SolicitudItemFacturaDTO
                {
                    NumInventario = item.Producto.NumInventario,
                    Cantidad = item.Cantidad
                })
                .ToList()
        };

        // Si el API falla, la excepción sube y el carrito se conserva.
        var factura = await facturaDao.CrearAsync(solicitud, ct);
        Vaciar();
        return factura;
    }

    public Task<AmortizacionDTO> ObtenerAmortizacionPorFacturaAsync(int numFactura, CancellationToken ct = default) =>
        amortizacionDao.PorFacturaAsync(numFactura, ct);

    public Task<AmortizacionDTO> ObtenerAmortizacionPorCedulaAsync(string cedula, CancellationToken ct = default) =>
        amortizacionDao.PorCedulaAsync((cedula ?? string.Empty).Trim(), ct);
}
