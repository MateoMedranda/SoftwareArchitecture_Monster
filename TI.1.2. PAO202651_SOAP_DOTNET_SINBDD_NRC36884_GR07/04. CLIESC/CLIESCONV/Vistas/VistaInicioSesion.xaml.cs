using System.Windows;
using System.Windows.Controls;
using ClienteEscritorioConversion.Componentes;
using ClienteEscritorioConversion.Controladores;

namespace ClienteEscritorioConversion.Vistas
{
    /// <summary>
    /// Vista de inicio de sesión. Solo presenta los campos y delega toda la lógica
    /// en el controlador: aquí no se llama al servidor ni se decide la navegación.
    /// </summary>
    public partial class VistaInicioSesion : UserControl
    {
        private ControladorInicioSesion? _controlador;

        public VistaInicioSesion()
        {
            InitializeComponent();
        }

        public void Inicializar(ControladorInicioSesion controlador)
        {
            _controlador = controlador;
            CampoUsuario.Focus();
        }

        /// <summary>Se dispara al terminar cada intento de login, para que el contenedor muestre el mensaje.</summary>
        public event Action? IntentoTerminado;

        private void CampoUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {
            _controlador?.AlCambiarUsuario(CampoUsuario.Text);
            ActualizarBoton();
        }

        private void CampoContrasena_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _controlador?.AlCambiarContrasena(CampoContrasena.Password);
            ActualizarBoton();
        }

        private void ActualizarBoton()
        {
            BotonIngresar.IsEnabled = _controlador?.PuedeIniciarSesion ?? false;
        }

        private async void BotonIngresar_Click(object sender, RoutedEventArgs e)
        {
            if (_controlador is null)
            {
                return;
            }

            BotonIngresar.IsEnabled = false;

            try
            {
                await _controlador.IniciarSesionAsync();
                IntentoTerminado?.Invoke();
            }
            finally
            {
                BotonIngresar.IsEnabled = _controlador.PuedeIniciarSesion;
            }
        }

        /// <summary>El banner lo manipula la vista; el texto exacto lo entrega el controlador/servidor.</summary>
        public void MostrarMensaje(string texto, TipoMensaje tipo)
        {
            Banner.Mostrar(texto, tipo);
        }

        public void LimpiarFormulario()
        {
            CampoUsuario.Clear();
            CampoContrasena.Clear();
            Banner.Ocultar();
        }

        public void EnfocarUsuario()
        {
            CampoUsuario.Focus();
        }

        private void ImagenLogo_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            // El logo es opcional: si el archivo no está, el espacio se colapsa sin romper la vista.
            ImagenLogo.Visibility = Visibility.Collapsed;
        }
    }
}
