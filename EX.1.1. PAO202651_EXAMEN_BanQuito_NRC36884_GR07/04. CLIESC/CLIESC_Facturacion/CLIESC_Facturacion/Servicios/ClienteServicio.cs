using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLIESC_Facturacion.Modelos;

namespace CLIESC_Facturacion.Servicios
{
    public sealed class ClienteServicio
    {
        private readonly ApiService _api;

        public ClienteServicio()
        {
            _api = new ApiService();
        }

        public Task<List<Cliente>> ListarAsync()
        {
            return _api.GetAsync<List<Cliente>>("api/clientes");
        }

        public Task<Cliente> BuscarAsync(int codigo)
        {
            return _api.GetAsync<Cliente>("api/clientes/" + codigo);
        }

        public Task<Cliente> BuscarPorCedulaAsync(string cedula)
        {
            return _api.GetAsync<Cliente>("api/clientes/por-cedula/" + Uri.EscapeDataString(cedula));
        }

        public Task<RespuestaApi> CrearAsync(ClienteRequest solicitud)
        {
            return _api.PostAsync<RespuestaApi>("api/clientes", solicitud);
        }

        public Task<RespuestaApi> ActualizarAsync(int codigo, ClienteRequest solicitud)
        {
            return _api.PutAsync<RespuestaApi>("api/clientes/" + codigo, solicitud);
        }

        public Task EliminarAsync(int codigo)
        {
            return _api.DeleteAsync("api/clientes/" + codigo);
        }
    }
}
