using System.Security.Cryptography;
using System.Text;
using EurekaBank.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EurekaBank.Server;

/// <summary>
/// Capa de aplicación de EurekaBank. Centraliza reglas de negocio y persistencia;
/// los controladores REST no acceden directamente al contexto de datos.
/// </summary>
public sealed class EurekaBankService(EurekaDbContext db)
{
    public ServiceResult<LoginDto> Login(LoginRequest request)
    {
        var passwordHash = Hash(request.Password);
        var user = db.Users
            .Include(item => item.Role)
            .SingleOrDefault(item =>
                item.UserName == request.UserName && item.PasswordHash == passwordHash);

        if (user is null)
        {
            return ServiceResult<LoginDto>.Fail("AUTH", "Usuario o contraseña inválidos.");
        }

        var tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        db.Tokens.Add(new BankToken
        {
            Id = Guid.NewGuid().ToString(),
            Value = tokenValue,
            Status = "ACTIVO",
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();

        return ServiceResult<LoginDto>.Success(new LoginDto
        {
            Token = tokenValue,
            UserId = user.Id,
            Name = $"{user.FirstName} {user.LastName}".Trim(),
            Role = user.Role?.Name ?? "CLIENTE"
        }, "Sesión iniciada.");
    }

    public ServiceResult<bool> Logout(TokenRequest request)
    {
        var token = db.Tokens.SingleOrDefault(item =>
            item.Value == request.Token && item.Status == "ACTIVO");

        if (token is null)
        {
            return ServiceResult<bool>.Fail("AUTH", "Sesión inválida o expirada.");
        }

        token.Status = "REVOCADO";
        db.SaveChanges();
        return ServiceResult<bool>.Success(true, "Sesión cerrada.");
    }

    public ServiceResult<List<AccountDto>> GetAccounts(TokenRequest request)
    {
        var user = Authorize(request.Token);
        if (user is null)
        {
            return Unauthorized<List<AccountDto>>();
        }

        var accounts = db.Accounts.Include(item => item.Branch).AsQueryable();
        if (!IsStaff(user))
        {
            accounts = accounts.Where(account => db.AccountOwners.Any(owner =>
                owner.AccountId == account.Id && owner.UserId == user.Id));
        }

        return ServiceResult<List<AccountDto>>.Success(accounts
            .OrderBy(account => account.Number)
            .AsEnumerable()
            .Select(ToDto)
            .ToList());
    }

    public ServiceResult<ProfileDto> GetProfile(TokenRequest request)
    {
        var user = Authorize(request.Token);
        if (user is null)
        {
            return Unauthorized<ProfileDto>();
        }

        var accounts = db.Accounts
            .Include(item => item.Branch)
            .Where(account => db.AccountOwners.Any(owner =>
                owner.AccountId == account.Id && owner.UserId == user.Id))
            .AsEnumerable()
            .Select(ToDto)
            .ToList();

        return ServiceResult<ProfileDto>.Success(new ProfileDto
        {
            Name = $"{user.FirstName} {user.LastName}".Trim(),
            UserName = user.UserName,
            Phone = user.Phone,
            Accounts = accounts
        });
    }

    public ServiceResult<List<CustomerDto>> GetCustomers(TokenRequest request)
    {
        var user = Authorize(request.Token);
        if (user is null)
        {
            return Unauthorized<List<CustomerDto>>();
        }

        if (!IsStaff(user))
        {
            return Forbidden<List<CustomerDto>>();
        }

        var customers = db.Users
            .Include(item => item.Role)
            .Where(item => item.Role!.Name == "CLIENTE")
            .OrderBy(item => item.FirstName)
            .Select(item => new CustomerDto
            {
                Id = item.Id,
                Name = $"{item.FirstName} {item.LastName}".Trim(),
                UserName = item.UserName
            })
            .ToList();

        return ServiceResult<List<CustomerDto>>.Success(customers);
    }

    public ServiceResult<List<MovementDto>> GetMovements(QueryRequest request)
    {
        var user = Authorize(request.Token);
        if (user is null)
        {
            return Unauthorized<List<MovementDto>>();
        }

        var account = FindAccount(request.AccountNumber);
        if (account is null)
        {
            return ServiceResult<List<MovementDto>>.Fail("NOT_FOUND", "La cuenta no existe.");
        }

        if (!CanAccess(user, account.Id))
        {
            return Forbidden<List<MovementDto>>();
        }

        var movements = db.Movements
            .Where(item => item.AccountId == account.Id)
            .OrderByDescending(item => item.CreatedAt)
            .AsEnumerable()
            .Select(item => ToDto(item, account.Number))
            .ToList();

        return ServiceResult<List<MovementDto>>.Success(movements);
    }

    public ServiceResult<MovementDto> RegisterMovement(MoneyRequest request)
    {
        var user = Authorize(request.Token);
        if (user is null)
        {
            return Unauthorized<MovementDto>();
        }

        if (!IsStaff(user))
        {
            return Forbidden<MovementDto>();
        }

        if (request.Amount <= 0 || request.Type is not ("INGRESO" or "RETIRO"))
        {
            return ServiceResult<MovementDto>.Fail("VALIDATION", "Tipo de operación o monto inválido.");
        }

        using var transaction = db.Database.BeginTransaction();
        var account = FindAccount(request.AccountNumber);
        if (account is null)
        {
            return ServiceResult<MovementDto>.Fail("NOT_FOUND", "La cuenta no existe.");
        }

        if (account.Status != "ACTIVA")
        {
            return ServiceResult<MovementDto>.Fail("ACCOUNT", "La cuenta no está activa.");
        }

        if (request.Type == "RETIRO" && account.Balance < request.Amount)
        {
            return ServiceResult<MovementDto>.Fail("FUNDS", "Saldo insuficiente.");
        }

        var balanceBefore = account.Balance;
        account.Balance += request.Type == "INGRESO" ? request.Amount : -request.Amount;
        var movement = AddMovement(account, request.Type, request.Amount, balanceBefore, user.UserName);
        db.SaveChanges();
        transaction.Commit();

        return ServiceResult<MovementDto>.Success(
            ToDto(movement, account.Number),
            "Movimiento registrado de forma auditable.");
    }

    public ServiceResult<List<MovementDto>> Transfer(TransferRequest request)
    {
        var user = Authorize(request.Token);
        if (user is null)
        {
            return Unauthorized<List<MovementDto>>();
        }

        if (request.Amount <= 0 || request.SourceNumber == request.DestinationNumber)
        {
            return ServiceResult<List<MovementDto>>.Fail("VALIDATION", "Datos de transferencia inválidos.");
        }

        using var transaction = db.Database.BeginTransaction();
        var source = FindAccount(request.SourceNumber);
        var destination = FindAccount(request.DestinationNumber);
        if (source is null || destination is null)
        {
            return ServiceResult<List<MovementDto>>.Fail("NOT_FOUND", "Una de las cuentas no existe.");
        }

        if (!CanAccess(user, source.Id))
        {
            return Forbidden<List<MovementDto>>();
        }

        if (source.Status != "ACTIVA" || destination.Status != "ACTIVA")
        {
            return ServiceResult<List<MovementDto>>.Fail("ACCOUNT", "Las cuentas deben estar activas.");
        }

        if (source.Balance < request.Amount)
        {
            return ServiceResult<List<MovementDto>>.Fail("FUNDS", "Saldo insuficiente.");
        }

        var sourceBalanceBefore = source.Balance;
        source.Balance -= request.Amount;
        var debit = AddMovement(source, "TRANSFERENCIA_SALIDA", request.Amount, sourceBalanceBefore, user.UserName);

        var destinationBalanceBefore = destination.Balance;
        destination.Balance += request.Amount;
        var credit = AddMovement(destination, "TRANSFERENCIA_ENTRADA", request.Amount, destinationBalanceBefore, user.UserName);

        db.SaveChanges();
        transaction.Commit();

        return ServiceResult<List<MovementDto>>.Success(
            [ToDto(debit, source.Number), ToDto(credit, destination.Number)],
            "Transferencia completada correctamente.");
    }

    public ServiceResult<MovementDto> Reverse(ReverseRequest request)
    {
        var user = Authorize(request.Token);
        if (user is null)
        {
            return Unauthorized<MovementDto>();
        }

        if (!IsStaff(user))
        {
            return Forbidden<MovementDto>();
        }

        using var transaction = db.Database.BeginTransaction();
        var original = db.Movements.SingleOrDefault(item => item.Id == request.MovementId);
        if (original is null)
        {
            return ServiceResult<MovementDto>.Fail("NOT_FOUND", "El movimiento no existe.");
        }

        if (original.Type.StartsWith("REVERSA_", StringComparison.Ordinal))
        {
            return ServiceResult<MovementDto>.Fail("VALIDATION", "No se puede reversar una reversa.");
        }

        var account = db.Accounts.Single(item => item.Id == original.AccountId);
        var reverseType = original.Type is "INGRESO" or "TRANSFERENCIA_ENTRADA"
            ? "REVERSA_RETIRO"
            : "REVERSA_INGRESO";

        if (reverseType == "REVERSA_RETIRO" && account.Balance < original.Amount)
        {
            return ServiceResult<MovementDto>.Fail("FUNDS", "No hay saldo para realizar la reversa.");
        }

        var balanceBefore = account.Balance;
        account.Balance += reverseType == "REVERSA_INGRESO" ? original.Amount : -original.Amount;
        var reverse = AddMovement(account, reverseType, original.Amount, balanceBefore, user.UserName);
        db.SaveChanges();
        transaction.Commit();

        return ServiceResult<MovementDto>.Success(
            ToDto(reverse, account.Number),
            "Reversa registrada correctamente.");
    }

    public ServiceResult<AccountDto> CreateAccount(CreateAccountRequest request)
    {
        var user = Authorize(request.Token);
        if (user is null)
        {
            return Unauthorized<AccountDto>();
        }

        if (!IsStaff(user))
        {
            return Forbidden<AccountDto>();
        }

        var customer = db.Users
            .Include(item => item.Role)
            .SingleOrDefault(item => item.Id == request.CustomerId && item.Role!.Name == "CLIENTE");
        if (customer is null)
        {
            return ServiceResult<AccountDto>.Fail("NOT_FOUND", "El cliente no existe.");
        }

        var branch = db.Branches.OrderBy(item => item.Name).FirstOrDefault();
        if (branch is null)
        {
            return ServiceResult<AccountDto>.Fail("CONFIG", "No existen sucursales configuradas.");
        }

        string accountNumber;
        do
        {
            accountNumber = RandomNumberGenerator.GetInt32(1_000_000_000, int.MaxValue).ToString();
        }
        while (db.Accounts.Any(item => item.Number == accountNumber));

        var account = new Account
        {
            Id = Guid.NewGuid().ToString(),
            Number = accountNumber,
            BranchId = branch.Id,
            Balance = 0,
            Status = "ACTIVA",
            Branch = branch
        };

        db.Accounts.Add(account);
        db.AccountOwners.Add(new AccountOwner { AccountId = account.Id, UserId = customer.Id });
        db.SaveChanges();

        return ServiceResult<AccountDto>.Success(ToDto(account), "Cuenta creada y asignada correctamente.");
    }

    private User? Authorize(string token) => db.Tokens
        .Where(item => item.Value == token && item.Status == "ACTIVO")
        .Join(
            db.Users.Include(item => item.Role),
            tokenItem => tokenItem.UserId,
            user => user.Id,
            (_, user) => user)
        .SingleOrDefault();

    private Account? FindAccount(string number) => db.Accounts
        .Include(item => item.Branch)
        .SingleOrDefault(item => item.Number == number || item.Id == number);

    private bool CanAccess(User user, string accountId) =>
        IsStaff(user) || db.AccountOwners.Any(item => item.UserId == user.Id && item.AccountId == accountId);

    private static bool IsStaff(User user) => user.Role?.Name is "ADMIN" or "CAJERO";

    private Movement AddMovement(Account account, string type, decimal amount, decimal balanceBefore, string author)
    {
        var movement = new Movement
        {
            Id = Guid.NewGuid().ToString(),
            AccountId = account.Id,
            Type = type,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = account.Balance,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = author
        };
        db.Movements.Add(movement);
        return movement;
    }

    private static AccountDto ToDto(Account account) => new()
    {
        Id = account.Id,
        Number = account.Number,
        Balance = account.Balance,
        Status = account.Status,
        Availability = account.Status == "ACTIVA" ? "DISPONIBLE" : "BLOQUEADA",
        Branch = account.Branch?.Name ?? string.Empty
    };

    private static MovementDto ToDto(Movement movement, string accountNumber) => new()
    {
        Id = movement.Id,
        Type = movement.Type,
        Amount = movement.Amount,
        CreatedAt = movement.CreatedAt,
        CreatedBy = movement.CreatedBy,
        AccountNumber = accountNumber
    };

    private static string Hash(string value) => Convert
        .ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value ?? string.Empty)))
        .ToLowerInvariant();

    private static ServiceResult<T> Unauthorized<T>() =>
        ServiceResult<T>.Fail("AUTH", "Sesión inválida o expirada.");

    private static ServiceResult<T> Forbidden<T>() =>
        ServiceResult<T>.Fail("FORBIDDEN", "No tiene permiso para realizar esta operación.");
}
