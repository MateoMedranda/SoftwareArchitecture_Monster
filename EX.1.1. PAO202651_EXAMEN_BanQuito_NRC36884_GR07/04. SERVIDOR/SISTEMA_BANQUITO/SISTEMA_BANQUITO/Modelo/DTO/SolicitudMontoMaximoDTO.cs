using System.ComponentModel.DataAnnotations;

namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Datos de entrada para calcular el monto maximo de credito de un cliente.
/// </summary>
public sealed class SolicitudMontoMaximoDTO
{
    [Required(ErrorMessage = "El codigo de cliente es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El codigo de cliente debe ser mayor que cero.")]
    public int CodCliente { get; set; }
}
