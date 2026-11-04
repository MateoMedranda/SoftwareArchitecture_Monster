namespace CLIMOV_Facturacion.Modelos.DTO;

/// <summary>Cliente devuelto por el API de facturación.</summary>
public sealed class ClienteDTO
{
    public int CodCliente { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

/// <summary>Datos para crear o actualizar un cliente.</summary>
public sealed class SolicitudClienteDTO
{
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}
