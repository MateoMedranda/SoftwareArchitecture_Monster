using ClienteMovilConversionRest.Models;
using ClienteMovilConversionRest.Services;

namespace ClienteMovilConversionRest.Controllers;

public sealed class AccountController
{
    private readonly IRestConversionService restService;
    private readonly AppSession session;

    public AccountController(IRestConversionService restService, AppSession session)
    {
        this.restService = restService;
        this.session = session;
    }

    public string Endpoint
    {
        get => restService.Endpoint;
        set => restService.Endpoint = value;
    }

    public async Task<LoginResponse> SignInAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            return new LoginResponse { Message = "Ingresa tu usuario y contraseña." };

        var response = await restService.SignInAsync(new LoginRequest
        {
            UserName = userName.Trim(),
            Password = password
        }, cancellationToken);

        if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Token))
            session.Start(response.Token, userName.Trim());

        return response;
    }
}
