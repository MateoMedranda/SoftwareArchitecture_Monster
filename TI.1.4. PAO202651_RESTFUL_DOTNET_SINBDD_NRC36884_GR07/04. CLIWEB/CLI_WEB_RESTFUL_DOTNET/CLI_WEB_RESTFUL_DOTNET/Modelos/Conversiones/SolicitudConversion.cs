namespace CLI_WEB_RESTFUL_DOTNET.Modelos.Conversiones;

/// <summary>DTO de entrada de POST /api/conversion/{categoria}.</summary>
public class SolicitudConversion
{
    public double? Valor { get; set; }
    public string? UnidadOrigen { get; set; }
    public string? UnidadDestino { get; set; }
}
