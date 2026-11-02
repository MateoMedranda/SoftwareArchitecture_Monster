using System.ComponentModel.DataAnnotations;

namespace SISTEMA_FACTURACION.Modelos.DTO;

public sealed class SolicitudProductoDTO
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;
    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal Precio { get; set; }
}
