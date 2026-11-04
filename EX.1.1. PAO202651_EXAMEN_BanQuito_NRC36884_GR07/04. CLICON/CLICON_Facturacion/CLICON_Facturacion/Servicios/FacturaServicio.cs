using System.Collections.Generic;
using System.Threading.Tasks;
using CLICON_Facturacion.Modelos;

namespace CLICON_Facturacion.Servicios
{
    public sealed class FacturaServicio
    {
        private readonly ApiService _api;

        public FacturaServicio()
        {
            _api = new ApiService();
        }

        public Task<List<Factura>> ListarAsync()
        {
            return _api.GetAsync<List<Factura>>("api/facturas");
        }

        public Task<Factura> BuscarAsync(int numero)
        {
            return _api.GetAsync<Factura>("api/facturas/" + numero);
        }

        public Task<Factura> CrearAsync(FacturaRequest solicitud)
        {
            return _api.PostAsync<Factura>("api/facturas", solicitud);
        }
    }
}
