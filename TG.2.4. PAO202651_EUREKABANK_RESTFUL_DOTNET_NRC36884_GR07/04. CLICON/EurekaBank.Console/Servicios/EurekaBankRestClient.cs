using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using EurekaBank.Contracts;

namespace EurekaBank.Client;

/// <summary>Cliente HTTP de la API REST v1 usado por el controlador MVC.</summary>
public sealed class EurekaBankRestClient(string? endpoint = null) : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient http = new()
    {
        BaseAddress = new Uri(BuildEndpoint(endpoint)),
        Timeout = TimeSpan.FromSeconds(15)
    };

    public ServiceResult<LoginDto> Login(string user, string password) =>
        Send<LoginDto>(Post("auth/sessions", new LoginRequest
        {
            UserName = user,
            Password = password
        }));

    public ServiceResult<bool> Logout(string token) =>
        Send<bool>(Delete("auth/sessions/current", token));

    public ServiceResult<List<AccountDto>> Accounts(string token) =>
        Send<List<AccountDto>>(Get("accounts", token));

    public ServiceResult<ProfileDto> Profile(string token) =>
        Send<ProfileDto>(Get("profile", token));

    public ServiceResult<List<CustomerDto>> Customers(string token) =>
        Send<List<CustomerDto>>(Get("customers", token));

    public ServiceResult<List<MovementDto>> Movements(string token, string number) =>
        Send<List<MovementDto>>(Get($"accounts/{Uri.EscapeDataString(number)}/movements", token));

    public ServiceResult<MovementDto> Movement(string token, string number, string type, decimal amount) =>
        Send<MovementDto>(Post($"accounts/{Uri.EscapeDataString(number)}/movements", new { type, amount }, token));

    public ServiceResult<List<MovementDto>> Transfer(string token, string origin, string destination, decimal amount) =>
        Send<List<MovementDto>>(Post("transfers", new
        {
            sourceNumber = origin,
            destinationNumber = destination,
            amount
        }, token));

    public ServiceResult<MovementDto> Reverse(string token, string id) =>
        Send<MovementDto>(Post($"movements/{Uri.EscapeDataString(id)}/reversals", null, token));

    public ServiceResult<AccountDto> CreateAccount(string token, string customerId) =>
        Send<AccountDto>(Post("accounts", new { customerId }, token));

    private ServiceResult<T> Send<T>(HttpRequestMessage request)
    {
        using (request)
        using (var response = http.SendAsync(request).GetAwaiter().GetResult())
        {
            var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<ServiceResult<T>>(body, JsonOptions)
                ?? ServiceResult<T>.Fail("HTTP", $"El servidor respondió con {(int)response.StatusCode}.");
        }
    }

    private static HttpRequestMessage Get(string path, string token) =>
        Request(HttpMethod.Get, path, token);

    private static HttpRequestMessage Delete(string path, string token) =>
        Request(HttpMethod.Delete, path, token);

    private static HttpRequestMessage Post(string path, object? body, string? token = null)
    {
        var request = Request(HttpMethod.Post, path, token);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        return request;
    }

    private static HttpRequestMessage Request(HttpMethod method, string path, string? token = null)
    {
        var request = new HttpRequestMessage(method, path);
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return request;
    }

    private static string BuildEndpoint(string? endpoint)
    {
        var configuredEndpoint = endpoint
            ?? Environment.GetEnvironmentVariable("EUREKABANK_REST_URL")
            ?? "http://localhost:62558/api/v1";

        return configuredEndpoint.TrimEnd('/') + "/";
    }

    public void Dispose() => http.Dispose();
}
