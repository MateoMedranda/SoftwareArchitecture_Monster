using Microsoft.Extensions.Logging;
using ClienteMovilConversionRest.Controllers;
using ClienteMovilConversionRest.Services;
using ClienteMovilConversionRest.Views;

namespace ClienteMovilConversionRest;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IRestConversionService, RestConversionService>();
        builder.Services.AddSingleton<AppSession>();
        builder.Services.AddSingleton<AccountController>();
        builder.Services.AddSingleton<ConversionController>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ConverterPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
