namespace SISTEMA_BANQUITO.Modelo;

/// <summary>
/// Representa la tabla CLIENTE del core de BanQuito.
/// </summary>
public sealed class Cliente
{
    public int CodCliente { get; set; }

    public string Cedula { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Genero { get; set; } = string.Empty;

    public DateTime FechaNacimiento { get; set; }
}
