using EurekaBank.Client;
using EurekaBank.Contracts;

namespace EurekaBank.Desktop.Controladores;

/// <summary>Controlador MVC para la vista WPF.</summary>
public sealed class BancaControlador : IDisposable
{
    private readonly EurekaBankSoapClient soap = new();
    public LoginDto? Sesion { get; private set; }
    public ServiceResult<LoginDto> Ingresar(string usuario, string clave) { var r = soap.Login(usuario, clave); Sesion = r.Data; return r; }
    public ServiceResult<List<AccountDto>> Cuentas() => soap.Accounts(Sesion!.Token);
    public ServiceResult<List<MovementDto>> Transferir(string origen, string destino, decimal monto) => soap.Transfer(Sesion!.Token, origen, destino, monto);
    public ServiceResult<List<MovementDto>> Movimientos(string cuenta) => soap.Movements(Sesion!.Token, cuenta);
    public ServiceResult<ProfileDto> Perfil() => soap.Profile(Sesion!.Token);
    public ServiceResult<MovementDto> Operar(string cuenta, string tipo, decimal monto) => soap.Movement(Sesion!.Token, cuenta, tipo, monto);
    public ServiceResult<MovementDto> Reversar(string id) => soap.Reverse(Sesion!.Token, id);
    public ServiceResult<List<CustomerDto>> Clientes() => soap.Customers(Sesion!.Token);
    public ServiceResult<AccountDto> CrearCuenta(string clienteId) => soap.CreateAccount(Sesion!.Token, clienteId);
    public void Salir() { if (Sesion is not null) soap.Logout(Sesion.Token); Sesion = null; }
    public void Dispose() { Salir(); soap.Dispose(); }
}
