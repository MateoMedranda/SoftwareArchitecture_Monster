using System.ServiceModel;
using EurekaBank.Contracts;
using EurekaBank.Mobile.Servicios;

namespace EurekaBank.Client;

public sealed class EurekaBankSoapClient(string? endpoint = null) : IDisposable
{
    private readonly ChannelFactory<IEurekaBankSoapClient> factory = new(new BasicHttpBinding(BasicHttpSecurityMode.None)
    {
        MaxReceivedMessageSize = 1024 * 1024,
        OpenTimeout = TimeSpan.FromSeconds(8), SendTimeout = TimeSpan.FromSeconds(12), ReceiveTimeout = TimeSpan.FromSeconds(12)
    }, new EndpointAddress(endpoint ?? ConfiguracionServidor.Url));

    private T Llamar<T>(Func<IEurekaBankSoapClient, T> operacion)
    {
        var channel = factory.CreateChannel();
        var comunicacion = (IClientChannel)channel;
        try { var resultado = operacion(channel); comunicacion.Close(); return resultado; }
        catch { comunicacion.Abort(); throw; }
    }

    public ServiceResult<LoginDto> Login(string user, string password) => Llamar(c => c.Login(new LoginRequest { UserName = user, Password = password }));
    public ServiceResult<bool> Logout(string token) => Llamar(c => c.Logout(new TokenRequest { Token = token }));
    public ServiceResult<List<AccountDto>> Accounts(string token) => Llamar(c => c.GetAccounts(new TokenRequest { Token = token }));
    public ServiceResult<ProfileDto> Profile(string token) => Llamar(c => c.GetProfile(new TokenRequest { Token = token }));
    public ServiceResult<List<CustomerDto>> Customers(string token) => Llamar(c => c.GetCustomers(new TokenRequest { Token = token }));
    public ServiceResult<List<MovementDto>> Movements(string token, string number) => Llamar(c => c.GetMovements(new QueryRequest { Token = token, AccountNumber = number }));
    public ServiceResult<MovementDto> Movement(string token, string number, string type, decimal amount) => Llamar(c => c.RegisterMovement(new MoneyRequest { Token = token, AccountNumber = number, Type = type, Amount = amount }));
    public ServiceResult<List<MovementDto>> Transfer(string token, string origin, string destination, decimal amount) => Llamar(c => c.Transfer(new TransferRequest { Token = token, SourceNumber = origin, DestinationNumber = destination, Amount = amount }));
    public ServiceResult<MovementDto> Reverse(string token, string id) => Llamar(c => c.Reverse(new ReverseRequest { Token = token, MovementId = id }));
    public ServiceResult<AccountDto> CreateAccount(string token, string customerId) => Llamar(c => c.CreateAccount(new CreateAccountRequest { Token = token, CustomerId = customerId }));
    public void Dispose() { try { factory.Close(); } catch { factory.Abort(); } }
}
