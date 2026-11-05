using CLIMOV_Facturacion.Config;
using CLIMOV_Facturacion.Controladores;
using CLIMOV_Facturacion.Modelos.DAO;
using CLIMOV_Facturacion.Servicios;
using Microsoft.Extensions.Logging;

namespace CLIMOV_Facturacion;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<SesionMock>();

        builder.Services.AddScoped(servicios => new HttpClient(new DireccionServidorHandler(servicios.GetRequiredService<SesionMock>())
        {
            InnerHandler = new HttpClientHandler()
        })
        {
            // El handler reemplaza esta base por la dirección guardada antes de cada envío.
            BaseAddress = new Uri("http://localhost/"),
            Timeout = TimeSpan.FromSeconds(AppConfig.TimeoutSegundos)
        });

        builder.Services.AddScoped<ClienteDao>();
        builder.Services.AddScoped<ProductoDao>();
        builder.Services.AddScoped<FacturaDao>();
        builder.Services.AddScoped<AmortizacionDao>();

        builder.Services.AddScoped<ClientesServicio>();
        builder.Services.AddScoped<CatalogoServicio>();
        builder.Services.AddScoped<FacturacionServicio>();

        builder.Services.AddScoped<ClientesControlador>();
        builder.Services.AddScoped<CatalogoControlador>();
        builder.Services.AddScoped<CompraControlador>();
        builder.Services.AddScoped<FacturasControlador>();
        builder.Services.AddScoped<CreditosControlador>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        return builder.Build();
    }
}
