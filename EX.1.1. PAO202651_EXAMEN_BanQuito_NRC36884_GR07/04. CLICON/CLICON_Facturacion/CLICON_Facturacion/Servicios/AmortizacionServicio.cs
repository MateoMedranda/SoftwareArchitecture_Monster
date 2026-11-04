using System.Threading.Tasks;
using CLICON_Facturacion.Modelos;

namespace CLICON_Facturacion.Servicios
{
    public sealed class AmortizacionServicio
    {
        private readonly ApiService _api;

        public AmortizacionServicio()
        {
            _api = new ApiService();
        }

        public Task<RespuestaAmortizacion> ObtenerPorFacturaAsync(int numFactura)
        {
            return _api.GetAsync<RespuestaAmortizacion>("api/amortizaciones/" + numFactura);
        }
    }
}
