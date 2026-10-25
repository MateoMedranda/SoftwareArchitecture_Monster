namespace CLI_WEB_RESTFUL_DOTNET.Servicios;

/// <summary>
/// Guarda el token devuelto por IniciarSesion mientras dure el circuito de Blazor Server.
/// El token se enviar� despu�s como cabecera SOAP en la operaci�n Convertir.
/// </summary>
public class SesionUsuario
{
    public string? NombreUsuario { get; private set; }
    public string? Token { get; private set; }
    public bool EstaAutenticado => !string.IsNullOrEmpty(Token);

    public event Action? CambioEstado;

    public void Iniciar(string nombreUsuario, string token)
    {
        NombreUsuario = nombreUsuario;
        Token = token;
        CambioEstado?.Invoke();
    }

    public void Cerrar()
    {
        NombreUsuario = null;
        Token = null;
        CambioEstado?.Invoke();
    }
}
