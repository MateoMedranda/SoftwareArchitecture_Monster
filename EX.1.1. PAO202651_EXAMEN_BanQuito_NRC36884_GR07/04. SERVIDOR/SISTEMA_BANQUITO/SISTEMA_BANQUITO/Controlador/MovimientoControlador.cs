using Microsoft.AspNetCore.Mvc;
using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Servicio;

namespace SISTEMA_BANQUITO.Controlador;

/// <summary>
/// CRUD de movimientos (depositos y retiros).
/// </summary>
[Controller]
[ApiController]
[Route("api/movimientos")]
public sealed class MovimientoControlador : ControllerBase
{
    private readonly MovimientoServicio _movimientoServicio;

    public MovimientoControlador(MovimientoServicio movimientoServicio)
    {
        _movimientoServicio = movimientoServicio;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RespuestaMovimiento>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarAsync(CancellationToken cancelacion)
    {
        var movimientos = await _movimientoServicio.ListarAsync(cancelacion);
        return Ok(movimientos);
    }

    [HttpPost("buscar")]
    [ProducesResponseType(typeof(RespuestaMovimiento), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarAsync(
        [FromBody] SolicitudBusquedaPorInt solicitud,
        CancellationToken cancelacion)
    {
        var movimiento = await _movimientoServicio.ObtenerPorCodigoAsync(
            solicitud.Valor, cancelacion);

        if (movimiento is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El movimiento {solicitud.Valor} no existe.");
        }

        return Ok(movimiento);
    }

    [HttpPost("por-cuenta")]
    [ProducesResponseType(typeof(List<RespuestaMovimiento>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListarPorCuentaAsync(
        [FromBody] SolicitudBusquedaPorString solicitud,
        CancellationToken cancelacion)
    {
        var movimientos = await _movimientoServicio.ListarPorCuentaAsync(
            solicitud.Valor, cancelacion);

        return Ok(movimientos);
    }

    [HttpPost("por-cliente")]
    [ProducesResponseType(typeof(List<RespuestaMovimiento>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorClienteAsync(
        [FromBody] SolicitudBusquedaPorInt solicitud,
        CancellationToken cancelacion)
    {
        var movimientos = await _movimientoServicio.ListarPorClienteAsync(
            solicitud.Valor, cancelacion);

        return Ok(movimientos);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RespuestaMovimiento), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CrearAsync(
        [FromBody] SolicitudMovimiento solicitud,
        CancellationToken cancelacion)
    {
        var respuesta = await _movimientoServicio.CrearAsync(
            solicitud, cancelacion);

        return Ok(respuesta);
    }

    [HttpPut("{codigo:int}")]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarAsync(
        int codigo,
        [FromBody] SolicitudMovimiento solicitud,
        CancellationToken cancelacion)
    {
        var respuesta = await _movimientoServicio.ActualizarAsync(
            codigo, solicitud, cancelacion);

        return Ok(respuesta);
    }

    [HttpDelete("{codigo:int}")]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EliminarAsync(
        int codigo,
        CancellationToken cancelacion)
    {
        var respuesta = await _movimientoServicio.EliminarAsync(
            codigo, cancelacion);

        return Ok(respuesta);
    }
}
