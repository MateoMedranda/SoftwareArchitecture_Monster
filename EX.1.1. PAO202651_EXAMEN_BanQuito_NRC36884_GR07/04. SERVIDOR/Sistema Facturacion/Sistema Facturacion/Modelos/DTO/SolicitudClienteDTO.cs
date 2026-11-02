using System.ComponentModel.DataAnnotations;

namespace SISTEMA_FACTURACION.Modelos.DTO;

public sealed class SolicitudClienteDTO
{
    [Required, StringLength(10, MinimumLength = 10)]
    public string Cedula { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;
}
