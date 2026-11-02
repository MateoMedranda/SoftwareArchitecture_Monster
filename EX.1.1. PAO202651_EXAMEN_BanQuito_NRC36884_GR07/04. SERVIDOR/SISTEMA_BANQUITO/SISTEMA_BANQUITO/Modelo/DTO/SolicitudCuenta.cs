using System.ComponentModel.DataAnnotations;

namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Datos de entrada para crear o actualizar una cuenta.
/// </summary>
public sealed class SolicitudCuenta
{
    [Range(1, int.MaxValue, ErrorMessage = "El codigo de cliente debe ser mayor que cero.")]
    public int CodCliente { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El saldo no puede ser negativo.")]
    public decimal SaldoCuenta { get; set; }
}
