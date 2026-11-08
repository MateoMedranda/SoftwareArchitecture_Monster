namespace MonsterConversorMovil.Models;

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
    public string Category { get; init; } = "Temperatura";
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

public sealed record UnitOption(string Name, string Symbol);

public sealed record CategoryOption(string Name, string Icon, string Description);
