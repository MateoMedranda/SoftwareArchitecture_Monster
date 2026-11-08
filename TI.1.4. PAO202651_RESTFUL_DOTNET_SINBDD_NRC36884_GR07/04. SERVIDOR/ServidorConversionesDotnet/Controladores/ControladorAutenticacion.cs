using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServidorConversionesDotnet.Modelos;
using ServidorConversionesDotnet.Servicios;

namespace ServidorConversionesDotnet.Controladores;

/// <summary>
/// Endpoints de autenticación: genera el token JWT usado en las conversiones.
/// </summary>
[Route("api/autenticacion")]
[ApiController]
[AllowAnonymous]
public sealed class ControladorAutenticacion : ControllerBase
{
    private readonly ServicioAutenticacion _servicioAutenticacion;

    public ControladorAutenticacion(ServicioAutenticacion servicioAutenticacion)
    {
        _servicioAutenticacion = servicioAutenticacion;
    }

    /// <summary>POST /api/autenticacion/login</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(RespuestaLogin), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespuestaError), StatusCodes.Status401Unauthorized)]
    public IActionResult IniciarSesion([FromBody] SolicitudLogin solicitud)
    {
        var resultado = _servicioAutenticacion.IniciarSesion(solicitud ?? new SolicitudLogin());

        if (!resultado.Exitoso)
        {
            return StatusCode(resultado.Error!.Codigo, resultado.Error);
        }

        return Ok(resultado.Valor);
    }
}
