using System.ComponentModel.DataAnnotations;

namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Datos de entrada para actualizar el monto total de un credito existente.
/// </summary>
public sealed class SolicitudActualizarCredito
{
    [Range(0.01, double.MaxValue, ErrorMessage = "El total del credito debe ser mayor que cero.")]
    public decimal CreditoTotal { get; set; }
}
