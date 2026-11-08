using CLI_WEB_RESTFUL_DOTNET.Modelos;
using CLI_WEB_RESTFUL_DOTNET.Servicios;
using Microsoft.AspNetCore.Components;

namespace CLI_WEB_RESTFUL_DOTNET.Controladores;

/// <summary>
/// CONTROLADOR de MVC. La vista (Login.razor) no habla con el servicio SOAP:
/// solo llama a este controlador, que orquesta modelo, servicio y sesión.
/// </summary>
public class LoginController
{
    private readonly IAutenticacionServicio _autenticacion;
    private readonly SesionUsuario _sesion;
    private readonly NavigationManager _navegacion;

    public LoginController(IAutenticacionServicio autenticacion, SesionUsuario sesion, NavigationManager navegacion)
    {
        _autenticacion = autenticacion;
        _sesion = sesion;
        _navegacion = navegacion;
    }

    /// <summary>Intenta autenticar; si va bien guarda el token y navega a la pantalla principal.</summary>
    public async Task<ResultadoLogin> IniciarSesionAsync(CredencialesLogin credenciales)
    {
        var resultado = await _autenticacion.IniciarSesionAsync(credenciales);

        if (resultado.Exito && resultado.Token is not null)
        {
            _sesion.Iniciar(credenciales.NombreUsuario, resultado.Token);
            _navegacion.NavigateTo("/");
        }

        return resultado;
    }

    public void CerrarSesion()
    {
        _sesion.Cerrar();
        _navegacion.NavigateTo("/login");
    }
}
