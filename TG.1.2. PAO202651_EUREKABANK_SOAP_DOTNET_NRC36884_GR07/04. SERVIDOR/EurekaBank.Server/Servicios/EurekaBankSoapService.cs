using System.Security.Cryptography;
using System.Text;
using CoreWCF;
using EurekaBank.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EurekaBank.Server;

// Servicio de aplicación: las vistas nunca acceden de forma directa a SQL Server.
public sealed class EurekaBankSoapService(EurekaDbContext db) : IEurekaBankSoapService
{
    public ServiceResult<LoginDto> Login(LoginRequest request)
    {
        var password = Hash(request.Password);
        var user = db.Users.Include(x => x.Role).SingleOrDefault(x => x.UserName == request.UserName && x.PasswordHash == password);
        if (user is null) return ServiceResult<LoginDto>.Fail("AUTH", "Usuario o contraseña inválidos.");

        var value = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        db.Tokens.Add(new BankToken { Id = Guid.NewGuid().ToString(), Value = value, Status = "ACTIVO", UserId = user.Id, CreatedAt = DateTime.UtcNow });
        db.SaveChanges();
        return ServiceResult<LoginDto>.Success(new LoginDto { Token = value, UserId = user.Id, Name = $"{user.FirstName} {user.LastName}".Trim(), Role = user.Role?.Name ?? "CLIENTE" }, "Sesión iniciada.");
    }

    public ServiceResult<bool> Logout(TokenRequest request)
    {
        var token = db.Tokens.SingleOrDefault(x => x.Value == request.Token && x.Status == "ACTIVO");
        if (token is null) return ServiceResult<bool>.Fail("AUTH", "Sesión inválida o expirada.");
        token.Status = "REVOCADO"; db.SaveChanges();
        return ServiceResult<bool>.Success(true, "Sesión cerrada.");
    }

    public ServiceResult<List<AccountDto>> GetAccounts(TokenRequest request)
    {
        var user = Authorize(request.Token); if (user is null) return Unauthorized<List<AccountDto>>();
        var query = db.Accounts.Include(x => x.Branch).AsQueryable();
        if (!IsStaff(user)) query = query.Where(x => db.AccountOwners.Any(o => o.AccountId == x.Id && o.UserId == user.Id));
        return ServiceResult<List<AccountDto>>.Success(query.OrderBy(x => x.Number).AsEnumerable().Select(ToDto).ToList());
    }

    public ServiceResult<ProfileDto> GetProfile(TokenRequest request)
    {
        var user = Authorize(request.Token); if (user is null) return Unauthorized<ProfileDto>();
        var accounts = db.Accounts.Include(x => x.Branch).Where(x => db.AccountOwners.Any(o => o.AccountId == x.Id && o.UserId == user.Id)).AsEnumerable().Select(ToDto).ToList();
        return ServiceResult<ProfileDto>.Success(new ProfileDto { Name = $"{user.FirstName} {user.LastName}".Trim(), UserName = user.UserName, Phone = user.Phone, Accounts = accounts });
    }

    public ServiceResult<List<CustomerDto>> GetCustomers(TokenRequest request)
    {
        var user = Authorize(request.Token); if (user is null) return Unauthorized<List<CustomerDto>>();
        if (!IsStaff(user)) return Forbidden<List<CustomerDto>>();
        return ServiceResult<List<CustomerDto>>.Success(db.Users.Include(x => x.Role).Where(x => x.Role!.Name == "CLIENTE").OrderBy(x => x.FirstName).Select(x => new CustomerDto { Id = x.Id, Name = $"{x.FirstName} {x.LastName}".Trim(), UserName = x.UserName }).ToList());
    }

    public ServiceResult<List<MovementDto>> GetMovements(QueryRequest request)
    {
        var user = Authorize(request.Token); if (user is null) return Unauthorized<List<MovementDto>>();
        var account = FindAccount(request.AccountNumber); if (account is null) return ServiceResult<List<MovementDto>>.Fail("NOT_FOUND", "La cuenta no existe.");
        if (!CanAccess(user, account.Id)) return Forbidden<List<MovementDto>>();
        return ServiceResult<List<MovementDto>>.Success(db.Movements.Where(x => x.AccountId == account.Id).OrderByDescending(x => x.CreatedAt).AsEnumerable().Select(x => ToDto(x, account.Number)).ToList());
    }

    public ServiceResult<MovementDto> RegisterMovement(MoneyRequest request)
    {
        var user = Authorize(request.Token); if (user is null) return Unauthorized<MovementDto>();
        if (!IsStaff(user)) return Forbidden<MovementDto>();
        if (request.Amount <= 0 || (request.Type != "INGRESO" && request.Type != "RETIRO")) return ServiceResult<MovementDto>.Fail("VALIDATION", "Tipo de operación o monto inválido.");
        using var transaction = db.Database.BeginTransaction();
        var account = FindAccount(request.AccountNumber); if (account is null) return ServiceResult<MovementDto>.Fail("NOT_FOUND", "La cuenta no existe.");
        if (account.Status != "ACTIVA") return ServiceResult<MovementDto>.Fail("ACCOUNT", "La cuenta no está activa.");
        if (request.Type == "RETIRO" && account.Balance < request.Amount) return ServiceResult<MovementDto>.Fail("FUNDS", "Saldo insuficiente.");
        var before = account.Balance; account.Balance += request.Type == "INGRESO" ? request.Amount : -request.Amount;
        var movement = AddMovement(account, request.Type, request.Amount, before, user.UserName);
        db.SaveChanges(); transaction.Commit();
        return ServiceResult<MovementDto>.Success(ToDto(movement, account.Number), "Movimiento registrado de forma auditable.");
    }

    public ServiceResult<List<MovementDto>> Transfer(TransferRequest request)
    {
        var user = Authorize(request.Token); if (user is null) return Unauthorized<List<MovementDto>>();
        if (request.Amount <= 0 || request.SourceNumber == request.DestinationNumber) return ServiceResult<List<MovementDto>>.Fail("VALIDATION", "Datos de transferencia inválidos.");
        using var transaction = db.Database.BeginTransaction();
        var source = FindAccount(request.SourceNumber); var destination = FindAccount(request.DestinationNumber);
        if (source is null || destination is null) return ServiceResult<List<MovementDto>>.Fail("NOT_FOUND", "Una de las cuentas no existe.");
        if (!CanAccess(user, source.Id)) return Forbidden<List<MovementDto>>();
        if (source.Status != "ACTIVA" || destination.Status != "ACTIVA") return ServiceResult<List<MovementDto>>.Fail("ACCOUNT", "Las cuentas deben estar activas.");
        if (source.Balance < request.Amount) return ServiceResult<List<MovementDto>>.Fail("FUNDS", "Saldo insuficiente.");
        var sourceBefore = source.Balance; source.Balance -= request.Amount;
        var debit = AddMovement(source, "TRANSFERENCIA_SALIDA", request.Amount, sourceBefore, user.UserName);
        var destinationBefore = destination.Balance; destination.Balance += request.Amount;
        var credit = AddMovement(destination, "TRANSFERENCIA_ENTRADA", request.Amount, destinationBefore, user.UserName);
        db.SaveChanges(); transaction.Commit();
        return ServiceResult<List<MovementDto>>.Success([ToDto(debit, source.Number), ToDto(credit, destination.Number)], "Transferencia completada correctamente.");
    }

    public ServiceResult<MovementDto> Reverse(ReverseRequest request)
    {
        var user = Authorize(request.Token); if (user is null) return Unauthorized<MovementDto>();
        if (!IsStaff(user)) return Forbidden<MovementDto>();
        using var transaction = db.Database.BeginTransaction();
        var original = db.Movements.SingleOrDefault(x => x.Id == request.MovementId);
        if (original is null) return ServiceResult<MovementDto>.Fail("NOT_FOUND", "El movimiento no existe.");
        if (original.Type.StartsWith("REVERSA_")) return ServiceResult<MovementDto>.Fail("VALIDATION", "No se puede reversar una reversa.");
        var account = db.Accounts.Single(x => x.Id == original.AccountId);
        var reverseType = original.Type is "INGRESO" or "TRANSFERENCIA_ENTRADA" ? "REVERSA_RETIRO" : "REVERSA_INGRESO";
        if (reverseType == "REVERSA_RETIRO" && account.Balance < original.Amount) return ServiceResult<MovementDto>.Fail("FUNDS", "No hay saldo para realizar la reversa.");
        var before = account.Balance; account.Balance += reverseType == "REVERSA_INGRESO" ? original.Amount : -original.Amount;
        var reverse = AddMovement(account, reverseType, original.Amount, before, user.UserName);
        db.SaveChanges(); transaction.Commit();
        return ServiceResult<MovementDto>.Success(ToDto(reverse, account.Number), "Reversa registrada correctamente.");
    }

    public ServiceResult<AccountDto> CreateAccount(CreateAccountRequest request)
    {
        var user = Authorize(request.Token); if (user is null) return Unauthorized<AccountDto>();
        if (!IsStaff(user)) return Forbidden<AccountDto>();
        var customer = db.Users.Include(x => x.Role).SingleOrDefault(x => x.Id == request.CustomerId && x.Role!.Name == "CLIENTE");
        if (customer is null) return ServiceResult<AccountDto>.Fail("NOT_FOUND", "El cliente no existe.");
        var branch = db.Branches.OrderBy(x => x.Name).FirstOrDefault(); if (branch is null) return ServiceResult<AccountDto>.Fail("CONFIG", "No existen sucursales configuradas.");
        string number; do { number = RandomNumberGenerator.GetInt32(1_000_000_000, int.MaxValue).ToString(); } while (db.Accounts.Any(x => x.Number == number));
        var account = new Account { Id = Guid.NewGuid().ToString(), Number = number, BranchId = branch.Id, Balance = 0, Status = "ACTIVA", Branch = branch };
        db.Accounts.Add(account); db.AccountOwners.Add(new AccountOwner { AccountId = account.Id, UserId = customer.Id }); db.SaveChanges();
        return ServiceResult<AccountDto>.Success(ToDto(account), "Cuenta creada y asignada correctamente.");
    }

    private User? Authorize(string token) => db.Tokens.Where(x => x.Value == token && x.Status == "ACTIVO").Join(db.Users.Include(x => x.Role), t => t.UserId, u => u.Id, (_, u) => u).SingleOrDefault();
    private Account? FindAccount(string number) => db.Accounts.Include(x => x.Branch).SingleOrDefault(x => x.Number == number || x.Id == number);
    private bool CanAccess(User user, string accountId) => IsStaff(user) || db.AccountOwners.Any(x => x.UserId == user.Id && x.AccountId == accountId);
    private static bool IsStaff(User user) => user.Role?.Name is "ADMIN" or "CAJERO";
    private Movement AddMovement(Account account, string type, decimal amount, decimal before, string author) { var m = new Movement { Id = Guid.NewGuid().ToString(), AccountId = account.Id, Type = type, Amount = amount, BalanceBefore = before, BalanceAfter = account.Balance, CreatedAt = DateTime.UtcNow, CreatedBy = author }; db.Movements.Add(m); return m; }
    private static AccountDto ToDto(Account x) => new() { Id = x.Id, Number = x.Number, Balance = x.Balance, Status = x.Status, Availability = x.Status == "ACTIVA" ? "DISPONIBLE" : "BLOQUEADA", Branch = x.Branch?.Name ?? "" };
    private static MovementDto ToDto(Movement x, string number) => new() { Id = x.Id, Type = x.Type, Amount = x.Amount, CreatedAt = x.CreatedAt, CreatedBy = x.CreatedBy, AccountNumber = number };
    private static string Hash(string text) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text ?? ""))).ToLowerInvariant();
    private static ServiceResult<T> Unauthorized<T>() => ServiceResult<T>.Fail("AUTH", "Sesión inválida o expirada.");
    private static ServiceResult<T> Forbidden<T>() => ServiceResult<T>.Fail("FORBIDDEN", "No tiene permiso para realizar esta operación.");
}
