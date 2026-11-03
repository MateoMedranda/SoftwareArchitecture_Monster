using System;
using System.Linq;
using System.Windows.Forms;

namespace CLIESC_Facturacion.Vistas
{
    /// <summary>
    /// Mantiene una sola pantalla visible y regresa al inicio cuando se
    /// cierra la pantalla secundaria.
    /// </summary>
    internal static class Navegacion
    {
        private static Form _inicio;

        public static void RegistrarInicio(Form inicio)
        {
            _inicio = inicio;
        }

        public static void Abrir(Form actual, Form destino)
        {
            if (destino == null || destino.IsDisposed)
            {
                return;
            }

            destino.FormClosed += DestinoFormClosed;
            destino.Show();
            destino.BringToFront();

            if (actual != null && !actual.IsDisposed)
            {
                actual.Hide();
                if (actual != _inicio)
                {
                    actual.Close();
                }
            }
        }

        private static void DestinoFormClosed(object sender, FormClosedEventArgs e)
        {
            var algunaVisible = Application.OpenForms
                .Cast<Form>()
                .Any(form => form.Visible && !form.IsDisposed);

            if (!algunaVisible && _inicio != null && !_inicio.IsDisposed)
            {
                _inicio.Show();
                _inicio.BringToFront();
            }
        }
    }
}
