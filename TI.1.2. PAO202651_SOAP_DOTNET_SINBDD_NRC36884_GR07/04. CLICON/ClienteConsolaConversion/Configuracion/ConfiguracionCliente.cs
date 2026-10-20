using Microsoft.Extensions.Configuration;

namespace ClienteConsolaConversion.Configuracion
{
    /// <summary>
    /// Carga la configuración del cliente (URL del endpoint SOAP) desde appsettings.json.
    /// </summary>
    public class ConfiguracionCliente
    {
        /// <summary>Valor por defecto: endpoint publicado por IIS Express del servidor.</summary>
        public const string UrlEndpointPorDefecto =
            "http://localhost:49159/Servicios/ServicioConversionUnidades.svc";

        public string UrlEndpoint { get; }

        private ConfiguracionCliente(string urlEndpoint)
        {
            UrlEndpoint = urlEndpoint;
        }

        public static ConfiguracionCliente Cargar()
        {
            string? urlEndpoint = null;

            try
            {
                IConfiguration configuracion = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .Build();

                urlEndpoint = configuracion["Servicio:UrlEndpoint"];
            }
            catch (Exception)
            {
                // Si el archivo no se puede leer se usa el endpoint por defecto.
            }

            if (string.IsNullOrWhiteSpace(urlEndpoint))
            {
                urlEndpoint = UrlEndpointPorDefecto;
            }

            return new ConfiguracionCliente(urlEndpoint.Trim());
        }
    }
}
