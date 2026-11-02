namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Respuesta con los datos de un movimiento. El mapeo inverso lo realiza
/// <see cref="Mapeo.MapeoMovimiento"/> a partir de la entidad Movimiento.
/// </summary>
public sealed class RespuestaMovimiento
{
    public int CodMovimiento { get; set; }

    public string NumCuenta { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public DateTime Fecha { get; set; }

    /// <summary>
    /// Saldo de la cuenta despues de la operacion. Solo se envia al crear
    /// o actualizar un movimiento.
    /// </summary>
    public decimal? SaldoCuenta { get; set; }
}
