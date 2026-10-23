using System.ComponentModel.DataAnnotations;

namespace CLI_WEB_SOAP_DOTNET.Modelos;

/// <summary>Modelo del formulario de inicio de sesión (MODELO de MVC).</summary>
public class CredencialesLogin
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Contrasena { get; set; } = string.Empty;
}
