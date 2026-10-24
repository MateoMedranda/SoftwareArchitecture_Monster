using System.Windows;
using ClienteEscritorioConversion.Configuracion;
using ClienteEscritorioConversion.Controladores;
using ClienteEscritorioConversion.Modelos;
using ClienteEscritorioConversion.Servicios;

namespace ClienteEscritorioConversion
{
    /// <summary>
    /// Punto de entrada: monta las capas (configuración, modelo, servicio REST,
    /// controlador y ventana contenedora) y arranca la aplicación.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var variables = CargadorEnv.Cargar(AppContext.BaseDirectory);
                foreach (var variable in variables)
                {
                    if (!string.IsNullOrWhiteSpace(variable.Value))
                    {
                        Environment.SetEnvironmentVariable(variable.Key, variable.Value);
                    }
                }
            }
            catch
            {
            }

            ConfiguracionCliente configuracion = ConfiguracionCliente.Cargar();
            var servicio = new ClienteSoapConversion(configuracion.UrlBase);
            var sesion = new SesionActual();

            var controlador = new ControladorAplicacion(servicio, sesion);

            var ventana = new MainWindow();
            MainWindow = ventana;
            ventana.Conectar(controlador);
            ventana.Show();
        }
    }
}
