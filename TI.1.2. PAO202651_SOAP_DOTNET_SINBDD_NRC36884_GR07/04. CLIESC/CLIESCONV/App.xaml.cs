using System.Windows;
using ClienteEscritorioConversion.Configuracion;
using ClienteEscritorioConversion.Controladores;
using ClienteEscritorioConversion.Modelos;
using ClienteEscritorioConversion.Servicios;

namespace ClienteEscritorioConversion
{
    /// <summary>
    /// Punto de entrada: monta las capas (configuración, modelo, servicio SOAP,
    /// controlador y ventana contenedora) y arranca la aplicación.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfiguracionCliente configuracion = ConfiguracionCliente.Cargar();
            var servicio = new ClienteSoapConversion(configuracion.UrlEndpoint);
            var sesion = new SesionActual();

            var controlador = new ControladorAplicacion(servicio, sesion);

            var ventana = new MainWindow();
            MainWindow = ventana;
            ventana.Conectar(controlador);
            ventana.Show();
        }
    }
}
