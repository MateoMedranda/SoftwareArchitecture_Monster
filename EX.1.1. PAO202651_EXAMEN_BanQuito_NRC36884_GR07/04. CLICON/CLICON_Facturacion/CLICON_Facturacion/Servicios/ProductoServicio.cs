using System.Collections.Generic;
using System.Threading.Tasks;
using CLICON_Facturacion.Modelos;

namespace CLICON_Facturacion.Servicios
{
    public sealed class ProductoServicio
    {
        private readonly ApiService _api;

        public ProductoServicio()
        {
            _api = new ApiService();
        }

        public Task<List<Producto>> ListarAsync()
        {
            return _api.GetAsync<List<Producto>>("api/productos");
        }

        public Task<Producto> BuscarAsync(int codigo)
        {
            return _api.GetAsync<Producto>("api/productos/" + codigo);
        }

        public Task<RespuestaApi> CrearAsync(ProductoRequest solicitud)
        {
            return _api.PostAsync<RespuestaApi>("api/productos", solicitud);
        }

        public Task<RespuestaApi> ActualizarAsync(int codigo, ProductoRequest solicitud)
        {
            return _api.PutAsync<RespuestaApi>("api/productos/" + codigo, solicitud);
        }

        public Task EliminarAsync(int codigo)
        {
            return _api.DeleteAsync("api/productos/" + codigo);
        }
    }
}
