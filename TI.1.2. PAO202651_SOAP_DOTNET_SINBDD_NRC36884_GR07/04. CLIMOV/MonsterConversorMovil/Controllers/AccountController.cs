using MonsterConversorMovil.Models;
using MonsterConversorMovil.Services;

namespace MonsterConversorMovil.Controllers;

public sealed class AccountController
{
    private readonly ISoapConversionService soapService;
    private readonly AppSession session;

    public AccountController(ISoapConversionService soapService, AppSession session)
    {
        this.soapService = soapService;
        this.session = session;
    }

    public string Endpoint
    {
        get => soapService.Endpoint;
        set => soapService.Endpoint = value;
    }

    public async Task<LoginResponse> SignInAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            return new LoginResponse { Message = "Ingresa tu usuario y contraseña." };
        }

        var response = await soapService.SignInAsync(new LoginRequest
        {
            UserName = userName.Trim(),
            Password = password
        }, cancellationToken);

        if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Token))
        {
            session.Start(response.Token, userName.Trim());
        }

        return response;
    }
}
