using ClienteMovilConversionRest.Models;

namespace ClienteMovilConversionRest.Services;

public interface IRestConversionService
{
    string Endpoint { get; set; }
    Task<LoginResponse> SignInAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<ConversionResponse> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken = default);
}
