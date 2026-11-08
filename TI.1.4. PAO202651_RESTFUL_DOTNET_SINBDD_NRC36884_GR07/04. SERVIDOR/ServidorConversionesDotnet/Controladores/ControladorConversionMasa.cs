using Microsoft.AspNetCore.Mvc;
using ServidorConversionesDotnet.Modelos;
using ServidorConversionesDotnet.Servicios;

namespace ServidorConversionesDotnet.Controladores;

/// <summary>
/// Endpoint de conversión de masa (protegido por token).
/// </summary>
[Route("api/conversion")]
[ApiController]
public sealed class ControladorConversionMasa : ControllerBase
{
    private readonly ServicioConversionMasa _servicioConversion;

    public ControladorConversionMasa(ServicioConversionMasa servicioConversion)
    {
        _servicioConversion = servicioConversion;
    }

    /// <summary>POST /api/conversion/masa</summary>
    [HttpPost("masa")]
    [ProducesResponseType(typeof(RespuestaConversion), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespuestaError), StatusCodes.Status401Unauthorized)]
    public IActionResult Convertir([FromBody] SolicitudConversion solicitud)
    {
        var resultado = _servicioConversion.Convertir(solicitud ?? new SolicitudConversion());

        if (!resultado.Exitoso)
        {
            return StatusCode(resultado.Error!.Codigo, resultado.Error);
        }

        return Ok(resultado.Valor);
    }
}
