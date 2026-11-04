using System.Collections.Generic;
using System.Threading.Tasks;
using CLICON_Facturacion.Modelos;
using CLICON_Facturacion.Servicios;

namespace CLICON_Facturacion.Controladores
{
    /// <summary>
    /// Controlador de la vista de clientes. Encapsula el acceso al
    /// ClienteControlador del API de facturacion.
    /// </summary>
    public sealed class ClienteControlador
    {
        private readonly ClienteServicio _servicio;

        public ClienteControlador()
        {
            _servicio = new ClienteServicio();
        }

        public Task<List<Cliente>> ListarAsync()
        {
            return _servicio.ListarAsync();
        }

        public Task<Cliente> BuscarAsync(int codigo)
        {
            return _servicio.BuscarAsync(codigo);
        }

        public Task<Cliente> BuscarPorCedulaAsync(string cedula)
        {
            return _servicio.BuscarPorCedulaAsync(cedula);
        }

        public Task<RespuestaApi> CrearAsync(Cliente cliente)
        {
            return _servicio.CrearAsync(ARequest(cliente));
        }

        public Task<RespuestaApi> ActualizarAsync(Cliente cliente)
        {
            return _servicio.ActualizarAsync(cliente.CodCliente, ARequest(cliente));
        }

        public Task EliminarAsync(int codigo)
        {
            return _servicio.EliminarAsync(codigo);
        }

        private static ClienteRequest ARequest(Cliente cliente)
        {
            return new ClienteRequest
            {
                Cedula = (cliente.Cedula ?? string.Empty).Trim(),
                Nombre = (cliente.Nombre ?? string.Empty).Trim()
            };
        }
    }
}
