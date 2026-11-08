namespace CLI_WEB_SOAP_DOTNET.Servicios.Soap;

public class OpcionesServicioSoap
{
    public const string Seccion = "ServicioSoap";

    /// <summary>URL del .svc publicado en IIS.</summary>
    public string Url { get; set; } = string.Empty;

    public int TimeoutSegundos { get; set; } = 30;
}
