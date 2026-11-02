using Microsoft.AspNetCore.Mvc;
using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Servicio;

namespace SISTEMA_BANQUITO.Controlador;

/// <summary>
/// CRUD de creditos. El otorgamiento vive en OtorgarCreditoControlador.
/// </summary>
[Controller]
[ApiController]
[Route("api/creditos")]
public sealed class CreditoControlador : ControllerBase
{
    private readonly CreditoServicio _creditoServicio;

    public CreditoControlador(CreditoServicio creditoServicio)
    {
        _creditoServicio = creditoServicio;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RespuestaCredito>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarAsync(CancellationToken cancelacion)
    {
        var creditos = await _creditoServicio.ListarAsync(cancelacion);
        return Ok(creditos);
    }

    [HttpPost("buscar")]
    [ProducesResponseType(typeof(RespuestaCredito), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarAsync(
        [FromBody] SolicitudBusquedaPorInt solicitud,
        CancellationToken cancelacion)
    {
        var credito = await _creditoServicio.ObtenerPorNumeroAsync(
            solicitud.Valor, cancelacion);

        if (credito is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El credito {solicitud.Valor} no existe.");
        }

        return Ok(credito);
    }

    [HttpPost("por-cuenta")]
    [ProducesResponseType(typeof(List<RespuestaCredito>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListarPorCuentaAsync(
        [FromBody] SolicitudBusquedaPorString solicitud,
        CancellationToken cancelacion)
    {
        var creditos = await _creditoServicio.ListarPorCuentaAsync(
            solicitud.Valor, cancelacion);

        return Ok(creditos);
    }

    [HttpPost("por-cliente")]
    [ProducesResponseType(typeof(List<RespuestaCredito>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListarPorClienteAsync(
        [FromBody] SolicitudBusquedaPorInt solicitud,
        CancellationToken cancelacion)
    {
        var creditos = await _creditoServicio.ListarPorClienteAsync(
            solicitud.Valor, cancelacion);

        return Ok(creditos);
    }

    [HttpPut("{numCredito:int}")]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarAsync(
        int numCredito,
        [FromBody] SolicitudActualizarCredito solicitud,
        CancellationToken cancelacion)
    {
        var respuesta = await _creditoServicio.ActualizarAsync(
            numCredito, solicitud, cancelacion);

        return Ok(respuesta);
    }

    [HttpDelete("{numCredito:int}")]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarAsync(
        int numCredito,
        CancellationToken cancelacion)
    {
        var respuesta = await _creditoServicio.EliminarAsync(
            numCredito, cancelacion);

        return Ok(respuesta);
    }
}
