using Microsoft.AspNetCore.Mvc;
using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Servicio;

namespace SISTEMA_BANQUITO.Controlador;

/// <summary>
/// Consulta de tablas de amortizacion y pago de cuotas.
/// El endpoint api/amortizacion/consultar se mantiene en TablaAmortizacionControlador.
/// </summary>
[Controller]
[ApiController]
[Route("api/amortizaciones")]
public sealed class AmortizacionControlador : ControllerBase
{
    private readonly AmortizacionServicio _amortizacionServicio;

    public AmortizacionControlador(AmortizacionServicio amortizacionServicio)
    {
        _amortizacionServicio = amortizacionServicio;
    }

    [HttpPost("tabla")]
    [ProducesResponseType(typeof(RespuestaTablaAmortizacionDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TablaAsync(
        [FromBody] SolicitudBusquedaPorInt solicitud,
        CancellationToken cancelacion)
    {
        var tabla = await _amortizacionServicio
            .ObtenerTablaAmortizacionPorCreditoAsync(solicitud.Valor, cancelacion);

        return Ok(tabla);
    }

    [HttpPost("tabla-por-cliente")]
    [ProducesResponseType(typeof(RespuestaTablaAmortizacionDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TablaPorClienteAsync(
        [FromBody] SolicitudBusquedaPorInt solicitud,
        CancellationToken cancelacion)
    {
        var tabla = await _amortizacionServicio
            .ObtenerTablaAmortizacionPorClienteAsync(solicitud.Valor, cancelacion);

        return Ok(tabla);
    }

    [HttpGet("{numCredito:int}/{numCuota:int}")]
    [ProducesResponseType(typeof(CuotaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerCuotaAsync(
        int numCredito,
        int numCuota,
        CancellationToken cancelacion)
    {
        var cuota = await _amortizacionServicio.ObtenerCuotaAsync(
            numCredito, numCuota, cancelacion);

        if (cuota is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"La cuota {numCuota} del credito {numCredito} no existe.");
        }

        return Ok(cuota);
    }

    [HttpPost("pagar")]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PagarAsync(
        [FromBody] SolicitudPagoCuota solicitud,
        CancellationToken cancelacion)
    {
        var respuesta = await _amortizacionServicio.PagarCuotaAsync(
            solicitud, cancelacion);

        return Ok(respuesta);
    }
}
