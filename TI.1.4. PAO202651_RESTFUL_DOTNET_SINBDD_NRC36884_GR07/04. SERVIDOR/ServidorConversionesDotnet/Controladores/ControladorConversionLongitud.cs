using Microsoft.AspNetCore.Mvc;
using ServidorConversionesDotnet.Modelos;
using ServidorConversionesDotnet.Servicios;

namespace ServidorConversionesDotnet.Controladores;

/// <summary>
/// Endpoint de conversión de longitud (protegido por token).
/// </summary>
[Route("api/conversion")]
[ApiController]
public sealed class ControladorConversionLongitud : ControllerBase
{
    private readonly ServicioConversionLongitud _servicioConversion;

    public ControladorConversionLongitud(ServicioConversionLongitud servicioConversion)
    {
        _servicioConversion = servicioConversion;
    }

    /// <summary>POST /api/conversion/longitud</summary>
    [HttpPost("longitud")]
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
