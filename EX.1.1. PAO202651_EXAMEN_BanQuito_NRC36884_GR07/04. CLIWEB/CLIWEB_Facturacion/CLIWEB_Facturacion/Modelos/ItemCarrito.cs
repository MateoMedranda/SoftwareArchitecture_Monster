using CLIWEB_Facturacion.Modelos.DTO;

namespace CLIWEB_Facturacion.Modelos;

/// <summary>Ítem del carrito temporal de una compra.</summary>
public sealed class ItemCarrito
{
    public ProductoDTO Producto { get; init; } = new();
    public int Cantidad { get; set; }
    public decimal Subtotal => Math.Round(Producto.Precio * Cantidad, 2, MidpointRounding.AwayFromZero);
}
