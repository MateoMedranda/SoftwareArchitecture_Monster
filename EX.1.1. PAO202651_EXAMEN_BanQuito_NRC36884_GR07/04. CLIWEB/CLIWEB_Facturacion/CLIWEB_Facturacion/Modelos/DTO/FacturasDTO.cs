namespace CLIWEB_Facturacion.Modelos.DTO;

/// <summary>Factura devuelta por el API de facturación.</summary>
public sealed class FacturaDTO
{
    public int NumFactura { get; set; }
    public int CodCliente { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string TipoPago { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int? NumCredito { get; set; }
    public List<FacturaItemDTO> Items { get; set; } = new();
}

/// <summary>Ítem o línea de una factura.</summary>
public sealed class FacturaItemDTO
{
    public int NumItem { get; set; }
    public int NumInventario { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int Cantidad { get; set; }
}

/// <summary>Datos para registrar una factura.</summary>
public sealed class SolicitudFacturaDTO
{
    public int CodCliente { get; set; }
    public string TipoPago { get; set; } = string.Empty;
    public int? NumCuotas { get; set; }
    public List<SolicitudItemFacturaDTO> Items { get; set; } = new();
}

/// <summary>Ítem enviado al registrar una factura.</summary>
public sealed class SolicitudItemFacturaDTO
{
    public int NumInventario { get; set; }
    public int Cantidad { get; set; }
}
