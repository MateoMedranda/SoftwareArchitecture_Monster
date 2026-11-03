using EurekaBank.Client;
using EurekaBank.Contracts;
using EurekaBank.Mobile.Servicios;

namespace EurekaBank.Mobile.Controladores;

/// <summary>Controlador MVC para la vista MAUI; conserva únicamente la sesión de la aplicación.</summary>
public sealed class BancaControlador : IDisposable
{
    private EurekaBankRestClient? api;
    private EurekaBankRestClient Api => api ??= new EurekaBankRestClient();
    public LoginDto? Sesion { get; private set; }
    public string UrlServidor => ConfiguracionServidor.Url;
    public bool EsPersonal => Sesion?.Role is "ADMIN" or "CAJERO";
    public ServiceResult<LoginDto> Ingresar(string usuario, string clave) { var r = Api.Login(usuario, clave); Sesion = r.Data; return r; }
    public ServiceResult<List<AccountDto>> Cuentas() => Api.Accounts(Sesion!.Token);
    public ServiceResult<List<MovementDto>> Transferir(string origen, string destino, decimal monto) => Api.Transfer(Sesion!.Token, origen, destino, monto);
    public ServiceResult<List<MovementDto>> Movimientos(string cuenta) => Api.Movements(Sesion!.Token, cuenta);
    public ServiceResult<MovementDto> Operar(string cuenta, string tipo, decimal monto) => Api.Movement(Sesion!.Token, cuenta, tipo, monto);
    public ServiceResult<MovementDto> Reversar(string movimientoId) => Api.Reverse(Sesion!.Token, movimientoId);
    public ServiceResult<ProfileDto> Perfil() => Api.Profile(Sesion!.Token);
    public ServiceResult<List<CustomerDto>> Clientes() => Api.Customers(Sesion!.Token);
    public ServiceResult<AccountDto> CrearCuenta(string clienteId) => Api.CreateAccount(Sesion!.Token, clienteId);
    public bool CambiarServidor(string url, out string endpoint, out string error)
    {
        if (!ConfiguracionServidor.IntentarGuardar(url, out endpoint, out error)) return false;
        api?.Dispose(); api = null;
        return true;
    }
    public void Salir()
    {
        try { if (Sesion is not null && api is not null) Api.Logout(Sesion.Token); }
        finally { Sesion = null; }
    }
    public void Dispose() { Salir(); api?.Dispose(); }
}
