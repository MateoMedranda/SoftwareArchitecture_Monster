using EurekaBank.Client;
using EurekaBank.Contracts;

namespace EurekaBank.Desktop.Controladores;

/// <summary>Controlador MVC para la vista WPF.</summary>
public sealed class BancaControlador : IDisposable
{
    private readonly EurekaBankRestClient api = new();
    public LoginDto? Sesion { get; private set; }
    public ServiceResult<LoginDto> Ingresar(string usuario, string clave) { var r = api.Login(usuario, clave); Sesion = r.Data; return r; }
    public ServiceResult<List<AccountDto>> Cuentas() => api.Accounts(Sesion!.Token);
    public ServiceResult<List<MovementDto>> Transferir(string origen, string destino, decimal monto) => api.Transfer(Sesion!.Token, origen, destino, monto);
    public ServiceResult<List<MovementDto>> Movimientos(string cuenta) => api.Movements(Sesion!.Token, cuenta);
    public ServiceResult<ProfileDto> Perfil() => api.Profile(Sesion!.Token);
    public ServiceResult<MovementDto> Operar(string cuenta, string tipo, decimal monto) => api.Movement(Sesion!.Token, cuenta, tipo, monto);
    public ServiceResult<MovementDto> Reversar(string id) => api.Reverse(Sesion!.Token, id);
    public ServiceResult<List<CustomerDto>> Clientes() => api.Customers(Sesion!.Token);
    public ServiceResult<AccountDto> CrearCuenta(string clienteId) => api.CreateAccount(Sesion!.Token, clienteId);
    public void Salir() { if (Sesion is not null) api.Logout(Sesion.Token); Sesion = null; }
    public void Dispose() { Salir(); api.Dispose(); }
}
