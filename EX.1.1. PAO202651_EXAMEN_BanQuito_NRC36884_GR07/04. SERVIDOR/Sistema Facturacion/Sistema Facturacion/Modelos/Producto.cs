namespace SISTEMA_FACTURACION.Modelos;

public sealed class Producto
{
    public int NumInventario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}
