namespace CLI_WEB_RESTFUL_DOTNET.Modelos.Conversiones;

/// <summary>DTO de salida de POST /api/conversion/{categoria}.</summary>
public class RespuestaConversion
{
    public double ValorConvertido { get; set; }
    public string? UnidadOrigen { get; set; }
    public string? UnidadDestino { get; set; }
}
