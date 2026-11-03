using System.Threading.Tasks;
using CLIESC_Facturacion.Modelos;

namespace CLIESC_Facturacion.Servicios
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
