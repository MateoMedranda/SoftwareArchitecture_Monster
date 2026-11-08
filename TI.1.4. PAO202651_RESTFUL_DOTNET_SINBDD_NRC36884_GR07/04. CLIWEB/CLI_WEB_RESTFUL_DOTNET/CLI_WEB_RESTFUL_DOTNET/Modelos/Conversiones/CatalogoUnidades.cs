namespace CLI_WEB_RESTFUL_DOTNET.Modelos.Conversiones;

public record UnidadInfo(string UnidadTag, string FormLabel, string Abbr);

public static class CatalogoUnidades
{
    public static IReadOnlyList<string> Categorias { get; } = new[] { "Longitud", "Masa", "Temperatura" };

    private static readonly Dictionary<string, List<UnidadInfo>> _unidades = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Longitud"] = new()
        {
            new("Metro", "Metros", "m"),
            new("Pie", "Pies", "ft"),
            new("Centimetro", "Centímetros", "cm"),
            new("Milla", "Millas", "mi"),
            new("Yarda", "Yardas", "yd")
        },
        ["Masa"] = new()
        {
            new("Kilogramo", "Kilogramos", "kg"),
            new("Gramo", "Gramos", "g"),
            new("Tonelada", "Toneladas", "t"),
            new("Onza", "Onzas", "oz")
        },
        ["Temperatura"] = new()
        {
            new("Celsius", "Celsius (°C)", "°C"),
            new("Fahrenheit", "Fahrenheit (°F)", "°F"),
            new("Kelvin", "Kelvin (K)", "K"),
            new("Rankine", "Rankine (°R)", "°R")
        }
    };

    public static IReadOnlyList<UnidadInfo> UnidadesDe(string categoria) =>
        _unidades.TryGetValue(categoria, out var lista) ? lista : Array.Empty<UnidadInfo>();
}
