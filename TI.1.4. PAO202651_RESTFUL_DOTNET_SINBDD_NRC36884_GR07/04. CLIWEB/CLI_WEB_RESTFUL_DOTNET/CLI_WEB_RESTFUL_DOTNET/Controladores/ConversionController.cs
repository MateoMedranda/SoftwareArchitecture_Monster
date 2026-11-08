using CLI_WEB_RESTFUL_DOTNET.Modelos.Conversiones;
using CLI_WEB_RESTFUL_DOTNET.Servicios;
using Microsoft.AspNetCore.Components;

namespace CLI_WEB_RESTFUL_DOTNET.Controladores;

/// <summary>
/// CONTROLADOR de MVC para la pantalla del conversor. La vista (Home.razor) no conoce
/// el servicio SOAP ni el token: solo pide datos y acciones a este controlador.
/// </summary>
public class ConversionController
{
    private readonly IConversionServicio _conversion;
    private readonly SesionUsuario _sesion;
    private readonly NavigationManager _navegacion;

    public ConversionController(IConversionServicio conversion, SesionUsuario sesion, NavigationManager navegacion)
    {
        _conversion = conversion;
        _sesion = sesion;
        _navegacion = navegacion;
    }
    public bool EstaAutenticado => _sesion.EstaAutenticado;
    public string NombreUsuario => _sesion.NombreUsuario ?? string.Empty;

    public IReadOnlyList<string> Categorias => CatalogoUnidades.Categorias;
    public IReadOnlyList<UnidadInfo> UnidadesDe(string categoria) => CatalogoUnidades.UnidadesDe(categoria);

    /// <summary>
    /// Recupera la sesión guardada en el navegador. Si no hay sesión, envía al usuario al login.
    /// Devuelve true si hay sesión válida.
    /// </summary>
    public async Task<bool> CargarSesionAsync()
    {
        if (_sesion.EstaAutenticado) return true;

        _navegacion.NavigateTo("/login");
        return false;
    }

    public async Task<ResultadoConversionModelo> ConvertirAsync(SolicitudConversionModelo solicitud)
    {
        if (_sesion.Token is null)
        {
            _navegacion.NavigateTo("/login");
            return new ResultadoConversionModelo(false, null, "La sesión ha expirado.");
        }

        return await _conversion.ConvertirAsync(_sesion.Token, solicitud);
    }

    public async Task CerrarSesionAsync()
    {
        _sesion.Cerrar();
        _navegacion.NavigateTo("/login");
    }
}
