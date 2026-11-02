using System.ComponentModel.DataAnnotations;

namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Datos de entrada para consultar la tabla de amortizacion.
/// Se puede consultar por numero de credito o por codigo de cliente.
/// </summary>
public sealed class SolicitudTablaAmortizacionDTO
{
    public int? NumCredito { get; set; }

    public int? CodCliente { get; set; }
}
