using CLI_WEB_RESTFUL_DOTNET.Components;
using CLI_WEB_RESTFUL_DOTNET.Controladores;
using CLI_WEB_RESTFUL_DOTNET.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Los servicios configuran su HttpClient a partir de .development.env.
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAutenticacionServicio, AutenticacionServicio>();
builder.Services.AddScoped<IConversionServicio, ConversionServicio>();

builder.Services.AddScoped<SesionUsuario>();
builder.Services.AddScoped<LoginController>();
builder.Services.AddScoped<ConversionController>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
