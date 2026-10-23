using CLI_WEB_SOAP_DOTNET.Components;
using CLI_WEB_SOAP_DOTNET.Controladores;
using CLI_WEB_SOAP_DOTNET.Servicios;
using CLI_WEB_SOAP_DOTNET.Servicios.Soap;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configuración del endpoint SOAP (appsettings.json -> "ServicioSoap")
builder.Services.Configure<OpcionesServicioSoap>(
    builder.Configuration.GetSection(OpcionesServicioSoap.Seccion));

// Capa de acceso al servicio SOAP (singleton: no guarda estado)
builder.Services.AddSingleton<IClienteConversionUnidades, ClienteConversionUnidades>();

// Estado de sesión del usuario (uno por circuito de Blazor Server)
builder.Services.AddScoped<SesionUsuario>();

// Servicio de negocio + controladores (la "C" de MVC)
builder.Services.AddScoped<IAutenticacionServicio, AutenticacionServicio>();
builder.Services.AddScoped<LoginController>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
