namespace SISTEMA_FACTURACION.Modelos;

public sealed class ItemFactura
{
    public int NumItem { get; set; }
    public int NumFactura { get; set; }
    public int NumInventario { get; set; }
    public decimal Valor { get; set; }
    public int Cantidad { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
}
