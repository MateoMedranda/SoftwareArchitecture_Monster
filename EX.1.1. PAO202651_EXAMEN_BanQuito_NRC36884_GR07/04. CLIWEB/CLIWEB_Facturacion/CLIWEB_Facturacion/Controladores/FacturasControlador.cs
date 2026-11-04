using CLIWEB_Facturacion.Modelos.DAO;
using CLIWEB_Facturacion.Modelos.DTO;

namespace CLIWEB_Facturacion.Controladores;

/// <summary>Estado y acciones de la pantalla de facturas.</summary>
public sealed class FacturasControlador(FacturaDao facturaDao, AmortizacionDao amortizacionDao)
{
    public List<FacturaDTO> Facturas { get; private set; } = new();

    public string FiltroCedula { get; set; } = string.Empty;

    public int? NumeroBusqueda { get; set; }

    public bool Cargando { get; private set; }

    public bool CargandoAmortizacion { get; private set; }

    public string? Error { get; private set; }

    public FacturaDTO? Seleccionada { get; private set; }

    public bool ModalDetalle { get; private set; }

    public bool ModalAmortizacion { get; private set; }

    public AmortizacionDTO? Amortizacion { get; private set; }

    public async Task CargarAsync()
    {
        Cargando = true;
        Error = null;

        try
        {
            Facturas = await facturaDao.ListarAsync();
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

    public async Task BuscarAsync()
    {
        Cargando = true;
        Error = null;

        try
        {
            if (!string.IsNullOrWhiteSpace(FiltroCedula))
            {
                var cedula = FiltroCedula.Trim();
                if (cedula.Length != 10)
                {
                    throw new Exception("La cédula debe tener exactamente 10 dígitos.");
                }

                Facturas = await facturaDao.ListarPorCedulaAsync(cedula);
            }
            else if (NumeroBusqueda is > 0)
            {
                Facturas = new List<FacturaDTO> { await facturaDao.PorNumeroAsync(NumeroBusqueda.Value) };
            }
            else
            {
                Facturas = await facturaDao.ListarAsync();
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

    public async Task LimpiarFiltrosAsync()
    {
        FiltroCedula = string.Empty;
        NumeroBusqueda = null;
        await CargarAsync();
    }

    public void VerDetalle(FacturaDTO factura)
    {
        Seleccionada = factura;
        Error = null;
        ModalDetalle = true;
    }

    public void CerrarDetalle() => ModalDetalle = false;

    public void CerrarAmortizacion() => ModalAmortizacion = false;

    public async Task VerAmortizacionAsync(FacturaDTO factura)
    {
        if (factura.NumCredito is null)
        {
            Error = "La factura se pagó en efectivo y no tiene crédito asociado.";
            return;
        }

        CargandoAmortizacion = true;
        Error = null;

        try
        {
            Amortizacion = await amortizacionDao.PorFacturaAsync(factura.NumFactura);
            Seleccionada = factura;
            ModalDetalle = false;
            ModalAmortizacion = true;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            CargandoAmortizacion = false;
        }
    }
}
