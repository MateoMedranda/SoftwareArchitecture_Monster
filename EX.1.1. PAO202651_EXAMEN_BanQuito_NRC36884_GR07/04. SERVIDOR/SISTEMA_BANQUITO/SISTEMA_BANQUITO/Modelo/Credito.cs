namespace SISTEMA_BANQUITO.Modelo;

/// <summary>
/// Representa la tabla CREDITO del modulo de creditos.
/// </summary>
public sealed class Credito
{
    public int NumCredito { get; set; }

    public string NumCuenta { get; set; } = string.Empty;

    public decimal CreditoTotal { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }
}
