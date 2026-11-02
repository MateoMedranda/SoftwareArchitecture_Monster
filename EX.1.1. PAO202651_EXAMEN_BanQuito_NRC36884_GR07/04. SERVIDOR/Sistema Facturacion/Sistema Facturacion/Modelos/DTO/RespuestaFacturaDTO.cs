namespace SISTEMA_FACTURACION.Modelos.DTO;

public sealed class RespuestaFacturaDTO
{
    public int NumFactura { get; set; }
    public int CodCliente { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string TipoPago { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public List<RespuestaItemFacturaDTO> Items { get; set; } = new();
}

public sealed class RespuestaItemFacturaDTO
{
    public int NumItem { get; set; }
    public int NumInventario { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int Cantidad { get; set; }
}
