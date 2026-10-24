using System.Windows;
using System.Windows.Controls;
using ClienteEscritorioConversion.Componentes;
using ClienteEscritorioConversion.Controladores;

namespace ClienteEscritorioConversion.Vistas
{
    /// <summary>
    /// Vista de conversión. Presenta los 3 selectores, el valor y el resultado,
    /// y delega cada acción en el controlador de conversión.
    /// </summary>
    public partial class VistaConversion : UserControl
    {
        private ControladorConversion? _controlador;

        public VistaConversion()
        {
            InitializeComponent();
        }

        public void Inicializar(ControladorConversion controlador, string? nombreUsuario)
        {
            _controlador = controlador;

            TextoUsuario.Text = string.IsNullOrWhiteSpace(nombreUsuario)
                ? "Selecciona el tipo de conversión y las unidades"
                : $"Sesión de {nombreUsuario} · Selecciona el tipo de conversión y las unidades";

            ComboCategoria.ItemsSource = controlador.Categorias;
            ComboCategoria.SelectedIndex = 0;

            RecargarUnidades(0);
            LimpiarResultado();
        }

        private void RecargarUnidades(int indiceCategoria)
        {
            if (_controlador is null)
            {
                return;
            }

            _controlador.CambiarCategoria(indiceCategoria);

            ComboOrigen.ItemsSource = _controlador.Unidades;
            ComboDestino.ItemsSource = _controlador.Unidades;

            if (_controlador.Unidades.Count > 0)
            {
                ComboOrigen.SelectedIndex = 0;
                ComboDestino.SelectedIndex = _controlador.Unidades.Count > 1 ? 1 : 0;
            }

            ActualizarEstadoBoton();
        }

        private void ComboCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_controlador is null || ComboCategoria.SelectedIndex < 0)
            {
                return;
            }

            _controlador.CambiarCategoria(ComboCategoria.SelectedIndex);
            ComboOrigen.ItemsSource = _controlador.Unidades;
            ComboDestino.ItemsSource = _controlador.Unidades;

            if (_controlador.Unidades.Count > 0)
            {
                ComboOrigen.SelectedIndex = 0;
                ComboDestino.SelectedIndex = _controlador.Unidades.Count > 1 ? 1 : 0;
            }

            LimpiarResultado();
            Banner.Ocultar();
            ActualizarEstadoBoton();
        }

        private void ComboUnidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_controlador is null)
            {
                return;
            }

            _controlador.CambiarUnidades(
                ComboOrigen.SelectedItem as string,
                ComboDestino.SelectedItem as string);

            LimpiarResultado();
            ActualizarEstadoBoton();
        }

        private void CampoValor_TextChanged(object sender, TextChangedEventArgs e)
        {
            _controlador?.CambiarValor(CampoValor.Text);
            ActualizarEstadoBoton();
        }

        private void ActualizarEstadoBoton()
        {
            if (_controlador is null)
            {
                return;
            }

            BotonConvertir.IsEnabled = _controlador.PuedeConvertir;

            if (_controlador.EsMismaUnidad)
            {
                Banner.Mostrar("No es posible convertir a la misma unidad.", TipoMensaje.Advertencia);
            }
            else if (Banner.Tipo == TipoMensaje.Advertencia)
            {
                Banner.Ocultar();
            }
        }

        private async void BotonConvertir_Click(object sender, RoutedEventArgs e)
        {
            if (_controlador is null)
            {
                return;
            }

            BotonConvertir.IsEnabled = false;

            try
            {
                string? errorLocal = await _controlador.ConvertirAsync();

                if (errorLocal is not null)
                {
                    Banner.Mostrar(errorLocal, TipoMensaje.Advertencia);
                    return;
                }

                if (_controlador.Exito)
                {
                    MostrarResultado(_controlador.Resultado);
                    Banner.Ocultar();
                }
                else
                {
                    LimpiarResultado();
                    // Mensaje del servidor tal cual, sin reescribirlo.
                    Banner.Mostrar(_controlador.MensajeServidor ?? string.Empty, TipoMensaje.Error);
                }
            }
            finally
            {
                BotonConvertir.IsEnabled = _controlador.PuedeConvertir;
            }
        }

        public void MostrarResultado(string resultado)
        {
            TextoResultado.Text = "Resultado: " + resultado;
            MarcoResultado.Visibility = Visibility.Visible;
        }

        private void LimpiarResultado()
        {
            TextoResultado.Text = string.Empty;
            MarcoResultado.Visibility = Visibility.Collapsed;
        }

        private void BotonCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            _controlador?.CerrarSesion();
        }
    }
}
