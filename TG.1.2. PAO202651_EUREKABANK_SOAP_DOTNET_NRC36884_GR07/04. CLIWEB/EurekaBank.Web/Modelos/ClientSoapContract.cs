using System.ServiceModel;

namespace EurekaBank.Contracts;

// Contrato consumido por los cuatro clientes .NET. Las acciones y tipos se mantienen estables.
[ServiceContract(Name = "EurekaBankSoap", Namespace = "https://eurekabank.edu.ec/soap")]
public interface IEurekaBankSoapClient
{
    [OperationContract] ServiceResult<LoginDto> Login(LoginRequest request);
    [OperationContract] ServiceResult<bool> Logout(TokenRequest request);
    [OperationContract] ServiceResult<List<AccountDto>> GetAccounts(TokenRequest request);
    [OperationContract] ServiceResult<ProfileDto> GetProfile(TokenRequest request);
    [OperationContract] ServiceResult<List<CustomerDto>> GetCustomers(TokenRequest request);
    [OperationContract] ServiceResult<List<MovementDto>> GetMovements(QueryRequest request);
    [OperationContract] ServiceResult<MovementDto> RegisterMovement(MoneyRequest request);
    [OperationContract] ServiceResult<List<MovementDto>> Transfer(TransferRequest request);
    [OperationContract] ServiceResult<MovementDto> Reverse(ReverseRequest request);
    [OperationContract] ServiceResult<AccountDto> CreateAccount(CreateAccountRequest request);
}
