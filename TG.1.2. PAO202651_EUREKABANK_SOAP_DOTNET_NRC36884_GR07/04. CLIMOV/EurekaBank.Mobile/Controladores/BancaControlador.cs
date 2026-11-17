using EurekaBank.Client;
using EurekaBank.Contracts;
using EurekaBank.Mobile.Servicios;

namespace EurekaBank.Mobile.Controladores;

/// <summary>Controlador MVC para la vista MAUI; conserva únicamente la sesión de la aplicación.</summary>
public sealed class BancaControlador : IDisposable
{
    private EurekaBankSoapClient? soap;
    private EurekaBankSoapClient Soap => soap ??= new EurekaBankSoapClient();
    public LoginDto? Sesion { get; private set; }
    public string UrlServidor => ConfiguracionServidor.Url;
    public bool EsPersonal => Sesion?.Role is "ADMIN" or "CAJERO";
    public ServiceResult<LoginDto> Ingresar(string usuario, string clave) { var r = Soap.Login(usuario, clave); Sesion = r.Data; return r; }
    public ServiceResult<List<AccountDto>> Cuentas() => Soap.Accounts(Sesion!.Token);
    public ServiceResult<List<MovementDto>> Transferir(string origen, string destino, decimal monto) => Soap.Transfer(Sesion!.Token, origen, destino, monto);
    public ServiceResult<List<MovementDto>> Movimientos(string cuenta) => Soap.Movements(Sesion!.Token, cuenta);
    public ServiceResult<MovementDto> Operar(string cuenta, string tipo, decimal monto) => Soap.Movement(Sesion!.Token, cuenta, tipo, monto);
    public ServiceResult<MovementDto> Reversar(string movimientoId) => Soap.Reverse(Sesion!.Token, movimientoId);
    public ServiceResult<ProfileDto> Perfil() => Soap.Profile(Sesion!.Token);
    public ServiceResult<List<CustomerDto>> Clientes() => Soap.Customers(Sesion!.Token);
    public ServiceResult<AccountDto> CrearCuenta(string clienteId) => Soap.CreateAccount(Sesion!.Token, clienteId);
    public bool CambiarServidor(string url, out string endpoint, out string error)
    {
        if (!ConfiguracionServidor.IntentarGuardar(url, out endpoint, out error)) return false;
        soap?.Dispose(); soap = null;
        return true;
    }
    public void Salir()
    {
        try { if (Sesion is not null && soap is not null) Soap.Logout(Sesion.Token); }
        finally { Sesion = null; }
    }
    public void Dispose() { Salir(); soap?.Dispose(); }
}
