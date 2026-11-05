using System.Net.Http.Headers;

namespace CLIWEB_Facturacion.Config;

public sealed class AutorizacionMockHandler(SesionMock sesion) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (sesion.Autenticado)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SesionMock.TokenDemo);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
