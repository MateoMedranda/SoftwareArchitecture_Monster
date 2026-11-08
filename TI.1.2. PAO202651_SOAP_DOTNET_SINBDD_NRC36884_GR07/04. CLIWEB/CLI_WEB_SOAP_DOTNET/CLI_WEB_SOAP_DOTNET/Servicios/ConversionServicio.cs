using CLI_WEB_SOAP_DOTNET.Servicios.Soap;
using CLI_WEB_SOAP_DOTNET.Modelos.Conversiones;
using System.Globalization;
using System.ServiceModel;

namespace CLI_WEB_SOAP_DOTNET.Servicios;

public interface IConversionServicio
{
    Task<ResultadoConversionModelo> ConvertirAsync(string token, SolicitudConversionModelo solicitud);
}

/// <summary>
/// Traduce el modelo de la aplicación al DTO SOAP, envía el token en la cabecera
/// y convierte los errores de comunicación en mensajes entendibles para la vista.
/// </summary>
public class ConversionServicio : IConversionServicio
{
    private readonly IClienteConversionUnidades _cliente;
    private readonly ILogger<ConversionServicio> _log;

    public ConversionServicio(IClienteConversionUnidades cliente, ILogger<ConversionServicio> log)
    {
        _cliente = cliente;
        _log = log;
    }

    public async Task<ResultadoConversionModelo> ConvertirAsync(string token, SolicitudConversionModelo solicitud)
    {
        try
        {
            var respuesta = await _cliente.ConvertirAsync(token, new SolicitudConversion
            {
                Categoria = solicitud.Categoria,
                UnidadOrigen = solicitud.UnidadOrigen,
                UnidadDestino = solicitud.UnidadDestino,
                // El WSDL define Valor como string. Se usa cultura invariante (punto decimal);
                // si tu servidor espera coma (es-EC), cambia esta línea.
                Valor = (solicitud.Valor ?? 0m).ToString(CultureInfo.InvariantCulture)
            });

            return respuesta.Exito
                ? new ResultadoConversionModelo(true, respuesta.ValorConvertido, respuesta.Mensaje ?? string.Empty)
                : new ResultadoConversionModelo(false, null, respuesta.Mensaje ?? "No se pudo realizar la conversión.");
        }
        catch (EndpointNotFoundException ex)
        {
            _log.LogError(ex, "No se encontró el endpoint SOAP");
            return new ResultadoConversionModelo(false, null, "No se pudo contactar con el servidor.");
        }
        catch (TimeoutException ex)
        {
            _log.LogError(ex, "Tiempo de espera agotado al llamar a Convertir");
            return new ResultadoConversionModelo(false, null, "El servidor tardó demasiado en responder.");
        }
        catch (FaultException ex)
        {
            _log.LogWarning(ex, "El servicio devolvió un fallo SOAP en Convertir");
            return new ResultadoConversionModelo(false, null, $"El servidor rechazó la solicitud: {ex.Message}");
        }
        catch (Exception ex) when (ex is CommunicationException or HttpRequestException)
        {
            _log.LogError(ex, "Error de comunicación con el servicio SOAP");
            return new ResultadoConversionModelo(false, null, "Error de comunicación con el servidor.");
        }
    }
}
