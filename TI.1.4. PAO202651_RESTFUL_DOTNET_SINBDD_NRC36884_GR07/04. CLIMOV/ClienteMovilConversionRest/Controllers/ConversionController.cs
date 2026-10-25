using System.Globalization;
using ClienteMovilConversionRest.Models;
using ClienteMovilConversionRest.Services;

namespace ClienteMovilConversionRest.Controllers;

public sealed class ConversionController
{
    private readonly IRestConversionService restService;
    private readonly AppSession session;
    private readonly IReadOnlyDictionary<string, IReadOnlyList<UnitOption>> unitsByCategory;

    public IReadOnlyList<CategoryOption> Categories { get; } =
    [
        new("Temperatura", "temperatura", "°", "Celsius, Fahrenheit, Kelvin y Rankine"),
        new("Longitud", "longitud", "↔", "Centímetro, metro, kilómetro, yarda, pie y milla"),
        new("Masa", "masa", "⚖", "Miligramo, gramo, kilogramo, onza y tonelada")
    ];

    public string UserName => session.UserName;

    public ConversionController(IRestConversionService restService, AppSession session)
    {
        this.restService = restService;
        this.session = session;
        unitsByCategory = new Dictionary<string, IReadOnlyList<UnitOption>>(StringComparer.Ordinal)
        {
            ["Temperatura"] = [new("Celsius", "°C"), new("Fahrenheit", "°F"), new("Kelvin", "K"), new("Rankine", "°R")],
            ["Longitud"] = [new("Centimetro", "cm"), new("Metro", "m"), new("Yarda", "yd"), new("Pie", "ft"), new("Milla", "mi")],
            ["Masa"] = [new("Miligramo", "mg"), new("Gramo", "g"), new("Kilogramo", "kg"), new("Onza", "oz"), new("Tonelada", "t")]
        };
    }

    public IReadOnlyList<UnitOption> GetUnits(CategoryOption? category) =>
        category is not null && unitsByCategory.TryGetValue(category.Name, out var units) ? units : Array.Empty<UnitOption>();

    public async Task<ConversionResponse> ConvertAsync(string? rawValue, CategoryOption? category, UnitOption? origin, UnitOption? destination, CancellationToken cancellationToken = default)
    {
        if (!session.IsAuthenticated)
            return new ConversionResponse { Message = "Tu sesión expiró. Inicia sesión nuevamente." };

        if (!double.TryParse(rawValue, NumberStyles.Float, CultureInfo.CurrentCulture, out var value) &&
            !double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            return new ConversionResponse { Message = "Escribe un valor numérico válido." };

        if (category is null || origin is null || destination is null)
            return new ConversionResponse { Message = "Selecciona una categoría y las unidades de origen y destino." };

        return await restService.ConvertAsync(new ConversionRequest
        {
            Token = session.Token,
            CategoryEndpoint = category.Endpoint,
            SourceUnit = origin.Name,
            TargetUnit = destination.Name,
            Value = value
        }, cancellationToken);
    }

    public void SignOut() => session.Clear();
}
