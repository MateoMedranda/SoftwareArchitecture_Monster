using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ServidorConversionesDotnet.Modelos;

namespace ServidorConversionesDotnet.Utilidades;

/// <summary>
/// Genera tokens JWT firmados con clave simétrica (HS256) a partir de la
/// configuración definida en el archivo .development.env.
/// </summary>
public sealed class GeneradorToken
{
    private const int ExpiracionPorDefectoMinutos = 60;

    private readonly IConfiguration _configuracion;

    public GeneradorToken(IConfiguration configuracion)
    {
        _configuracion = configuracion;
    }

    /// <summary>
    /// Crea el token JWT para el usuario indicado y devuelve el token
    /// junto con su fecha de expiración (UTC).
    /// </summary>
    public RespuestaLogin Crear(string usuario)
    {
        var clave = _configuracion["CLAVE_SECRETA"];
        if (string.IsNullOrWhiteSpace(clave))
        {
            throw new InvalidOperationException(
                "Falta la variable CLAVE_SECRETA en el archivo .development.env.");
        }

        var minutosTexto = _configuracion["TIEMPO_EXPIRACION_MINUTOS"];
        var minutos = int.TryParse(minutosTexto, out var minutosLeidos) && minutosLeidos > 0
            ? minutosLeidos
            : ExpiracionPorDefectoMinutos;

        var expiracion = DateTime.UtcNow.AddMinutes(minutos);

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
            SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, usuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = expiracion,
            NotBefore = DateTime.UtcNow,
            SigningCredentials = credenciales,
            Issuer = "ServidorConversionesDotnet",
            Audience = "ClientesConversionUnidades"
        };

        var manejador = new JwtSecurityTokenHandler();
        var token = manejador.WriteToken(manejador.CreateToken(descriptor));

        return new RespuestaLogin
        {
            Token = token,
            Expiracion = expiracion
        };
    }
}
