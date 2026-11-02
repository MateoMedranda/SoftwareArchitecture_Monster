using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo;
using SISTEMA_BANQUITO.Modelo.DAO;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Servicio;

var builder = WebApplication.CreateBuilder(args);

// Controladores + JSON en camelCase para los DTO (sujetoCredito, codCliente, etc.)
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(opciones =>
    {
        // Los errores de validacion de los DTO tambien usan RespuestaGenerica.
        opciones.InvalidModelStateResponseFactory = contexto =>
        {
            var mensajes = contexto.ModelState.Values
                .SelectMany(valor => valor.Errors)
                .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "Solicitud invalida."
                    : error.ErrorMessage)
                .Distinct();

            var respuesta = new RespuestaGenerica
            {
                Exitoso = false,
                Mensaje = string.Join(" ", mensajes),
                ProcesoHttp = contexto.HttpContext.Request.Method
            };

            return new BadRequestObjectResult(respuesta);
        };
    })
    .AddJsonOptions(opciones =>
    {
        opciones.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        opciones.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        // Permite enviar el tipo de movimiento como "DEP" o "RET".
        opciones.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();

// Capa Modelo: conexion y acceso a datos (DAO)
builder.Services.AddSingleton<Conexion>();
builder.Services.AddScoped<ClienteDAO>();
builder.Services.AddScoped<CuentaDAO>();
builder.Services.AddScoped<MovimientoDAO>();
builder.Services.AddScoped<CreditoDAO>();
builder.Services.AddScoped<AmortizacionDAO>();

// Capa Servicio: reglas de negocio
builder.Services.AddScoped<ClienteServicio>();
builder.Services.AddScoped<CuentaServicio>();
builder.Services.AddScoped<MovimientoServicio>();
builder.Services.AddScoped<CreditoServicio>();
builder.Services.AddScoped<AmortizacionServicio>();

// CORS para que los clientes web/moviles puedan consumir la API
builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("PermitirClientes", politica =>
        politica.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Convierte las excepciones de negocio en respuestas RespuestaGenerica.
app.UseMiddleware<ManejadorErrores>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("PermitirClientes");
app.UseAuthorization();
app.MapControllers();

app.Run();
