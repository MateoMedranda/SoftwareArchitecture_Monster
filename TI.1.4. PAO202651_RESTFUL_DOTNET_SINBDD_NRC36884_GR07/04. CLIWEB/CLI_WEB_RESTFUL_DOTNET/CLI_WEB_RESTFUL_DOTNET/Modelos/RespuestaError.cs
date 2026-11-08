namespace CLI_WEB_RESTFUL_DOTNET.Modelos;

/// <summary>DTO de error devuelto por la API REST (400/401).</summary>
public class RespuestaError
{
    public int Codigo { get; set; }
    public string? Mensaje { get; set; }
    public string? Detalle { get; set; }
}
