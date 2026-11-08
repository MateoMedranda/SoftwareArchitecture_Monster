namespace ServidorConversionesDotnet.Modelos;

/// <summary>
/// Respuesta exitosa del inicio de sesión: token JWT y su fecha de expiración.
/// </summary>
public sealed class RespuestaLogin
{
    /// <summary>Token JWT que el cliente debe enviar en las conversiones.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Instante (UTC) en el que el token deja de ser válido.</summary>
    public DateTime Expiracion { get; set; }
}
