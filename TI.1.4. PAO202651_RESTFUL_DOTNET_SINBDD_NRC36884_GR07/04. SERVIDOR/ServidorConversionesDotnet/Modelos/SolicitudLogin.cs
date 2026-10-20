namespace ServidorConversionesDotnet.Modelos;

/// <summary>
/// Cuerpo de la solicitud para el inicio de sesión.
/// </summary>
public sealed class SolicitudLogin
{
    /// <summary>Nombre de usuario.</summary>
    public string? Usuario { get; set; }

    /// <summary>Contraseña del usuario.</summary>
    public string? Contrasena { get; set; }
}
