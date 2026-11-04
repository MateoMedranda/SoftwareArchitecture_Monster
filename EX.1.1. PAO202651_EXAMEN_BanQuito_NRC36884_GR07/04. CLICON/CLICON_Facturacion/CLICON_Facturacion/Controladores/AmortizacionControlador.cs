using System.Threading.Tasks;
using CLICON_Facturacion.Modelos;
using CLICON_Facturacion.Servicios;

namespace CLICON_Facturacion.Controladores
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
