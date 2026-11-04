using System.Collections.Generic;
using System.Threading.Tasks;
using CLICON_Facturacion.Modelos;
using CLICON_Facturacion.Servicios;

namespace CLICON_Facturacion.Controladores
{
    /// <summary>
    /// Controlador de la vista de catalogo. Encapsula el acceso al
    /// ProductoControlador del API de facturacion.
    /// </summary>
    public sealed class ProductoControlador
    {
        private readonly ProductoServicio _servicio;

        public ProductoControlador()
        {
            _servicio = new ProductoServicio();
        }

        public Task<List<Producto>> ListarAsync()
        {
            return _servicio.ListarAsync();
        }

        public Task<Producto> BuscarAsync(int codigo)
        {
            return _servicio.BuscarAsync(codigo);
        }

        public Task<RespuestaApi> CrearAsync(Producto producto)
        {
            return _servicio.CrearAsync(ARequest(producto));
        }

        public Task<RespuestaApi> ActualizarAsync(Producto producto)
        {
            return _servicio.ActualizarAsync(producto.NumInventario, ARequest(producto));
        }

        public Task EliminarAsync(int codigo)
        {
            return _servicio.EliminarAsync(codigo);
        }

        private static ProductoRequest ARequest(Producto producto)
        {
            return new ProductoRequest
            {
                Nombre = (producto.Nombre ?? string.Empty).Trim(),
                Precio = producto.Precio
            };
        }
    }
}
