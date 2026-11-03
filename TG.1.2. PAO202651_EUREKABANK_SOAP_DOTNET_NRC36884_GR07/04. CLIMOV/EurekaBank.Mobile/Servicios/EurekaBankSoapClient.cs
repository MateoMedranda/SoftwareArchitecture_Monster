using System.ServiceModel;
using EurekaBank.Contracts;

namespace EurekaBank.Client;

public sealed class EurekaBankSoapClient(string? endpoint = null) : IDisposable
{
    private readonly ChannelFactory<IEurekaBankSoapClient> factory = new(new BasicHttpBinding(BasicHttpSecurityMode.None) { MaxReceivedMessageSize = 1024 * 1024 }, new EndpointAddress(endpoint ?? Environment.GetEnvironmentVariable("EUREKABANK_URL") ?? "http://10.0.2.2:51241/EurekaBankSoap.svc"));
    private IEurekaBankSoapClient Channel => factory.CreateChannel();
    public ServiceResult<LoginDto> Login(string user, string password) => Channel.Login(new LoginRequest { UserName = user, Password = password });
    public ServiceResult<bool> Logout(string token) => Channel.Logout(new TokenRequest { Token = token });
    public ServiceResult<List<AccountDto>> Accounts(string token) => Channel.GetAccounts(new TokenRequest { Token = token });
    public ServiceResult<ProfileDto> Profile(string token) => Channel.GetProfile(new TokenRequest { Token = token });
    public ServiceResult<List<CustomerDto>> Customers(string token) => Channel.GetCustomers(new TokenRequest { Token = token });
    public ServiceResult<List<MovementDto>> Movements(string token, string number) => Channel.GetMovements(new QueryRequest { Token = token, AccountNumber = number });
    public ServiceResult<MovementDto> Movement(string token, string number, string type, decimal amount) => Channel.RegisterMovement(new MoneyRequest { Token = token, AccountNumber = number, Type = type, Amount = amount });
    public ServiceResult<List<MovementDto>> Transfer(string token, string origin, string destination, decimal amount) => Channel.Transfer(new TransferRequest { Token = token, SourceNumber = origin, DestinationNumber = destination, Amount = amount });
    public ServiceResult<MovementDto> Reverse(string token, string id) => Channel.Reverse(new ReverseRequest { Token = token, MovementId = id });
    public ServiceResult<AccountDto> CreateAccount(string token, string customerId) => Channel.CreateAccount(new CreateAccountRequest { Token = token, CustomerId = customerId });
    public void Dispose() { try { factory.Close(); } catch { factory.Abort(); } }
}
