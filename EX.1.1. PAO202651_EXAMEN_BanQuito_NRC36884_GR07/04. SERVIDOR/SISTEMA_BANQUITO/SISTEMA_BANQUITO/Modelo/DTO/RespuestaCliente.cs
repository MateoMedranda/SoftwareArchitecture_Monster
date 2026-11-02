namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Respuesta con los datos de un cliente. El mapeo inverso lo realiza
/// <see cref="Mapeo.MapeoCliente"/> a partir de la entidad Cliente.
/// </summary>
public sealed class RespuestaCliente
{
    public int CodCliente { get; set; }

    public string Cedula { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Genero { get; set; } = string.Empty;

    public DateTime FechaNacimiento { get; set; }

    public int Edad { get; set; }
}
