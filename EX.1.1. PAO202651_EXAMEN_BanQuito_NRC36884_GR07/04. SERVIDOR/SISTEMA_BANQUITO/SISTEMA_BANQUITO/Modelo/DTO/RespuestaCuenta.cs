namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Respuesta con los datos de una cuenta. El mapeo inverso lo realiza
/// <see cref="Mapeo.MapeoCuenta"/> a partir de la entidad Cuenta.
/// </summary>
public sealed class RespuestaCuenta
{
    public string NumCuenta { get; set; } = string.Empty;

    public int CodCliente { get; set; }

    public decimal SaldoCuenta { get; set; }
}
