using CLI_WEB_SOAP_DOTNET.Servicios.Soap;
using Microsoft.Extensions.Options;
using System.ServiceModel;

/// <summary>Pasarela hacia el servicio SOAP. Es la única capa que conoce WCF.</summary>
public interface IClienteConversionUnidades
{
    Task<RespuestaInicioSesion> IniciarSesionAsync(SolicitudInicioSesion solicitud);
    Task<RespuestaConversion> ConvertirAsync(string token, SolicitudConversion solicitud);
}

public class ClienteConversionUnidades : IClienteConversionUnidades
{
    private readonly ChannelFactory<IServicioConversionUnidadesSoap> _fabrica;

    public ClienteConversionUnidades(IOptions<OpcionesServicioSoap> opciones)
    {
        var config = opciones.Value;

        // El WSDL declara BasicHttpBinding_IServicioConversionUnidades sobre HTTP (sin seguridad).
        // Si en IIS publicas con HTTPS, cambia a BasicHttpSecurityMode.Transport.
        var binding = new BasicHttpBinding(config.Url.StartsWith("https", StringComparison.OrdinalIgnoreCase)
            ? BasicHttpSecurityMode.Transport
            : BasicHttpSecurityMode.None)
        {
            OpenTimeout = TimeSpan.FromSeconds(config.TimeoutSegundos),
            SendTimeout = TimeSpan.FromSeconds(config.TimeoutSegundos),
            ReceiveTimeout = TimeSpan.FromSeconds(config.TimeoutSegundos),
        };

        _fabrica = new ChannelFactory<IServicioConversionUnidadesSoap>(
            binding, new EndpointAddress(config.Url));
    }

    public Task<RespuestaInicioSesion> IniciarSesionAsync(SolicitudInicioSesion solicitud) =>
        Ejecutar(canal => canal.IniciarSesionAsync(solicitud));

    public async Task<RespuestaConversion> ConvertirAsync(string token, SolicitudConversion solicitud)
    {
        SolicitudConversionMensaje solicitudConv = new SolicitudConversionMensaje
        {
            Token = token,
            Conversion = solicitud
        };

        var respuesta = await Ejecutar(canal => canal.ConvertirAsync(solicitudConv));

        return respuesta.ConvertirResult
               ?? new RespuestaConversion { Exito = false, Mensaje = "El servidor devolvió una respuesta vacía." };
    }


    /// <summary>Abre un canal, ejecuta la operación y lo cierra (o lo aborta si falla).</summary>
    private async Task<T> Ejecutar<T>(Func<IServicioConversionUnidadesSoap, Task<T>> operacion)
    {
        var canal = _fabrica.CreateChannel();
        var canalCliente = (IClientChannel)canal;
        try
        {
            var resultado = await operacion(canal);
            canalCliente.Close();
            return resultado;
        }
        catch
        {
            canalCliente.Abort();
            throw;
        }
    }
}
