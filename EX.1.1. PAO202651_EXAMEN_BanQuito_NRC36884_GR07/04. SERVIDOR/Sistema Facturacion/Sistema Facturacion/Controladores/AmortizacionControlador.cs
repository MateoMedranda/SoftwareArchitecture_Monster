using Microsoft.AspNetCore.Mvc;
using SISTEMA_FACTURACION.Servicios;

namespace SISTEMA_FACTURACION.Controladores;

/// <summary>
/// Proxy de solo lectura hacia la tabla de amortizacion de BanQuito.
/// La solicitud se recibe en facturacion; este servicio no accede a la BD del banco.
/// </summary>
[ApiController, Route("api/amortizaciones")]
public sealed class AmortizacionControlador(AmortizacionProxyServicio servicio):ControllerBase
{
    [HttpGet("por-cedula/{cedula}")]
    public async Task<IActionResult> PorCedula(string cedula,CancellationToken ct)=>
        Ok(await servicio.ObtenerPorCedulaAsync(cedula,ct));
}
