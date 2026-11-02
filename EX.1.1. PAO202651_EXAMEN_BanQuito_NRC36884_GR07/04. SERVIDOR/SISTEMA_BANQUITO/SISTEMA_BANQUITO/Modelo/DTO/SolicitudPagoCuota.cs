using System.ComponentModel.DataAnnotations;

namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Datos de entrada para registrar el pago de una cuota de amortizacion.
/// </summary>
public sealed class SolicitudPagoCuota
{
    [Range(1, int.MaxValue, ErrorMessage = "El numero de credito debe ser mayor que cero.")]
    public int NumCredito { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El numero de cuota debe ser mayor que cero.")]
    public int NumCuota { get; set; }

    [Required(ErrorMessage = "El numero de cuenta es obligatorio.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El numero de cuenta debe tener 8 digitos.")]
    public string NumCuenta { get; set; } = string.Empty;
}
