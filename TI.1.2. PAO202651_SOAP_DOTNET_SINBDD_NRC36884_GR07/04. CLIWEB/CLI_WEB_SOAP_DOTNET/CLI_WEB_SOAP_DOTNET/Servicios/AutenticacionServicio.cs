using System.ServiceModel;
using CLI_WEB_SOAP_DOTNET.Modelos;
using CLI_WEB_SOAP_DOTNET.Servicios.Soap;

namespace CLI_WEB_SOAP_DOTNET.Servicios;

public interface IAutenticacionServicio
{
    Task<ResultadoLogin> IniciarSesionAsync(CredencialesLogin credenciales);
}

/// <summary>
/// Traduce entre los modelos de la aplicación y los DTOs SOAP, y convierte
/// los errores de comunicación en mensajes entendibles para la vista.
/// </summary>
public class AutenticacionServicio : IAutenticacionServicio
{
    private readonly IClienteConversionUnidades _cliente;
    private readonly ILogger<AutenticacionServicio> _log;

    public AutenticacionServicio(IClienteConversionUnidades cliente, ILogger<AutenticacionServicio> log)
    {
        _cliente = cliente;
        _log = log;
    }

    public async Task<ResultadoLogin> IniciarSesionAsync(CredencialesLogin credenciales)
    {
        try
        {
            var respuesta = await _cliente.IniciarSesionAsync(new SolicitudInicioSesion
            {
                NombreUsuario = credenciales.NombreUsuario,
                Contrasena = credenciales.Contrasena
            });

            if (respuesta.Exito && !string.IsNullOrEmpty(respuesta.Token))
                return new ResultadoLogin(true, respuesta.Token, respuesta.Mensaje ?? "Sesión iniciada.");

            return new ResultadoLogin(false, null, respuesta.Mensaje ?? "Credenciales incorrectas.");
        }
        catch (EndpointNotFoundException ex)
        {
            _log.LogError(ex, "No se encontró el endpoint SOAP");
            return new ResultadoLogin(false, null, "No se pudo contactar con el servidor. Revisa la URL del servicio.");
        }
        catch (TimeoutException ex)
        {
            _log.LogError(ex, "Tiempo de espera agotado al llamar a IniciarSesion");
            return new ResultadoLogin(false, null, "El servidor tardó demasiado en responder.");
        }
        catch (Exception ex) when (ex is CommunicationException or HttpRequestException)
        {
            _log.LogError(ex, "Error de comunicación con el servicio SOAP");
            return new ResultadoLogin(false, null, "Error de comunicación con el servidor.");
        }
    }
}
