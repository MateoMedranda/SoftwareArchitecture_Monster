using Microsoft.AspNetCore.Mvc;
using SISTEMA_BANQUITO.Modelo;
using SISTEMA_BANQUITO.Modelo.DTO;

namespace SISTEMA_BANQUITO.Controlador;

/// <summary>
/// Endpoints de diagnostico del servidor y de la conexion a SQL Server.
/// </summary>
[Controller]
[ApiController]
[Route("api/salud")]
public sealed class SaludControlador : ControllerBase
{
    private readonly Conexion _conexion;

    public SaludControlador(Conexion conexion)
    {
        _conexion = conexion;
    }

    [HttpGet]
    public IActionResult Estado()
    {
        return Ok(new
        {
            servicio = "SISTEMA_BANQUITO",
            estado = "activo",
            fecha = DateTime.UtcNow
        });
    }

    [HttpGet("conexion")]
    [ProducesResponseType(typeof(RespuestaSaludDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaSaludDTO), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ConexionAsync(CancellationToken cancelacion)
    {
        try
        {
            var conectado = await _conexion.ProbarConexionAsync(cancelacion);

            return Ok(new RespuestaSaludDTO
            {
                Conectado = conectado,
                BaseDatos = _conexion.NombreBaseDatos
            });
        }
        catch (Exception excepcion)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new RespuestaSaludDTO
                {
                    Conectado = false,
                    BaseDatos = _conexion.NombreBaseDatos,
                    Detalle = excepcion.Message
                });
        }
    }
}
