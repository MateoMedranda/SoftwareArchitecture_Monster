namespace CLI_WEB_RESTFUL_DOTNET.Modelos;

/// <summary>DTO de entrada para POST /api/autenticacion/login.</summary>
public class SolicitudLogin
{
    public string? Usuario { get; set; }
    public string? Contrasena { get; set; }
}
