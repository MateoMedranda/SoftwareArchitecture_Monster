using Microsoft.AspNetCore.Mvc;
using SISTEMA_FACTURACION.Modelos.DTO;

namespace SISTEMA_FACTURACION.Comun;
public sealed class ManejadorErrores(RequestDelegate siguiente, ILogger<ManejadorErrores> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await siguiente(context); }
        catch (ExcepcionNegocio ex)
        {
            context.Response.StatusCode=ex.StatusCode;
            await context.Response.WriteAsJsonAsync(new RespuestaGenericaDTO{Exitoso=false,Mensaje=ex.Message});
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex,"No se pudo comunicar con BanQuito"); context.Response.StatusCode=StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new RespuestaGenericaDTO{Exitoso=false,Mensaje="El servicio BanQuito no está disponible."});
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"Error no controlado en facturación"); context.Response.StatusCode=StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new RespuestaGenericaDTO{Exitoso=false,Mensaje="Error interno del sistema."});
        }
    }
}
