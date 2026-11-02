using System.ComponentModel.DataAnnotations;

namespace SISTEMA_FACTURACION.Modelos.DTO;

public sealed class SolicitudFacturaDTO
{
    [Range(1, int.MaxValue)]
    public int CodCliente { get; set; }
    [Required]
    public string TipoPago { get; set; } = string.Empty;
    public int? NumCuotas { get; set; }
    [Required, MinLength(1)]
    public List<SolicitudItemFacturaDTO> Items { get; set; } = new();
}
