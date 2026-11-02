namespace SISTEMA_BANQUITO.Modelo;

/// <summary>
/// Representa la tabla MOVIMIENTO. El campo TIPO usa DEP (deposito) o RET (retiro).
/// </summary>
public sealed class Movimiento
{

    public int CodMovimiento { get; set; }

    public string NumCuenta { get; set; } = string.Empty;

    public TipoMov Tipo { get; set; }

    public decimal Valor { get; set; }

    public DateTime Fecha { get; set; }
}
