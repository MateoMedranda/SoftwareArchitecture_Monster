namespace SISTEMA_FACTURACION.Modelos;

public sealed class Factura
{
    public int NumFactura { get; set; }
    public int CodCliente { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoPago { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public Cliente? Cliente { get; set; }
    public List<ItemFactura> Items { get; set; } = new();
}
