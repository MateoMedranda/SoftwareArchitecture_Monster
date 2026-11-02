using Microsoft.AspNetCore.Mvc;
using SISTEMA_FACTURACION.Modelos.DTO;
using SISTEMA_FACTURACION.Servicios;

namespace SISTEMA_FACTURACION.Controladores;
[ApiController, Route("api/facturas")]
public sealed class FacturaControlador(FacturaServicio servicio):ControllerBase
{
    [HttpPost] public async Task<IActionResult> Crear([FromBody]SolicitudFacturaDTO s,CancellationToken ct){var factura=await servicio.CrearAsync(s,ct);return CreatedAtAction(nameof(Buscar),new{numero=factura.NumFactura},factura);}
    [HttpGet("{numero:int}")] public async Task<IActionResult> Buscar(int numero,CancellationToken ct)=>Ok(await servicio.BuscarAsync(numero,ct));
    [HttpGet("{numero:int}/items")] public async Task<IActionResult> Items(int numero,CancellationToken ct)
    {
        var factura=await servicio.BuscarAsync(numero,ct);
        return Ok(factura.Items);
    }
    [HttpGet] public async Task<IActionResult> Listar([FromQuery]int? codigoCliente,[FromQuery]string? cedula,CancellationToken ct)
    {
        if(codigoCliente is null&&string.IsNullOrWhiteSpace(cedula)) return Ok(await servicio.ListarAsync(ct));
        return Ok(await servicio.BuscarClienteAsync(codigoCliente,cedula,ct));
    }
}
