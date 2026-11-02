using System.ComponentModel.DataAnnotations;

namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Datos de entrada para otorgar un credito por la compra de un telefono.
/// </summary>
public sealed class SolicitudOtorgarCreditoDTO
{
    [Required(ErrorMessage = "El codigo de cliente es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El codigo de cliente debe ser mayor que cero.")]
    public int CodCliente { get; set; }

    [Required(ErrorMessage = "El precio del telefono es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio del telefono debe ser mayor que cero.")]
    public decimal PrecioTelefono { get; set; }

    [Required(ErrorMessage = "El numero de cuotas es obligatorio.")]
    [Range(1, 60, ErrorMessage = "El numero de cuotas debe estar entre 1 y 60.")]
    public int NumCuotas { get; set; }
}
