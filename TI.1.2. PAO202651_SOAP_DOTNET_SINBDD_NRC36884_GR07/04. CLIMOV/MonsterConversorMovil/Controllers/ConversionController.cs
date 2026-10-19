using MonsterConversorMovil.Models;
using MonsterConversorMovil.Services;

namespace MonsterConversorMovil.Controllers;

public sealed class ConversionController
{
    private readonly ISoapConversionService soapService;
    private readonly AppSession session;

    public IReadOnlyList<UnitOption> TemperatureUnits { get; } =
    [
        new("Celsius", "°C"),
        new("Fahrenheit", "°F"),
        new("Kelvin", "K"),
        new("Rankine", "°R")
    ];

    public string UserName => session.UserName;

    public ConversionController(ISoapConversionService soapService, AppSession session)
    {
        this.soapService = soapService;
        this.session = session;
    }

    public async Task<ConversionResponse> ConvertTemperatureAsync(string? rawValue, UnitOption? origin, UnitOption? destination, CancellationToken cancellationToken = default)
    {
        if (!session.IsAuthenticated)
        {
            return new ConversionResponse { Message = "Tu sesión expiró. Inicia sesión nuevamente." };
        }

        if (!double.TryParse(rawValue, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out var value) &&
            !double.TryParse(rawValue, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value))
        {
            return new ConversionResponse { Message = "Escribe un valor numérico válido." };
        }

        if (origin is null || destination is null)
        {
            return new ConversionResponse { Message = "Selecciona las unidades de origen y destino." };
        }

        return await soapService.ConvertAsync(new ConversionRequest
        {
            Token = session.Token,
            SourceUnit = origin.Name,
            TargetUnit = destination.Name,
            Value = value
        }, cancellationToken);
    }

    public void SignOut() => session.Clear();
}
