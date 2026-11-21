using EurekaBank.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EurekaBank.Server.Controladores;

/// <summary>
/// Fachada REST v1 que delega las reglas de negocio, auditoría y datos
/// a la capa de aplicación.
/// </summary>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public sealed class BankApiController(EurekaBankService bank) : ControllerBase
{
    [HttpPost("auth/sessions")]
    [ProducesResponseType(typeof(ServiceResult<LoginDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<LoginDto>), StatusCodes.Status401Unauthorized)]
    public ActionResult<ServiceResult<LoginDto>> Login([FromBody] LoginRequest request) => Respond(bank.Login(request));

    [HttpDelete("auth/sessions/current")]
    [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status401Unauthorized)]
    public ActionResult<ServiceResult<bool>> Logout() => Respond(bank.Logout(new TokenRequest { Token = Token }));

    [HttpGet("accounts")]
    [ProducesResponseType(typeof(ServiceResult<List<AccountDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<AccountDto>>), StatusCodes.Status401Unauthorized)]
    public ActionResult<ServiceResult<List<AccountDto>>> Accounts() => Respond(bank.GetAccounts(new TokenRequest { Token = Token }));

    [HttpGet("profile")]
    [ProducesResponseType(typeof(ServiceResult<ProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<ProfileDto>), StatusCodes.Status401Unauthorized)]
    public ActionResult<ServiceResult<ProfileDto>> Profile() => Respond(bank.GetProfile(new TokenRequest { Token = Token }));

    [HttpGet("customers")]
    [ProducesResponseType(typeof(ServiceResult<List<CustomerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<CustomerDto>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResult<List<CustomerDto>>), StatusCodes.Status403Forbidden)]
    public ActionResult<ServiceResult<List<CustomerDto>>> Customers() => Respond(bank.GetCustomers(new TokenRequest { Token = Token }));

    [HttpGet("accounts/{accountNumber}/movements")]
    [ProducesResponseType(typeof(ServiceResult<List<MovementDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<MovementDto>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResult<List<MovementDto>>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResult<List<MovementDto>>), StatusCodes.Status404NotFound)]
    public ActionResult<ServiceResult<List<MovementDto>>> Movements(string accountNumber) =>
        Respond(bank.GetMovements(new QueryRequest { Token = Token, AccountNumber = accountNumber }));

    [HttpPost("accounts/{accountNumber}/movements")]
    [ProducesResponseType(typeof(ServiceResult<MovementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<MovementDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<MovementDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResult<MovementDto>), StatusCodes.Status403Forbidden)]
    public ActionResult<ServiceResult<MovementDto>> RegisterMovement(string accountNumber, [FromBody] MovementInput request) =>
        Respond(bank.RegisterMovement(new MoneyRequest { Token = Token, AccountNumber = accountNumber, Type = request.Type, Amount = request.Amount }));

    [HttpPost("transfers")]
    [ProducesResponseType(typeof(ServiceResult<List<MovementDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<MovementDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<List<MovementDto>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResult<List<MovementDto>>), StatusCodes.Status403Forbidden)]
    public ActionResult<ServiceResult<List<MovementDto>>> Transfer([FromBody] TransferInput request) =>
        Respond(bank.Transfer(new TransferRequest { Token = Token, SourceNumber = request.SourceNumber, DestinationNumber = request.DestinationNumber, Amount = request.Amount }));

    [HttpPost("movements/{movementId}/reversals")]
    [ProducesResponseType(typeof(ServiceResult<MovementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<MovementDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResult<MovementDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResult<MovementDto>), StatusCodes.Status404NotFound)]
    public ActionResult<ServiceResult<MovementDto>> Reverse(string movementId) =>
        Respond(bank.Reverse(new ReverseRequest { Token = Token, MovementId = movementId }));

    [HttpPost("accounts")]
    [ProducesResponseType(typeof(ServiceResult<AccountDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<AccountDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResult<AccountDto>), StatusCodes.Status403Forbidden)]
    public ActionResult<ServiceResult<AccountDto>> CreateAccount([FromBody] CreateAccountInput request)
    {
        var result = bank.CreateAccount(new CreateAccountRequest { Token = Token, CustomerId = request.CustomerId });
        return result.Ok && result.Data is not null
            ? Created($"api/v1/accounts/{result.Data.Number}", result)
            : Respond(result);
    }

    private string Token
    {
        get
        {
            var authorization = Request.Headers.Authorization.ToString();
            return authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authorization[7..].Trim()
                : string.Empty;
        }
    }

    private ActionResult<ServiceResult<T>> Respond<T>(ServiceResult<T> result) => result.Code switch
    {
        "AUTH" => Unauthorized(result),
        "FORBIDDEN" => StatusCode(StatusCodes.Status403Forbidden, result),
        "NOT_FOUND" => NotFound(result),
        "VALIDATION" or "FUNDS" or "ACCOUNT" => BadRequest(result),
        _ => Ok(result)
    };
}

public sealed class MovementInput { public string Type { get; set; } = ""; public decimal Amount { get; set; } }
public sealed class TransferInput { public string SourceNumber { get; set; } = ""; public string DestinationNumber { get; set; } = ""; public decimal Amount { get; set; } }
public sealed class CreateAccountInput { public string CustomerId { get; set; } = ""; }
