using MonsterConversorMovil.Models;

namespace MonsterConversorMovil.Services;

public interface ISoapConversionService
{
    string Endpoint { get; set; }
    Task<LoginResponse> SignInAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<ConversionResponse> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken = default);
}
