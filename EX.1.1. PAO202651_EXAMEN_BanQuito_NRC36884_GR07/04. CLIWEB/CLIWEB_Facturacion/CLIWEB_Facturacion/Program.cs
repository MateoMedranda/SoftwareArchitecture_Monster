using CLIWEB_Facturacion.Config;
using CLIWEB_Facturacion.Controladores;
using CLIWEB_Facturacion.Modelos.DAO;
using CLIWEB_Facturacion.Servicios;
using CLIWEB_Facturacion.Vistas;

var builder = WebApplication.CreateBuilder(args);

// Lee el .env del proyecto para conocer el puerto del API de facturación.
AppConfig.Cargar(builder.Environment.ContentRootPath);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<SesionMock>();

// Un HttpClient por circuito con la URL base configurada en .env.
builder.Services.AddScoped(servicios => new HttpClient(new AutorizacionMockHandler(servicios.GetRequiredService<SesionMock>())
{
    InnerHandler = new HttpClientHandler()
})
{
    BaseAddress = new Uri(AppConfig.ApiBaseUrl),
    Timeout = TimeSpan.FromSeconds(AppConfig.TimeoutSegundos)
});

// DAOs: acceso a datos vía REST.
builder.Services.AddScoped<ClienteDao>();
builder.Services.AddScoped<ProductoDao>();
builder.Services.AddScoped<FacturaDao>();
builder.Services.AddScoped<AmortizacionDao>();

// Servicios: lógica del cliente.
builder.Services.AddScoped<ClientesServicio>();
builder.Services.AddScoped<CatalogoServicio>();
builder.Services.AddScoped<FacturacionServicio>();

// Controladores: estado de cada pantalla.
builder.Services.AddScoped<ClientesControlador>();
builder.Services.AddScoped<CatalogoControlador>();
builder.Services.AddScoped<CompraControlador>();
builder.Services.AddScoped<FacturasControlador>();
builder.Services.AddScoped<CreditosControlador>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
