using System.Threading.Tasks;
using CLIESC_Facturacion.Modelos;
using CLIESC_Facturacion.Servicios;

namespace CLIESC_Facturacion.Controladores
{
    public sealed class AmortizacionControlador
    {
        private readonly AmortizacionServicio _servicio;

        public AmortizacionControlador()
        {
            _servicio = new AmortizacionServicio();
        }

        public Task<RespuestaAmortizacion> ObtenerPorFacturaAsync(int numFactura)
        {
            return _servicio.ObtenerPorFacturaAsync(numFactura);
        }
    }
}
