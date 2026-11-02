using Microsoft.AspNetCore.Mvc;
using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Servicio;

namespace SISTEMA_BANQUITO.Controlador;

/// <summary>
/// CRUD de clientes.
/// </summary>
[Controller]
[ApiController]
[Route("api/clientes")]
public sealed class ClienteControlador : ControllerBase
{
    private readonly ClienteServicio _clienteServicio;

    public ClienteControlador(ClienteServicio clienteServicio)
    {
        _clienteServicio = clienteServicio;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RespuestaCliente>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarAsync(CancellationToken cancelacion)
    {
        var clientes = await _clienteServicio.ListarAsync(cancelacion);
        return Ok(clientes);
    }

    [HttpPost("buscar-codigo")]
    [ProducesResponseType(typeof(RespuestaCliente), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarPorCodigoAsync(
        [FromBody] SolicitudBusquedaPorInt solicitud,
        CancellationToken cancelacion)
    {
        var cliente = await _clienteServicio.ObtenerPorCodigoAsync(
            solicitud.Valor, cancelacion);

        if (cliente is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El cliente {solicitud.Valor} no existe.");
        }

        return Ok(cliente);
    }

    [HttpPost("buscar-cedula")]
    [ProducesResponseType(typeof(RespuestaCliente), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarPorCedulaAsync(
        [FromBody] SolicitudBusquedaPorString solicitud,
        CancellationToken cancelacion)
    {
        var cliente = await _clienteServicio.ObtenerPorCedulaAsync(
            solicitud.Valor, cancelacion);

        if (cliente is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El cliente con cedula '{solicitud.Valor}' no existe.");
        }

        return Ok(cliente);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CrearAsync(
        [FromBody] SolicitudCliente solicitud,
        CancellationToken cancelacion)
    {
        var respuesta = await _clienteServicio.CrearAsync(solicitud, cancelacion);
        return Ok(respuesta);
    }

    [HttpPut("{codigo:int}")]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaGenerica), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarAsync(
        int codigo,
        [FromBody] SolicitudCliente solicitud,
        CancellationToken cancelacion)
    {
        var respuesta = await _clienteServicio.ActualizarAsync(
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
        var respuesta = await _clienteServicio.EliminarAsync(codigo, cancelacion);
        return Ok(respuesta);
    }
}
