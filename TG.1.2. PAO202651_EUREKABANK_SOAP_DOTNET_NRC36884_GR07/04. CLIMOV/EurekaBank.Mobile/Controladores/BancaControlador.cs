using EurekaBank.Client;
using EurekaBank.Contracts;

namespace EurekaBank.Mobile.Controladores;

/// <summary>Controlador MVC para la vista MAUI; conserva únicamente la sesión de la aplicación.</summary>
public sealed class BancaControlador : IDisposable
{
    private readonly EurekaBankSoapClient soap = new();
    public LoginDto? Sesion { get; private set; }
    public ServiceResult<LoginDto> Ingresar(string usuario, string clave) { var r = soap.Login(usuario, clave); Sesion = r.Data; return r; }
    public ServiceResult<List<AccountDto>> Cuentas() => soap.Accounts(Sesion!.Token);
    public ServiceResult<List<MovementDto>> Transferir(string origen, string destino, decimal monto) => soap.Transfer(Sesion!.Token, origen, destino, monto);
    public void Salir() { if (Sesion is not null) soap.Logout(Sesion.Token); Sesion = null; }
    public void Dispose() { Salir(); soap.Dispose(); }
}
