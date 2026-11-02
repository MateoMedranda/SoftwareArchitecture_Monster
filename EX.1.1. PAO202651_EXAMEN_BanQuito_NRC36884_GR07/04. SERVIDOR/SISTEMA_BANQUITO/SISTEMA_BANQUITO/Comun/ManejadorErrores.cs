using System.Text.Json;
using SISTEMA_BANQUITO.Modelo.DTO;

namespace SISTEMA_BANQUITO.Comun;

/// <summary>
/// Middleware que convierte las excepciones de negocio y los errores
/// inesperados en una respuesta JSON uniforme.
/// </summary>
public sealed class ManejadorErrores
{
    private static readonly JsonSerializerOptions Opciones = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _siguiente;

    public ManejadorErrores(RequestDelegate siguiente)
    {
        _siguiente = siguiente;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (ExcepcionNegocio excepcion)
        {
            await EscribirAsync(contexto, excepcion.Estado, excepcion.Message, contexto.Request.Method);
        }
        catch (Exception excepcion)
        {
            await EscribirAsync(
                contexto,
                StatusCodes.Status500InternalServerError,
                $"Ocurrio un error inesperado en el servidor: {excepcion.Message}",
                contexto.Request.Method);
        }
    }

    private static async Task EscribirAsync(
        HttpContext contexto,
        int estado,
        string mensaje,
        string proceso)
    {
        if (contexto.Response.HasStarted)
        {
            return;
        }

        contexto.Response.Clear();
        contexto.Response.StatusCode = estado;
        contexto.Response.ContentType = "application/json; charset=utf-8";

        var cuerpo = new RespuestaGenerica
        {
            Exitoso = false,
            Mensaje = mensaje,
            ProcesoHttp = proceso
        };

        await contexto.Response.WriteAsync(JsonSerializer.Serialize(cuerpo, Opciones));
    }
}
