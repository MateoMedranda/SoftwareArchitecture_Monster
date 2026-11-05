using System;
using System.Windows.Forms;
using CLIESC_Facturacion.Config;
using CLIESC_Facturacion.Vistas;

namespace CLIESC_Facturacion
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal de la aplicacion.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppConfig.Load();

            using (var login = new FormLogin())
            {
                if (login.ShowDialog() != DialogResult.OK) return;
            }

            var inicio = new FormInicio();
            Navegacion.RegistrarInicio(inicio);
            Application.Run(inicio);
        }
    }
}
