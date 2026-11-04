using CLIMOV_Facturacion.Modelos.DAO;
using CLIMOV_Facturacion.Modelos.DTO;
using CLIMOV_Facturacion.Servicios;

namespace CLIMOV_Facturacion.Controladores;

/// <summary>Estado y acciones de la pantalla "Registrar compra".</summary>
public sealed class CompraControlador(
    FacturacionServicio facturacion,
    ClienteDao clienteDao,
    ProductoDao productoDao)
{
    public FacturacionServicio Facturacion { get; } = facturacion;

    public List<ProductoDTO> Productos { get; private set; } = new();

    public int NumInventario { get; set; }

    public int Cantidad { get; set; } = 1;

    public bool Cargando { get; private set; }

    public bool Procesando { get; private set; }

    public string? Error { get; private set; }

    public string? Mensaje { get; private set; }

    public bool ModalEfectivo { get; private set; }

    public bool ModalCredito { get; private set; }

    public bool ModalAmortizacion { get; private set; }

    public bool ModalExito { get; private set; }

    public bool ModalDetalleFactura { get; private set; }

    public FacturaDTO? FacturaCreada { get; private set; }

    public AmortizacionDTO? Amortizacion { get; private set; }

    public bool TieneCredito => FacturaCreada?.NumCredito is not null;

    public bool PuedePagar => Facturacion.Cliente is not null && Facturacion.Carrito.Count > 0;

    private string _cedula = string.Empty;

    public string Cedula
    {
        get => _cedula;
        set
        {
            _cedula = value ?? string.Empty;
            if (Facturacion.Cliente is not null &&
                !string.Equals(Facturacion.Cliente.Cedula, _cedula.Trim(), StringComparison.Ordinal))
            {
                Facturacion.QuitarCliente();
            }
        }
    }

    public async Task CargarAsync()
    {
        Cargando = true;
        Error = null;

        try
        {
            Productos = await productoDao.ListarAsync();
            if (NumInventario == 0 && Productos.Count > 0)
            {
                NumInventario = Productos[0].NumInventario;
            }

            if (Facturacion.Cliente is not null)
            {
                _cedula = Facturacion.Cliente.Cedula;
            }
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

    public async Task BuscarClienteAsync()
    {
        Error = null;
        Mensaje = null;
        var cedula = (Cedula ?? string.Empty).Trim();

        if (cedula.Length != 10)
        {
            Facturacion.QuitarCliente();
            Error = "La cédula debe tener exactamente 10 dígitos.";
            return;
        }

        try
        {
            var cliente = await clienteDao.PorCedulaAsync(cedula);
            Facturacion.SeleccionarCliente(cliente);
            _cedula = cliente.Cedula;
            Mensaje = $"Cliente: {cliente.Nombre}";
        }
        catch (Exception ex)
        {
            Facturacion.QuitarCliente();
            Error = ex.Message;
        }
    }

    public void AgregarProducto()
    {
        Error = null;
        Mensaje = null;

        var producto = Productos.FirstOrDefault(p => p.NumInventario == NumInventario);
        if (producto is null)
        {
            Error = "Seleccione un producto del catálogo.";
            return;
        }

        try
        {
            Facturacion.Agregar(producto, Cantidad);
            Mensaje = $"{producto.Nombre} agregado al carrito.";
            Cantidad = 1;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }

    public void QuitarProducto(int numInventario) =>
        Facturacion.Quitar(numInventario);

    public void AbrirEfectivo()
    {
        Error = null;
        if (PuedeFacturar())
        {
            ModalEfectivo = true;
        }
    }

    public void AbrirCredito()
    {
        Error = null;
        if (PuedeFacturar())
        {
            ModalCredito = true;
        }
    }

    public void CerrarEfectivo() => ModalEfectivo = false;

    public void CerrarCredito() => ModalCredito = false;

    public void CerrarAmortizacion() => ModalAmortizacion = false;

    public void VerDetalleFactura()
    {
        if (FacturaCreada is null)
        {
            return;
        }

        ModalExito = false;
        ModalDetalleFactura = true;
    }

    public void CerrarDetalleFactura()
    {
        ModalDetalleFactura = false;
        if (FacturaCreada is not null)
        {
            ModalExito = true;
        }
    }

    public void CerrarExito()
    {
        ModalExito = false;
        ModalDetalleFactura = false;
        FacturaCreada = null;
        Amortizacion = null;
        Error = null;
        Mensaje = null;
    }

    public async Task ConfirmarEfectivoAsync()
    {
        Procesando = true;
        Error = null;

        try
        {
            FacturaCreada = await Facturacion.CrearAsync("EFECTIVO", null);
            ModalEfectivo = false;
            ModalExito = true;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            Procesando = false;
        }
    }

    public async Task ConfirmarCreditoAsync(int cuotas)
    {
        Error = null;

        if (cuotas is < 3 or > 18)
        {
            Error = "El plazo del crédito debe estar entre 3 y 18 meses.";
            return;
        }

        Procesando = true;

        try
        {
            FacturaCreada = await Facturacion.CrearAsync("CREDITO", cuotas);
            ModalCredito = false;
            ModalExito = true;

            try
            {
                Amortizacion = await Facturacion.ObtenerAmortizacionPorFacturaAsync(FacturaCreada.NumFactura);
            }
            catch
            {
                // La factura ya quedó registrada; la tabla se puede reintentar.
                Amortizacion = null;
            }
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            Procesando = false;
        }
    }

    public async Task VerAmortizacionAsync()
    {
        if (FacturaCreada is null)
        {
            return;
        }

        Error = null;
        Procesando = true;

        try
        {
            Amortizacion ??= await Facturacion.ObtenerAmortizacionPorFacturaAsync(FacturaCreada.NumFactura);
            ModalExito = false;
            ModalAmortizacion = true;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            Procesando = false;
        }
    }

    private bool PuedeFacturar()
    {
        if (Facturacion.Cliente is null)
        {
            Error = "Busque y seleccione un cliente por cédula.";
            return false;
        }

        if (Facturacion.Carrito.Count == 0)
        {
            Error = "Agregue al menos un producto antes de facturar.";
            return false;
        }

        return true;
    }
}
