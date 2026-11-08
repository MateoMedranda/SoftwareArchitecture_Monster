using Microsoft.Extensions.Logging;

namespace MonsterConversorMovil;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<Services.ISoapConversionService, Services.SoapConversionService>();
		builder.Services.AddSingleton<Services.AppSession>();
		builder.Services.AddSingleton<Controllers.AccountController>();
		builder.Services.AddSingleton<Controllers.ConversionController>();
		builder.Services.AddSingleton<AppShell>();
		builder.Services.AddTransient<Views.LoginPage>();
		builder.Services.AddTransient<Views.ConverterPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
