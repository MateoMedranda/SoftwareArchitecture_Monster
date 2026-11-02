namespace SISTEMA_BANQUITO.Modelo;

/// <summary>
/// Representa la tabla CUENTA del core de BanQuito.
/// </summary>
public sealed class Cuenta
{
    public string NumCuenta { get; set; } = string.Empty;

    public int CodCliente { get; set; }

    public decimal SaldoCuenta { get; set; }
}
