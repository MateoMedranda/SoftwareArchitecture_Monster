using ServidorConversionesDotnet.Modelos;
using ServidorConversionesDotnet.Utilidades;

namespace ServidorConversionesDotnet.Servicios;

/// <summary>
/// Valida las credenciales del login y solicita la generación del token JWT.
/// </summary>
public sealed class ServicioAutenticacion
{
    private readonly IConfiguration _configuracion;
    private readonly GeneradorToken _generadorToken;

    public ServicioAutenticacion(IConfiguration configuracion, GeneradorToken generadorToken)
    {
        _configuracion = configuracion;
        _generadorToken = generadorToken;
    }

    /// <summary>
    /// Valida la solicitud de login y, si las credenciales son correctas,
    /// devuelve el token JWT con su fecha de expiración.
    /// </summary>
    public ResultadoServicio<RespuestaLogin> IniciarSesion(SolicitudLogin solicitud)
    {
        if (string.IsNullOrWhiteSpace(solicitud.Usuario) ||
            string.IsNullOrWhiteSpace(solicitud.Contrasena))
        {
            return ResultadoServicio<RespuestaLogin>.ConError(new RespuestaError
            {
                Codigo = 400,
                Mensaje = "El usuario y la contraseña son obligatorios.",
                Detalle = "Los campos 'usuario' y 'contrasena' no pueden estar vacíos."
            });
        }

        var usuarioDemo = _configuracion["USUARIO_DEMO"];
        var contrasenaDemo = _configuracion["CONTRASENA_DEMO"];

        var credencialesCorrectas =
            !string.IsNullOrWhiteSpace(usuarioDemo) &&
            !string.IsNullOrWhiteSpace(contrasenaDemo) &&
            string.Equals(solicitud.Usuario.Trim(), usuarioDemo, StringComparison.Ordinal) &&
            string.Equals(solicitud.Contrasena, contrasenaDemo, StringComparison.Ordinal);

        if (!credencialesCorrectas)
        {
            return ResultadoServicio<RespuestaLogin>.ConError(new RespuestaError
            {
                Codigo = 401,
                Mensaje = "Usuario o contraseña incorrectos.",
                Detalle = "Las credenciales enviadas no coinciden con las registradas."
            });
        }

        try
        {
            return ResultadoServicio<RespuestaLogin>.ConExito(
                _generadorToken.Crear(solicitud.Usuario.Trim()));
        }
        catch (Exception excepcion)
        {
            return ResultadoServicio<RespuestaLogin>.ConError(new RespuestaError
            {
                Codigo = 500,
                Mensaje = "No fue posible generar el token de acceso. Intente nuevamente más tarde.",
                Detalle = excepcion.Message
            });
        }
    }
}
