using System.Windows;
using ClienteEscritorioConversion.Componentes;
using ClienteEscritorioConversion.Modelos;
using ClienteEscritorioConversion.Servicios;
using ClienteEscritorioConversion.Vistas;

namespace ClienteEscritorioConversion.Controladores
{
    /// <summary>
    /// Controlador de la aplicación: navega entre las vistas de login y conversión
    /// y conecta los controladores de cada pantalla con su vista.
    /// </summary>
    public class ControladorAplicacion
    {
        private readonly ClienteSoapConversion _servicio;
        private readonly SesionActual _sesion;

        private readonly VistaInicioSesion _vistaInicioSesion = new VistaInicioSesion();
        private readonly VistaConversion _vistaConversion = new VistaConversion();

        private readonly ControladorInicioSesion _controladorInicioSesion;
        private readonly ControladorConversion _controladorConversion;

        public ControladorAplicacion(ClienteSoapConversion servicio, SesionActual sesion)
        {
            _servicio = servicio;
            _sesion = sesion;

            _controladorInicioSesion = new ControladorInicioSesion(_servicio, _sesion);
            _controladorConversion = new ControladorConversion(_servicio, _sesion);

            _controladorInicioSesion.InicioSesionExitoso += MostrarConversion;
            _controladorConversion.SesionCerrada += MostrarInicioSesion;

            _vistaInicioSesion.Inicializar(_controladorInicioSesion);

            // El botón del formulario no expone eventos públicos: se enlaza el resultado
            // del intento desde la propia vista mediante este hook.
            _vistaInicioSesion.IntentoTerminado += MostrarMensajeInicioSesion;
        }

        /// <summary>Contenedor donde se intercambian las vistas (lo entrega MainWindow).</summary>
        public event Action<UIElement>? VistaCambiada;

        public void Iniciar()
        {
            MostrarInicioSesion();
        }

        private void MostrarInicioSesion()
        {
            _vistaInicioSesion.LimpiarFormulario();
            VistaCambiada?.Invoke(_vistaInicioSesion);
            _vistaInicioSesion.EnfocarUsuario();
        }

        private void MostrarConversion()
        {
            _vistaConversion.Inicializar(_controladorConversion, _sesion.NombreUsuario);
            VistaCambiada?.Invoke(_vistaConversion);
        }

        private void MostrarMensajeInicioSesion()
        {
            string? mensaje = _controladorInicioSesion.Mensaje;

            if (string.IsNullOrEmpty(mensaje))
            {
                return;
            }

            _vistaInicioSesion.MostrarMensaje(
                mensaje,
                _controladorInicioSesion.EsError ? TipoMensaje.Error : TipoMensaje.Exito);
        }
    }
}
