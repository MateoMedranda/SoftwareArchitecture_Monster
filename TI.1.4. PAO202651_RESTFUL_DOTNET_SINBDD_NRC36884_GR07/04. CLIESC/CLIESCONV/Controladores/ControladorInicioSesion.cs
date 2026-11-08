using ClienteEscritorioConversion.Modelos;
using ClienteEscritorioConversion.Servicios;

namespace ClienteEscritorioConversion.Controladores
{
    /// <summary>
    /// Controlador del inicio de sesión: valida los campos en el cliente
    /// y delega la autenticación en el servicio SOAP.
    /// Los mensajes del servidor se propagan sin modificar.
    /// </summary>
    public class ControladorInicioSesion
    {
        public const string MensajeUsuarioVacio = "El usuario no puede estar vacío";
        public const string MensajeContrasenaVacia = "La contraseña no puede estar vacía";

        private readonly ClienteSoapConversion _servicio;
        private readonly SesionActual _sesion;

        public ControladorInicioSesion(ClienteSoapConversion servicio, SesionActual sesion)
        {
            _servicio = servicio;
            _sesion = sesion;
        }

        public string Usuario { get; private set; } = string.Empty;

        public string Contrasena { get; private set; } = string.Empty;

        public bool PuedeIniciarSesion =>
            !string.IsNullOrWhiteSpace(Usuario) && !string.IsNullOrEmpty(Contrasena);

        /// <summary>Mensaje del servidor o mensaje local del último intento de login.</summary>
        public string? Mensaje { get; private set; }

        /// <summary>True cuando el mensaje del último intento es un error.</summary>
        public bool EsError { get; private set; }

        public event Action? InicioSesionExitoso;

        public void AlCambiarUsuario(string usuario)
        {
            Usuario = usuario ?? string.Empty;
        }

        public void AlCambiarContrasena(string contrasena)
        {
            Contrasena = contrasena ?? string.Empty;
        }

        public async Task IniciarSesionAsync()
        {
            Mensaje = null;
            EsError = false;

            // Validación local: no se llama al servidor con campos vacíos.
            if (string.IsNullOrWhiteSpace(Usuario))
            {
                Mensaje = MensajeUsuarioVacio;
                EsError = true;
                return;
            }

            if (string.IsNullOrEmpty(Contrasena))
            {
                Mensaje = MensajeContrasenaVacia;
                EsError = true;
                return;
            }

            RespuestaInicioSesion respuesta = await _servicio.IniciarSesionAsync(Usuario, Contrasena);

            if (respuesta.Exito && !string.IsNullOrEmpty(respuesta.Token))
            {
                _sesion.Iniciar(Usuario, respuesta.Token);
                Mensaje = respuesta.Mensaje;
                InicioSesionExitoso?.Invoke();
                return;
            }

            // Mensaje del servidor tal cual (credenciales inválidas, red caída, etc.).
            Mensaje = respuesta.Mensaje;
            EsError = true;
        }
    }
}
