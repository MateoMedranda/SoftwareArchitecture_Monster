namespace CLIMOV_Facturacion.Config;

/// <summary>
/// Resuelve cada solicitud contra la dirección elegida por el usuario.
/// Así, un cambio de servidor tiene efecto sin reinstalar ni reiniciar la app.
/// </summary>
public sealed class DireccionServidorHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri is { IsAbsoluteUri: true } uri)
        {
            request.RequestUri = new Uri(new Uri(AppConfig.ApiBaseUrl), uri.PathAndQuery);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
