using Microsoft.AspNetCore.Mvc;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Servicio;

namespace SISTEMA_BANQUITO.Controlador;

/// <summary>
/// Endpoint para otorgar un credito por la compra de un telefono.
/// </summary>
[Controller]
[ApiController]
[Route("api/credito")]
public sealed class OtorgarCreditoControlador : ControllerBase
{
    private readonly CreditoServicio _creditoServicio;

    public OtorgarCreditoControlador(CreditoServicio creditoServicio)
    {
        _creditoServicio = creditoServicio;
    }

    [HttpPost("otorgar")]
    [ProducesResponseType(typeof(RespuestaOtorgarCreditoDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> OtorgarAsync(
        [FromBody] SolicitudOtorgarCreditoDTO solicitud,
        CancellationToken cancelacion)
    {
        var respuesta = await _creditoServicio
            .OtorgarCreditoAsync(solicitud, cancelacion);

        return Ok(respuesta);
    }
}
