using System.ComponentModel.DataAnnotations;

namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Solicitud generica para buscar un registro por un valor de texto.
/// Sirve para cedula de cliente o numero de cuenta.
/// </summary>
public sealed class SolicitudBusquedaPorString
{
    [Required(ErrorMessage = "El valor de busqueda es obligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El valor de busqueda debe tener entre 1 y 100 caracteres.")]
    public string Valor { get; set; } = string.Empty;
}
