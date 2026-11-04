using CLIMOV_Facturacion.Modelos.DAO;
using CLIMOV_Facturacion.Modelos.DTO;
using CLIMOV_Facturacion.Servicios;

namespace CLIMOV_Facturacion.Controladores;

/// <summary>
/// Estado y acciones de la pantalla de créditos de la tienda.
/// El selector se arma con las facturas a crédito y, opcionalmente, con el
/// crédito vigente que BanQuito devuelve para una cédula.
/// </summary>
public sealed class CreditosControlador(
    FacturaDao facturaDao,
    AmortizacionDao amortizacionDao,
    ClienteDao clienteDao)
{
    private readonly Dictionary<int, AmortizacionDTO> _amortizaciones = new();

    public List<CreditoOpcionDTO> Creditos { get; private set; } = new();

    public string FiltroCedula { get; set; } = string.Empty;

    public int CreditoSeleccionado { get; private set; }

    public CreditoOpcionDTO? OpcionSeleccionada =>
        Creditos.FirstOrDefault(credito => credito.NumCredito == CreditoSeleccionado);

    public AmortizacionDTO? Amortizacion { get; private set; }

    public bool Cargando { get; private set; }

    public bool CargandoAmortizacion { get; private set; }

    public string? Error { get; private set; }

    public string? ErrorAmortizacion { get; private set; }

    public string? Mensaje { get; private set; }

    public int CuotasPagadas => Amortizacion?.Cuotas.Count(cuota => cuota.Pagada) ?? 0;

    public int CuotasPendientes => Amortizacion?.Cuotas.Count(cuota => !cuota.Pagada) ?? 0;

    public decimal TotalPagado => Math.Round(
        Amortizacion?.Cuotas.Where(cuota => cuota.Pagada).Sum(cuota => cuota.ValorCuota) ?? 0m,
        2,
        MidpointRounding.AwayFromZero);

    public decimal TotalPendiente => Math.Round(
        Amortizacion?.Cuotas.Where(cuota => !cuota.Pagada).Sum(cuota => cuota.ValorCuota) ?? 0m,
        2,
        MidpointRounding.AwayFromZero);

    public CuotaDTO? ProximaCuota => Amortizacion?.Cuotas
        .Where(cuota => !cuota.Pagada)
        .OrderBy(cuota => cuota.NumCuota)
        .FirstOrDefault();

    public async Task CargarAsync()
    {
        Cargando = true;
        Error = null;
        ErrorAmortizacion = null;
        Mensaje = null;
        CreditoSeleccionado = 0;
        Amortizacion = null;
        _amortizaciones.Clear();

        try
        {
            var facturas = await facturaDao.ListarAsync();
            Creditos = CrearOpciones(facturas);
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            Creditos = new();
        }
        finally
        {
            Cargando = false;
        }
    }

    public async Task BuscarAsync()
    {
        Error = null;
        ErrorAmortizacion = null;
        Mensaje = null;

        var cedula = (FiltroCedula ?? string.Empty).Trim();
        if (cedula.Length == 0)
        {
            await CargarAsync();
            return;
        }

        if (cedula.Length != 10)
        {
            Error = "La cédula debe tener exactamente 10 dígitos.";
            return;
        }

        Cargando = true;
        CreditoSeleccionado = 0;
        Amortizacion = null;
        _amortizaciones.Clear();

        try
        {
            var facturas = await facturaDao.ListarPorCedulaAsync(cedula);
            var opciones = CrearOpciones(facturas);
            string? nombre = opciones.FirstOrDefault()?.NombreCliente;
            AmortizacionDTO? creditoBanquito = null;

            try
            {
                var cliente = await clienteDao.PorCedulaAsync(cedula);
                nombre = cliente.Nombre;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            if (string.IsNullOrWhiteSpace(Error))
            {
                try
                {
                    creditoBanquito = await amortizacionDao.PorCedulaAsync(cedula);
                    _amortizaciones[creditoBanquito.NumCredito] = creditoBanquito;

                    var existente = opciones.FirstOrDefault(opcion => opcion.NumCredito == creditoBanquito.NumCredito);
                    if (existente is null)
                    {
                        opciones.Add(new CreditoOpcionDTO
                        {
                            NumCredito = creditoBanquito.NumCredito,
                            Cedula = cedula,
                            NombreCliente = nombre ?? string.Empty,
                            Origen = "BanQuito"
                        });
                    }
                    else if (string.IsNullOrWhiteSpace(existente.NombreCliente))
                    {
                        existente.NombreCliente = nombre ?? string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Mensaje = ex.Message;
                }
            }

            Creditos = opciones
                .OrderByDescending(opcion => opcion.Fecha ?? DateTime.MinValue)
                .ThenByDescending(opcion => opcion.NumCredito)
                .ToList();

            if (creditoBanquito is not null)
            {
                CreditoSeleccionado = creditoBanquito.NumCredito;
                Amortizacion = creditoBanquito;
            }
            else if (Creditos.Count > 0)
            {
                await SeleccionarAsync(Creditos[0].NumCredito);
            }
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            Creditos = new();
        }
        finally
        {
            Cargando = false;
        }
    }

    public async Task SeleccionarAsync(int numCredito)
    {
        ErrorAmortizacion = null;

        if (numCredito <= 0)
        {
            CreditoSeleccionado = 0;
            Amortizacion = null;
            return;
        }

        var opcion = Creditos.FirstOrDefault(credito => credito.NumCredito == numCredito);
        if (opcion is null)
        {
            return;
        }

        CreditoSeleccionado = numCredito;
        CargandoAmortizacion = true;

        try
        {
            if (_amortizaciones.TryGetValue(numCredito, out var amortizacionGuardada))
            {
                Amortizacion = amortizacionGuardada;
                return;
            }

            AmortizacionDTO amortizacion;
            if (opcion.NumFactura is int numFactura)
            {
                amortizacion = await amortizacionDao.PorFacturaAsync(numFactura);
            }
            else
            {
                amortizacion = await amortizacionDao.PorCedulaAsync(opcion.Cedula);

                if (amortizacion.NumCredito != numCredito)
                {
                    throw new ApiException(404, "El crédito seleccionado ya no está disponible en BanQuito.");
                }
            }

            _amortizaciones[numCredito] = amortizacion;
            Amortizacion = amortizacion;
        }
        catch (Exception ex)
        {
            Amortizacion = null;
            ErrorAmortizacion = ex.Message;
        }
        finally
        {
            CargandoAmortizacion = false;
        }
    }

    public async Task LimpiarAsync()
    {
        FiltroCedula = string.Empty;
        await CargarAsync();
    }

    private static List<CreditoOpcionDTO> CrearOpciones(IEnumerable<FacturaDTO> facturas) =>
        facturas
            .Where(factura => factura.NumCredito is not null &&
                string.Equals(factura.TipoPago, "CREDITO", StringComparison.OrdinalIgnoreCase))
            .Select(factura => new CreditoOpcionDTO
            {
                NumCredito = factura.NumCredito!.Value,
                NumFactura = factura.NumFactura,
                Cedula = factura.Cedula,
                NombreCliente = factura.NombreCliente,
                Fecha = factura.Fecha,
                Valor = factura.Valor,
                Origen = "Tienda"
            })
            .ToList();
}
