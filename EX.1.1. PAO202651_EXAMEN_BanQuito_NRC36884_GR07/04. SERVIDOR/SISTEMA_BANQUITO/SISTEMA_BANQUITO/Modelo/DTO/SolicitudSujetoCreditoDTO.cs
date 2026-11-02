using System.ComponentModel.DataAnnotations;

namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Datos de entrada para verificar si un cliente es sujeto de credito.
/// </summary>
public sealed class SolicitudSujetoCreditoDTO
{
    [Required(ErrorMessage = "La cedula es obligatoria.")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "La cedula debe tener 10 digitos.")]
    public string Cedula { get; set; } = string.Empty;
}
