using System.ComponentModel.DataAnnotations;

namespace SISTEMA_FACTURACION.Modelos.DTO;

public sealed class SolicitudItemFacturaDTO
{
    [Range(1, int.MaxValue)]
    public int NumInventario { get; set; }
    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }
}
