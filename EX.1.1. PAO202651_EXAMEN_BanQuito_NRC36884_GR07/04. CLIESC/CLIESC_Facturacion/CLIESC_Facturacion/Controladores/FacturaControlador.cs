using System.Collections.Generic;
using System.Threading.Tasks;
using CLIESC_Facturacion.Modelos;
using CLIESC_Facturacion.Servicios;

namespace CLIESC_Facturacion.Controladores
{
    /// <summary>
    /// Controlador de la vista de facturas. Encapsula el acceso al
    /// FacturaControlador del API de facturacion.
    /// </summary>
    public sealed class FacturaControlador
    {
        private readonly FacturaServicio _servicio;

        public FacturaControlador()
        {
            _servicio = new FacturaServicio();
        }

        public Task<List<Factura>> ListarAsync()
        {
            return _servicio.ListarAsync();
        }

        public Task<Factura> BuscarAsync(int numero)
        {
            return _servicio.BuscarAsync(numero);
        }

        public Task<Factura> CrearAsync(int codCliente, string tipoPago, int? numCuotas, IEnumerable<FacturaItemRequest> items)
        {
            var solicitud = new FacturaRequest
            {
                CodCliente = codCliente,
                TipoPago = (tipoPago ?? string.Empty).Trim(),
                NumCuotas = numCuotas,
                Items = new List<FacturaItemRequest>(items)
            };

            return _servicio.CrearAsync(solicitud);
        }
    }
}
