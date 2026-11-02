using Microsoft.AspNetCore.Mvc;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Servicio;

namespace SISTEMA_BANQUITO.Controlador;

/// <summary>
/// Endpoint para consultar la tabla de amortizacion de un credito.
/// </summary>
[Controller]
[ApiController]
[Route("api/amortizacion")]
public sealed class TablaAmortizacionControlador : ControllerBase
{
    private readonly AmortizacionServicio _amortizacionServicio;

    public TablaAmortizacionControlador(AmortizacionServicio amortizacionServicio)
    {
        _amortizacionServicio = amortizacionServicio;
    }

    [HttpPost("consultar")]
    [ProducesResponseType(typeof(RespuestaTablaAmortizacionDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarAsync(
        [FromBody] SolicitudTablaAmortizacionDTO solicitud,
        CancellationToken cancelacion)
    {
        var respuesta = await _amortizacionServicio
            .ObtenerTablaAmortizacionAsync(solicitud, cancelacion);

        return Ok(respuesta);
    }
}
