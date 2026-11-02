using Microsoft.AspNetCore.Mvc;
using SISTEMA_FACTURACION.Modelos.DTO;
using SISTEMA_FACTURACION.Servicios;

namespace SISTEMA_FACTURACION.Controladores;
[ApiController, Route("api/clientes")]
public sealed class ClienteControlador(ClienteServicio servicio):ControllerBase
{
    [HttpGet] public async Task<IActionResult> Listar(CancellationToken ct)=>Ok(await servicio.ListarAsync(ct));
    [HttpGet("{codigo:int}")] public async Task<IActionResult> Buscar(int codigo,CancellationToken ct)=>Ok(await servicio.BuscarAsync(codigo,ct));
    [HttpGet("por-cedula/{cedula}")] public async Task<IActionResult> BuscarCedula(string cedula,CancellationToken ct)=>Ok(await servicio.BuscarCedulaAsync(cedula,ct));
    [HttpPost] public async Task<IActionResult> Crear([FromBody]SolicitudClienteDTO s,CancellationToken ct){var r=await servicio.CrearAsync(s,ct);return StatusCode(201,r);}
    [HttpPut("{codigo:int}")] public async Task<IActionResult> Actualizar(int codigo,[FromBody]SolicitudClienteDTO s,CancellationToken ct)=>Ok(await servicio.ActualizarAsync(codigo,s,ct));
    [HttpDelete("{codigo:int}")] public async Task<IActionResult> Eliminar(int codigo,CancellationToken ct)=>Ok(await servicio.EliminarAsync(codigo,ct));
}
