using Microsoft.AspNetCore.Mvc;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Servicio;

namespace SISTEMA_BANQUITO.Controlador;

/// <summary>
/// Endpoint para verificar si un cliente es sujeto de credito.
/// </summary>
[Controller]
[ApiController]
[Route("api/sujeto-credito")]
public sealed class SujetoCreditoControlador : ControllerBase
{
    private readonly ClienteServicio _clienteServicio;

    public SujetoCreditoControlador(ClienteServicio clienteServicio)
    {
        _clienteServicio = clienteServicio;
    }

    [HttpPost("verificar")]
    [ProducesResponseType(typeof(RespuestaSujetoCreditoDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerificarAsync(
        [FromBody] SolicitudSujetoCreditoDTO solicitud,
        CancellationToken cancelacion)
    {
        var respuesta = await _clienteServicio
            .VerificarSujetoCreditoAsync(solicitud, cancelacion);

        return Ok(respuesta);
    }
}
