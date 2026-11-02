namespace SISTEMA_FACTURACION.Modelos;

public sealed class Cliente
{
    public int CodCliente { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}
