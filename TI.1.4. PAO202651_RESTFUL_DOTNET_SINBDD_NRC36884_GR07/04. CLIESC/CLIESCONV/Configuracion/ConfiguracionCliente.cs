using System.Net.Http;

namespace ClienteEscritorioConversion.Configuracion
{
    public class ConfiguracionCliente
    {
        public string UrlBase { get; }

        private ConfiguracionCliente(string urlBase)
        {
            UrlBase = urlBase.TrimEnd('/');
        }

        public static ConfiguracionCliente Cargar()
        {
            string? urlBase = null;

            try
            {
                var ruta = AppContext.BaseDirectory;
                var variables = CargadorEnv.Cargar(ruta);
                if (variables.TryGetValue("URL_BASE", out var valor) && !string.IsNullOrWhiteSpace(valor))
                {
                    urlBase = valor.Trim();
                }
            }
            catch
            {
            }

            if (string.IsNullOrWhiteSpace(urlBase))
            {
                try
                {
                    urlBase = Environment.GetEnvironmentVariable("URL_BASE");
                }
                catch
                {
                }
            }

            if (string.IsNullOrWhiteSpace(urlBase))
            {
                urlBase = "http://localhost:5000";
            }

            return new ConfiguracionCliente(urlBase);
        }

        public HttpClient CrearHttpClient()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(UrlBase + "/"),
                Timeout = TimeSpan.FromSeconds(30)
            };
            return httpClient;
        }

        public HttpClient CrearHttpClient(int segundosTimeout)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(UrlBase + "/"),
                Timeout = TimeSpan.FromSeconds(segundosTimeout > 0 ? segundosTimeout : 30)
            };
            return httpClient;
        }
    }
}
