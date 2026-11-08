namespace CLI_WEB_RESTFUL_DOTNET.Modelos;

/// <summary>DTO de salida de POST /api/autenticacion/login.</summary>
public class RespuestaLogin
{
    public string? Token { get; set; }
    public DateTime Expiracion { get; set; }
}
