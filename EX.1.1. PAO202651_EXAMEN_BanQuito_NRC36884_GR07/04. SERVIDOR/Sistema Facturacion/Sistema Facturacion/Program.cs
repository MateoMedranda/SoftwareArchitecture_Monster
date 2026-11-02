using SISTEMA_FACTURACION.Modelos;
using SISTEMA_FACTURACION.Modelos.DAO;
using SISTEMA_FACTURACION.Servicios;
using SISTEMA_FACTURACION.Comun;

var builder = WebApplication.CreateBuilder(args);

// Importa el .development.env del servidor en ejecucion local. Las variables
// del proceso/sistema tienen prioridad sobre los valores del archivo.
if (builder.Environment.IsDevelopment())
{
    var archivoEnv = Path.GetFullPath(Path.Combine(
        builder.Environment.ContentRootPath,
        "..", "..", "..", ".development.env"));

    if (File.Exists(archivoEnv))
    {
        var variablesArchivo = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var linea in File.ReadLines(archivoEnv))
        {
            var contenido = linea.Trim();
            if (contenido.Length == 0 || contenido.StartsWith('#'))
            {
                continue;
            }

            var separador = contenido.IndexOf('=');
            if (separador <= 0)
            {
                continue;
            }

            var nombre = contenido[..separador].Trim();
            var valor = contenido[(separador + 1)..].Trim();
            if (valor.Length >= 2 &&
                ((valor[0] == '"' && valor[^1] == '"') ||
                 (valor[0] == '\'' && valor[^1] == '\'')))
            {
                valor = valor[1..^1];
            }

            if (Environment.GetEnvironmentVariable(nombre) is null)
            {
                variablesArchivo[nombre] = valor;
                if (nombre.Contains("__", StringComparison.Ordinal))
                    variablesArchivo[nombre.Replace("__", ":", StringComparison.Ordinal)] = valor;
            }
        }

        builder.Configuration.AddInMemoryCollection(
            variablesArchivo.Where(variable =>
                Environment.GetEnvironmentVariable(variable.Key) is null));
    }

    builder.Configuration.AddEnvironmentVariables();
}

var servidorDb = builder.Configuration["FACTURACION_DB_SERVER"] ?? "localhost,1433";
var nombreDb = builder.Configuration["FACTURACION_DB_NAME"] ?? "FACTURACION";
var usuarioDb = builder.Configuration["FACTURACION_DB_USER"] ?? "sa";
var contrasenaDb = builder.Configuration["FACTURACION_DB_PASSWORD"];
if (!string.IsNullOrWhiteSpace(contrasenaDb) && Environment.GetEnvironmentVariable("ConnectionStrings__Facturacion") is null)
{
    var cadenaDb = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder
    {
        DataSource = servidorDb,
        InitialCatalog = nombreDb,
        UserID = usuarioDb,
        Password = contrasenaDb,
        TrustServerCertificate = true,
        Encrypt = false
    };
    builder.Configuration["ConnectionStrings:Facturacion"] = cadenaDb.ConnectionString;
}

builder.Configuration["Servicios:Banquito:BaseUrl"] =
    builder.Configuration["BANQUITO_BASE_URL"] ??
    builder.Configuration["Servicios:Banquito:BaseUrl"];
if (int.TryParse(builder.Configuration["BANQUITO_TIMEOUT_SECONDS"], out var timeoutSegundos))
{
    builder.Configuration["Servicios:Banquito:TimeoutSegundos"] = timeoutSegundos.ToString();
}

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<Conexion>();
builder.Services.AddScoped<ClienteDAO>();
builder.Services.AddScoped<ProductoDAO>();
builder.Services.AddScoped<FacturaDAO>();
builder.Services.AddScoped<ClienteServicio>();
builder.Services.AddScoped<ProductoServicio>();
builder.Services.AddScoped<FacturaServicio>();
var banquitoBaseUrl = builder.Configuration["Servicios:Banquito:BaseUrl"] ?? "http://localhost:5173/";
var banquitoTimeout = int.TryParse(builder.Configuration["Servicios:Banquito:TimeoutSegundos"], out var timeout) ? timeout : 10;
builder.Services.AddHttpClient<BanquitoCliente>(http =>
{
    http.BaseAddress = new Uri(banquitoBaseUrl);
    http.Timeout = TimeSpan.FromSeconds(banquitoTimeout);
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ManejadorErrores>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
