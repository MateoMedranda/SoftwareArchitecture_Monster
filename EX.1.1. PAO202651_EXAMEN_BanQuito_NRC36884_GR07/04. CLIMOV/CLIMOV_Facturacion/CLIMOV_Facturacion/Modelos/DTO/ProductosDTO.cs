namespace CLIMOV_Facturacion.Modelos.DTO;

/// <summary>Producto del catálogo devuelto por el API de facturación.</summary>
public sealed class ProductoDTO
{
    public int NumInventario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}

/// <summary>Datos para crear o actualizar un producto.</summary>
public sealed class SolicitudProductoDTO
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}
