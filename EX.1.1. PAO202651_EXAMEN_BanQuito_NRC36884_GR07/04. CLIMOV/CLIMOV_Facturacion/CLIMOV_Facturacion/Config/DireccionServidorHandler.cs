namespace CLIMOV_Facturacion.Config;

/// <summary>
/// Resuelve cada solicitud contra la dirección elegida por el usuario.
/// Así, un cambio de servidor tiene efecto sin reinstalar ni reiniciar la app.
/// </summary>
public sealed class DireccionServidorHandler : DelegatingHandler
{
    private readonly SesionMock _sesion;

    public DireccionServidorHandler(SesionMock sesion) => _sesion = sesion;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri is { IsAbsoluteUri: true } uri)
        {
            request.RequestUri = new Uri(new Uri(AppConfig.ApiBaseUrl), uri.PathAndQuery);
        }

        if (_sesion.Autenticado)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SesionMock.TokenDemo);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
