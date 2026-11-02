using System.ComponentModel.DataAnnotations;

namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Datos de entrada para crear o actualizar un cliente.
/// </summary>
public sealed class SolicitudCliente
{
    [Required(ErrorMessage = "La cedula es obligatoria.")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "La cedula debe tener 10 digitos.")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El genero es obligatorio.")]
    [RegularExpression("^[FM]$", ErrorMessage = "El genero debe ser F (femenino) o M (masculino).")]
    public string Genero { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateTime FechaNacimiento { get; set; }
}
