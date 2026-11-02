using Microsoft.AspNetCore.Mvc;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Servicio;

namespace SISTEMA_BANQUITO.Controlador;

/// <summary>
/// Endpoint para calcular el monto maximo de credito de un cliente.
/// </summary>
[Controller]
[ApiController]
[Route("api/monto-maximo")]
public sealed class MontoMaximoControlador : ControllerBase
{
    private readonly CreditoServicio _creditoServicio;

    public MontoMaximoControlador(CreditoServicio creditoServicio)
    {
        _creditoServicio = creditoServicio;
    }

    [HttpPost("calcular")]
    [ProducesResponseType(typeof(RespuestaMontoMaximoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CalcularAsync(
        [FromBody] SolicitudMontoMaximoDTO solicitud,
        CancellationToken cancelacion)
    {
        var respuesta = await _creditoServicio
            .ObtenerMontoMaximoAsync(solicitud, cancelacion);

        return Ok(respuesta);
    }
}
