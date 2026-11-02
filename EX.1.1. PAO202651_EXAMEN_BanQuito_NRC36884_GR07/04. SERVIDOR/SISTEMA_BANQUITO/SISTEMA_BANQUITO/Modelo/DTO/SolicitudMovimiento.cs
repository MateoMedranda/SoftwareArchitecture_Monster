using System.ComponentModel.DataAnnotations;
using SISTEMA_BANQUITO.Modelo;

namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Datos de entrada para registrar un movimiento (deposito o retiro).
/// </summary>
public sealed class SolicitudMovimiento
{
    [Required(ErrorMessage = "El numero de cuenta es obligatorio.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El numero de cuenta debe tener 8 digitos.")]
    public string NumCuenta { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de movimiento es obligatorio.")]
    [EnumDataType(typeof(TipoMov), ErrorMessage = "El tipo debe ser DEP (deposito) o RET (retiro).")]
    public TipoMov Tipo { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El valor del movimiento debe ser mayor que cero.")]
    public decimal Valor { get; set; }

    public DateTime? Fecha { get; set; }
}
