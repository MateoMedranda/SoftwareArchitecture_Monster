using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using ServidorConversionesDotnet.Modelos;

namespace ServidorConversionesDotnet.Filtros;

/// <summary>
/// Filtro de autorización global: exige el encabezado
/// 'Authorization: Bearer {token}' y valida firma y expiración del JWT.
/// Los endpoints marcados con [AllowAnonymous] (login) quedan exentos.
/// </summary>
public sealed class FiltroAutorizacionToken : IAuthorizationFilter
{
    private const string PrefijoBearer = "Bearer ";
    private const string MensajeTokenInvalido =
        "El token es inválido o ha expirado. Inicie sesión nuevamente.";

    private readonly IConfiguration _configuracion;

    public FiltroAutorizacionToken(IConfiguration configuracion)
    {
        _configuracion = configuracion;
    }

    public void OnAuthorization(AuthorizationFilterContext contexto)
    {
        if (contexto.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
        {
            return;
        }

        var encabezado = contexto.HttpContext.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(encabezado) ||
            !encabezado.StartsWith(PrefijoBearer, StringComparison.OrdinalIgnoreCase))
        {
            ResponderTokenInvalido(contexto);
            return;
        }

        var token = encabezado[PrefijoBearer.Length..].Trim();
        if (!ValidarToken(token))
        {
            ResponderTokenInvalido(contexto);
        }
    }

    private bool ValidarToken(string token)
    {
        var clave = _configuracion["CLAVE_SECRETA"];
        if (string.IsNullOrWhiteSpace(clave) || string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var parametros = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            new JwtSecurityTokenHandler().ValidateToken(token, parametros, out _);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static void ResponderTokenInvalido(AuthorizationFilterContext contexto)
    {
        var error = new RespuestaError
        {
            Codigo = 401,
            Mensaje = MensajeTokenInvalido,
            Detalle = "Revise el encabezado 'Authorization: Bearer {token}' e inicie sesión nuevamente " +
                      "en /api/autenticacion/login para obtener un token válido."
        };

        contexto.Result = new ObjectResult(error)
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            ContentTypes = { "application/json" }
        };
    }
}
