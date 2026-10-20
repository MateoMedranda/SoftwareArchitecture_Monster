using System.Text;
using ClienteConsolaConversion.Configuracion;
using ClienteConsolaConversion.Controladores;
using ClienteConsolaConversion.Modelos;
using ClienteConsolaConversion.Servicios;

namespace ClienteConsolaConversion
{
    /// <summary>
    /// Punto de entrada del cliente de consola.
    /// Solo monta las capas (configuración, modelo, servicio SOAP y controlador)
    /// y delega el flujo completo en ControladorAplicacion.
    /// </summary>
    internal static class Program
    {
        private static async Task Main()
        {
            ConfigurarConsola();

            ConfiguracionCliente configuracion = ConfiguracionCliente.Cargar();
            var servicio = new ClienteSoapConversion(configuracion.UrlEndpoint);
            var sesion = new SesionActual();

            var aplicacion = new ControladorAplicacion(servicio, sesion);
            await aplicacion.EjecutarAsync();
        }

        private static void ConfigurarConsola()
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
            }
            catch (IOException)
            {
                // Consola sin soporte de codificación: se mantiene la predeterminada.
            }

            Console.Title = "Monster - Conversión de Unidades (Cliente SOAP)";
        }
    }
}
