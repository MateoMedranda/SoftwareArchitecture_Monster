using System.Globalization;
using CLIWEB_Facturacion.Modelos.DTO;
using CLIWEB_Facturacion.Servicios;

namespace CLIWEB_Facturacion.Controladores;

/// <summary>Estado y acciones de la pantalla de catálogo.</summary>
public sealed class CatalogoControlador(CatalogoServicio servicio)
{
    public List<ProductoDTO> Productos { get; private set; } = new();

    public string Filtro { get; set; } = string.Empty;

    public bool Cargando { get; private set; }

    public string? Error { get; private set; }

    public string? Mensaje { get; private set; }

    public IEnumerable<ProductoDTO> ProductosFiltrados
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Filtro))
            {
                return Productos;
            }

            var texto = Filtro.Trim();
            return Productos.Where(producto =>
                producto.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                producto.NumInventario.ToString().Contains(texto, StringComparison.OrdinalIgnoreCase));
        }
    }

    // Estado del formulario de alta/edición.
    public bool ModalAbierto { get; private set; }
    public bool Guardando { get; private set; }
    public string? ErrorModal { get; private set; }
    public ProductoDTO? Editando { get; private set; }
    public string Nombre { get; set; } = string.Empty;
    public string PrecioTexto { get; set; } = string.Empty;
    public decimal Precio { get; private set; }

    // Estado de la confirmación de eliminación.
    public ProductoDTO? ProductoAEliminar { get; private set; }
    public bool Eliminando { get; private set; }
    public string? ErrorEliminar { get; private set; }

    public async Task CargarAsync()
    {
        Cargando = true;
        Error = null;

        try
        {
            Productos = await servicio.ListarAsync();
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

    public void AbrirCrear()
    {
        Editando = null;
        Nombre = string.Empty;
        Precio = 0;
        PrecioTexto = string.Empty;
        ErrorModal = null;
        ModalAbierto = true;
    }

    public void AbrirEditar(ProductoDTO producto)
    {
        Editando = producto;
        Nombre = producto.Nombre;
        Precio = producto.Precio;
        PrecioTexto = producto.Precio.ToString("0.##", CultureInfo.CurrentCulture);
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
        ErrorModal = null;

        if (!TryParsePrecio(PrecioTexto, out var precio))
        {
            ErrorModal = "Ingrese un precio válido mayor que cero. Puede usar coma o punto como separador decimal.";
            return;
        }

        Precio = precio;
        Guardando = true;

        try
        {
            if (Editando is null)
            {
                await servicio.CrearAsync(Nombre, Precio);
                Mensaje = "Producto creado correctamente.";
            }
            else
            {
                await servicio.ActualizarAsync(Editando.NumInventario, Nombre, Precio);
                Mensaje = "Producto actualizado correctamente.";
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

    public void PedirEliminar(ProductoDTO producto)
    {
        ProductoAEliminar = producto;
        ErrorEliminar = null;
    }

    public void CancelarEliminar()
    {
        ProductoAEliminar = null;
        ErrorEliminar = null;
    }

    public async Task ConfirmarEliminarAsync()
    {
        if (ProductoAEliminar is null)
        {
            return;
        }

        Eliminando = true;
        ErrorEliminar = null;

        try
        {
            await servicio.EliminarAsync(ProductoAEliminar.NumInventario);
            ProductoAEliminar = null;
            Mensaje = "Producto eliminado correctamente.";
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

    private static bool TryParsePrecio(string? texto, out decimal precio)
    {
        precio = 0;

        if (string.IsNullOrWhiteSpace(texto))
        {
            return false;
        }

        var limpio = texto.Trim().Replace("$", string.Empty).Replace(" ", string.Empty);
        if (limpio.Contains(',') && limpio.Contains('.'))
        {
            // Formato 1.234,56: los puntos son separadores de miles.
            limpio = limpio.Replace(".", string.Empty).Replace(',', '.');
        }
        else
        {
            limpio = limpio.Replace(',', '.');
        }

        return decimal.TryParse(limpio, NumberStyles.Number, CultureInfo.InvariantCulture, out precio)
            && precio > 0;
    }
}
