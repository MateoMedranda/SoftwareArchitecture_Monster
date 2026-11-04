using CLIWEB_Facturacion.Modelos.DTO;
using CLIWEB_Facturacion.Servicios;

namespace CLIWEB_Facturacion.Controladores;

/// <summary>Estado y acciones de la pantalla de clientes.</summary>
public sealed class ClientesControlador(ClientesServicio servicio)
{
    public List<ClienteDTO> Clientes { get; private set; } = new();

    public string Filtro { get; set; } = string.Empty;

    public string CedulaBusqueda { get; set; } = string.Empty;

    public bool Cargando { get; private set; }

    public string? Error { get; private set; }

    public string? Mensaje { get; private set; }

    public IEnumerable<ClienteDTO> ClientesFiltrados
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Filtro))
            {
                return Clientes;
            }

            var texto = Filtro.Trim();
            return Clientes.Where(cliente =>
                cliente.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                cliente.Cedula.Contains(texto, StringComparison.OrdinalIgnoreCase));
        }
    }

    // Estado del formulario de alta/edición.
    public bool ModalAbierto { get; private set; }
    public bool Guardando { get; private set; }
    public string? ErrorModal { get; private set; }
    public ClienteDTO? Editando { get; private set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    // Estado de la confirmación de eliminación.
    public ClienteDTO? ClienteAEliminar { get; private set; }
    public bool Eliminando { get; private set; }
    public string? ErrorEliminar { get; private set; }

    public async Task CargarAsync()
    {
        Cargando = true;
        Error = null;

        try
        {
            Clientes = await servicio.ListarAsync();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            Cargando = false;
        }
    }

    public async Task BuscarPorCedulaAsync()
    {
        Error = null;
        Mensaje = null;

        try
        {
            var cliente = await servicio.PorCedulaAsync(CedulaBusqueda);
            Filtro = cliente.Cedula;
            Mensaje = $"Cliente encontrado: {cliente.Nombre}";
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }

    public void AbrirCrear()
    {
        Editando = null;
        Cedula = string.Empty;
        Nombre = string.Empty;
        ErrorModal = null;
        ModalAbierto = true;
    }

    public void AbrirEditar(ClienteDTO cliente)
    {
        Editando = cliente;
        Cedula = cliente.Cedula;
        Nombre = cliente.Nombre;
        ErrorModal = null;
        ModalAbierto = true;
    }

    public void CerrarModal()
    {
        ModalAbierto = false;
        ErrorModal = null;
    }

    public async Task GuardarAsync()
    {
        Guardando = true;
        ErrorModal = null;

        try
        {
            if (Editando is null)
            {
                await servicio.CrearAsync(Cedula, Nombre);
                Mensaje = "Cliente creado correctamente.";
            }
            else
            {
                await servicio.ActualizarAsync(Editando.CodCliente, Cedula, Nombre);
                Mensaje = "Cliente actualizado correctamente.";
            }

            ModalAbierto = false;
            await CargarAsync();
        }
        catch (Exception ex)
        {
            ErrorModal = ex.Message;
        }
        finally
        {
            Guardando = false;
        }
    }

    public void PedirEliminar(ClienteDTO cliente)
    {
        ClienteAEliminar = cliente;
        ErrorEliminar = null;
    }

    public void CancelarEliminar()
    {
        ClienteAEliminar = null;
        ErrorEliminar = null;
    }

    public async Task ConfirmarEliminarAsync()
    {
        if (ClienteAEliminar is null)
        {
            return;
        }

        Eliminando = true;
        ErrorEliminar = null;

        try
        {
            await servicio.EliminarAsync(ClienteAEliminar.CodCliente);
            ClienteAEliminar = null;
            Mensaje = "Cliente eliminado correctamente.";
            await CargarAsync();
        }
        catch (Exception ex)
        {
            ErrorEliminar = ex.Message;
        }
        finally
        {
            Eliminando = false;
        }
    }
}
