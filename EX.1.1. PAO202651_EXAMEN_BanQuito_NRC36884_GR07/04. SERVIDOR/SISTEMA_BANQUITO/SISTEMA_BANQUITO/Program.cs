using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo;
using SISTEMA_BANQUITO.Modelo.DAO;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Servicio;

var builder = WebApplication.CreateBuilder(args);

// Carga la configuracion compartida al ejecutar localmente; las variables
// del proceso/sistema prevalecen sobre el archivo.
if (builder.Environment.IsDevelopment())
{
    var archivoEnv = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", ".development.env"));
    if (File.Exists(archivoEnv))
    {
        var valores = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var linea in File.ReadLines(archivoEnv))
        {
            var contenido = linea.Trim();
            if (contenido.Length == 0 || contenido.StartsWith('#')) continue;
            var separador = contenido.IndexOf('=');
            if (separador <= 0) continue;
            var clave = contenido[..separador].Trim();
            var valor = contenido[(separador + 1)..].Trim();
            if (valor.Length >= 2 && ((valor[0] == '"' && valor[^1] == '"') || (valor[0] == '\'' && valor[^1] == '\''))) valor = valor[1..^1];
            if (Environment.GetEnvironmentVariable(clave) is null)
            {
                valores[clave] = valor;
                if (clave.Contains("__", StringComparison.Ordinal)) valores[clave.Replace("__", ":", StringComparison.Ordinal)] = valor;
            }
        }
        builder.Configuration.AddInMemoryCollection(valores);
    }
    builder.Configuration.AddEnvironmentVariables();
}

var passwordBanquito = builder.Configuration["BANQUITO_DB_PASSWORD"];
if (!string.IsNullOrWhiteSpace(passwordBanquito) && Environment.GetEnvironmentVariable("ConnectionStrings__Banquito") is null)
{
    builder.Configuration["ConnectionStrings:Banquito"] = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder
    {
        DataSource = builder.Configuration["BANQUITO_DB_SERVER"] ?? "localhost,1433",
        InitialCatalog = builder.Configuration["BANQUITO_DB_NAME"] ?? "BANQUITO",
        UserID = builder.Configuration["BANQUITO_DB_USER"] ?? "sa",
        Password = passwordBanquito,
        TrustServerCertificate = true,
        Encrypt = false
    }.ConnectionString;
}

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
