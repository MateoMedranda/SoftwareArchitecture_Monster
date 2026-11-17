using System.Runtime.Serialization;

namespace EurekaBank.Contracts;

[DataContract] public sealed class ServiceResult<T>
{
    [DataMember(Order = 1)] public bool Ok { get; set; }
    [DataMember(Order = 2)] public string Message { get; set; } = "";
    [DataMember(Order = 3)] public string Code { get; set; } = "";
    [DataMember(Order = 4)] public T? Data { get; set; }
    public static ServiceResult<T> Success(T data, string message = "Operación completada.") => new() { Ok = true, Data = data, Message = message };
    public static ServiceResult<T> Fail(string code, string message) => new() { Code = code, Message = message };
}

[DataContract] public sealed class LoginRequest { [DataMember(Order = 1)] public string UserName { get; set; } = ""; [DataMember(Order = 2)] public string Password { get; set; } = ""; }
[DataContract] public sealed class LoginDto { [DataMember(Order = 1)] public string Token { get; set; } = ""; [DataMember(Order = 2)] public string UserId { get; set; } = ""; [DataMember(Order = 3)] public string Name { get; set; } = ""; [DataMember(Order = 4)] public string Role { get; set; } = ""; }
[DataContract] public sealed class AccountDto { [DataMember(Order = 1)] public string Id { get; set; } = ""; [DataMember(Order = 2)] public string Number { get; set; } = ""; [DataMember(Order = 3)] public decimal Balance { get; set; } [DataMember(Order = 4)] public string Status { get; set; } = ""; [DataMember(Order = 5)] public string Availability { get; set; } = "DISPONIBLE"; [DataMember(Order = 6)] public string Branch { get; set; } = ""; }
[DataContract] public sealed class CustomerDto { [DataMember(Order = 1)] public string Id { get; set; } = ""; [DataMember(Order = 2)] public string Name { get; set; } = ""; [DataMember(Order = 3)] public string UserName { get; set; } = ""; }
[DataContract] public sealed class ProfileDto { [DataMember(Order = 1)] public string Name { get; set; } = ""; [DataMember(Order = 2)] public string UserName { get; set; } = ""; [DataMember(Order = 3)] public string Phone { get; set; } = ""; [DataMember(Order = 4)] public List<AccountDto> Accounts { get; set; } = []; }
[DataContract] public sealed class MovementDto { [DataMember(Order = 1)] public string Id { get; set; } = ""; [DataMember(Order = 2)] public string Type { get; set; } = ""; [DataMember(Order = 3)] public decimal Amount { get; set; } [DataMember(Order = 4)] public DateTime CreatedAt { get; set; } [DataMember(Order = 5)] public string CreatedBy { get; set; } = ""; [DataMember(Order = 6)] public string AccountNumber { get; set; } = ""; }
[DataContract] public sealed class MoneyRequest { [DataMember(Order = 1)] public string Token { get; set; } = ""; [DataMember(Order = 2)] public string AccountNumber { get; set; } = ""; [DataMember(Order = 3)] public string Type { get; set; } = ""; [DataMember(Order = 4)] public decimal Amount { get; set; } }
[DataContract] public sealed class TransferRequest { [DataMember(Order = 1)] public string Token { get; set; } = ""; [DataMember(Order = 2)] public string SourceNumber { get; set; } = ""; [DataMember(Order = 3)] public string DestinationNumber { get; set; } = ""; [DataMember(Order = 4)] public decimal Amount { get; set; } }
[DataContract] public sealed class TokenRequest { [DataMember(Order = 1)] public string Token { get; set; } = ""; }
[DataContract] public sealed class QueryRequest { [DataMember(Order = 1)] public string Token { get; set; } = ""; [DataMember(Order = 2)] public string AccountNumber { get; set; } = ""; }
[DataContract] public sealed class CreateAccountRequest { [DataMember(Order = 1)] public string Token { get; set; } = ""; [DataMember(Order = 2)] public string CustomerId { get; set; } = ""; }
[DataContract] public sealed class ReverseRequest { [DataMember(Order = 1)] public string Token { get; set; } = ""; [DataMember(Order = 2)] public string MovementId { get; set; } = ""; }
