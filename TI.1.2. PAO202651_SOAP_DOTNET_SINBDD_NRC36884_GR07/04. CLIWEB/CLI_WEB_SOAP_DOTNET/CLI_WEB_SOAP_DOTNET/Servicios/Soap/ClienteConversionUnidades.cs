using System.ServiceModel;
using Microsoft.Extensions.Options;

namespace CLI_WEB_SOAP_DOTNET.Servicios.Soap;

/// <summary>Pasarela hacia el servicio SOAP. Es la única clase que conoce WCF.</summary>
public interface IClienteConversionUnidades
{
    Task<RespuestaInicioSesion> IniciarSesionAsync(SolicitudInicioSesion solicitud);
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

    public async Task<RespuestaInicioSesion> IniciarSesionAsync(SolicitudInicioSesion solicitud)
    {
        var canal = _fabrica.CreateChannel();
        var canalCliente = (IClientChannel)canal;
        try
        {
            var respuesta = await canal.IniciarSesionAsync(solicitud);
            canalCliente.Close();
            return respuesta;
        }
        catch
        {
            canalCliente.Abort();
            throw;
        }
    }
}
