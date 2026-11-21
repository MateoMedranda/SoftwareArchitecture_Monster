using CoreWCF;
using EurekaBank.Contracts;

namespace EurekaBank.Server;

[ServiceContract(Name = "EurekaBankSoap", Namespace = "https://eurekabank.edu.ec/soap")]
public interface IEurekaBankSoapService
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
