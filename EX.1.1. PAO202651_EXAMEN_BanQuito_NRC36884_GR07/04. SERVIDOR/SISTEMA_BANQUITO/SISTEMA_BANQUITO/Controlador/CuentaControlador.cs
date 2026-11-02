using Microsoft.AspNetCore.Mvc;
using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Servicio;

namespace SISTEMA_BANQUITO.Controlador;

/// <summary>
/// CRUD de cuentas.
/// </summary>
[Controller]
[ApiController]
[Route("api/cuentas")]
public sealed class CuentaControlador : ControllerBase
{
    private readonly CuentaServicio _cuentaServicio;

    public CuentaControlador(CuentaServicio cuentaServicio)
    {
        _cuentaServicio = cuentaServicio;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RespuestaCuenta>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarAsync(CancellationToken cancelacion)
    {
        var cuentas = await _cuentaServicio.ListarAsync(cancelacion);
        return Ok(cuentas);
    }

    [HttpPost("buscar")]
    [ProducesResponseType(typeof(RespuestaCuenta), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarAsync(
        [FromBody] SolicitudBusquedaPorString solicitud,
        CancellationToken cancelacion)
    {
        var cuenta = await _cuentaServicio.ObtenerPorNumeroAsync(
            solicitud.Valor, cancelacion);

        if (cuenta is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"La cuenta {solicitud.Valor} no existe.");
        }

        return Ok(cuenta);
    }

    [HttpPost("por-cliente")]
    [ProducesResponseType(typeof(List<RespuestaCuenta>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorClienteAsync(
        [FromBody] SolicitudBusquedaPorInt solicitud,
        CancellationToken cancelacion)
    {
        var cuentas = await _cuentaServicio.ListarPorClienteAsync(
            solicitud.Valor, cancelacion);

        return Ok(cuentas);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CrearAsync(
        [FromBody] SolicitudCuenta solicitud,
        CancellationToken cancelacion)
    {
        var respuesta = await _cuentaServicio.CrearAsync(solicitud, cancelacion);
        return Ok(respuesta);
    }

    [HttpPut("{numCuenta}")]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarAsync(
        string numCuenta,
        [FromBody] SolicitudCuenta solicitud,
        CancellationToken cancelacion)
    {
        var respuesta = await _cuentaServicio.ActualizarAsync(
            numCuenta, solicitud, cancelacion);

        return Ok(respuesta);
    }

    [HttpDelete("{numCuenta}")]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EliminarAsync(
        string numCuenta,
        CancellationToken cancelacion)
    {
        var respuesta = await _cuentaServicio.EliminarAsync(
            numCuenta, cancelacion);

        return Ok(respuesta);
    }
}
