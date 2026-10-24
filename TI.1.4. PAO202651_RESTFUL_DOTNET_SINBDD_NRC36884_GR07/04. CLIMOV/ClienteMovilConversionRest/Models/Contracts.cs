namespace ClienteMovilConversionRest.Models;

public sealed class LoginRequest
{
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public sealed class LoginResponse
{
    public bool IsSuccessful { get; init; }
    public string? Token { get; init; }
    public string Message { get; init; } = string.Empty;
}

public sealed class ConversionRequest
{
    public string Token { get; init; } = string.Empty;
    public string CategoryEndpoint { get; init; } = string.Empty;
    public string SourceUnit { get; init; } = string.Empty;
    public string TargetUnit { get; init; } = string.Empty;
    public double Value { get; init; }
}

public sealed class ConversionResponse
{
    public bool IsSuccessful { get; init; }
    public double ConvertedValue { get; init; }
    public string Message { get; init; } = string.Empty;
}

public sealed class ApiErrorResponse
{
    public int Codigo { get; init; }
    public string? Mensaje { get; init; }
    public string? Detalle { get; init; }
}

public sealed class ApiLoginResponse
{
    public string? Token { get; init; }
    public DateTime Expiracion { get; init; }
}

public sealed class ApiConversionResponse
{
    public double ValorConvertido { get; init; }
    public string? UnidadOrigen { get; init; }
    public string? UnidadDestino { get; init; }
}

public sealed record UnitOption(string Name, string Symbol);
public sealed record CategoryOption(string Name, string Endpoint, string Icon, string Description);
