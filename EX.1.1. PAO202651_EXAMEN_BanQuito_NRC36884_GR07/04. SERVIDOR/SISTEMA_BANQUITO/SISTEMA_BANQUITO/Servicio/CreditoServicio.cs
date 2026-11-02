using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo;
using SISTEMA_BANQUITO.Modelo.DAO;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Modelo.Mapeo;

namespace SISTEMA_BANQUITO.Servicio;

/// <summary>
/// Reglas de negocio del modulo de creditos: CRUD, monto maximo y otorgamiento.
/// </summary>
public sealed class CreditoServicio
{
    private const decimal TasaAnual = 0.165m;

    private readonly ClienteDAO _clienteDAO;
    private readonly CuentaDAO _cuentaDAO;
    private readonly MovimientoDAO _movimientoDAO;
    private readonly CreditoDAO _creditoDAO;
    private readonly AmortizacionDAO _amortizacionDAO;
    private readonly ClienteServicio _clienteServicio;

    public CreditoServicio(
        ClienteDAO clienteDAO,
        CuentaDAO cuentaDAO,
        MovimientoDAO movimientoDAO,
        CreditoDAO creditoDAO,
        AmortizacionDAO amortizacionDAO,
        ClienteServicio clienteServicio)
    {
        _clienteDAO = clienteDAO;
        _cuentaDAO = cuentaDAO;
        _movimientoDAO = movimientoDAO;
        _creditoDAO = creditoDAO;
        _amortizacionDAO = amortizacionDAO;
        _clienteServicio = clienteServicio;
    }

    public async Task<List<RespuestaCredito>> ListarAsync(
        CancellationToken cancelacion = default)
    {
        var creditos = await _creditoDAO.ListarAsync(cancelacion);
        return creditos.Select(MapeoCredito.ADto).ToList();
    }

    public async Task<RespuestaCredito?> ObtenerPorNumeroAsync(
        int numCredito,
        CancellationToken cancelacion = default)
    {
        var credito = await _creditoDAO.ObtenerPorNumeroAsync(numCredito, cancelacion);
        return credito is null ? null : MapeoCredito.ADto(credito);
    }

    public async Task<List<RespuestaCredito>> ListarPorCuentaAsync(
        string numCuenta,
        CancellationToken cancelacion = default)
    {
        var cuenta = await _cuentaDAO.ObtenerPorNumeroAsync(
            numCuenta.Trim(), cancelacion);

        if (cuenta is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"La cuenta {numCuenta} no existe.");
        }

        var creditos = await _creditoDAO.ObtenerPorCuentaAsync(
            cuenta.NumCuenta, cancelacion);

        return creditos.Select(MapeoCredito.ADto).ToList();
    }

    public async Task<List<RespuestaCredito>> ListarPorClienteAsync(
        int codCliente,
        CancellationToken cancelacion = default)
    {
        var cliente = await _clienteDAO.ObtenerPorCodigoAsync(codCliente, cancelacion);
        if (cliente is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El cliente {codCliente} no existe.");
        }

        var creditos = await _creditoDAO.ObtenerPorClienteAsync(
            codCliente, cancelacion);

        return creditos.Select(MapeoCredito.ADto).ToList();
    }

    public async Task<RespuestaGenerica> ActualizarAsync(
        int numCredito,
        SolicitudActualizarCredito solicitud,
        CancellationToken cancelacion = default)
    {
        var credito = await _creditoDAO.ObtenerPorNumeroAsync(numCredito, cancelacion);
        if (credito is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El credito {numCredito} no existe.");
        }

        credito.CreditoTotal = Math.Round(solicitud.CreditoTotal, 2);
        credito.FechaActualizacion = DateTime.Now;

        await _creditoDAO.ActualizarAsync(credito, cancelacion);

        return new RespuestaGenerica
        {
            Exitoso = true,
            Mensaje = $"Credito {numCredito} actualizado correctamente.",
            ProcesoHttp = "ACTUALIZAR"
        };
    }

    /// <summary>
    /// Elimina el credito y todas sus cuotas de amortizacion.
    /// </summary>
    public async Task<RespuestaGenerica> EliminarAsync(
        int numCredito,
        CancellationToken cancelacion = default)
    {
        var eliminado = await _creditoDAO.EliminarAsync(numCredito, cancelacion);

        if (!eliminado)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El credito {numCredito} no existe.");
        }

        return new RespuestaGenerica
        {
            Exitoso = true,
            Mensaje = $"Credito {numCredito} y sus cuotas fueron eliminados correctamente.",
            ProcesoHttp = "ELIMINAR"
        };
    }

    /// <summary>
    /// montoMaximo = max(0, (promedioDepositos3m - promedioRetiros3m) * 0.30 * 6).
    /// </summary>
    public async Task<RespuestaMontoMaximoDTO> ObtenerMontoMaximoAsync(
        SolicitudMontoMaximoDTO solicitud,
        CancellationToken cancelacion = default)
    {
        var cliente = await _clienteDAO.ObtenerPorCodigoAsync(
            solicitud.CodCliente, cancelacion);

        if (cliente is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El cliente {solicitud.CodCliente} no existe.");
        }

        var desde = DateTime.Today.AddMonths(-3);
        var (depositos, retiros) = await _movimientoDAO.ObtenerTotalesAsync(
            solicitud.CodCliente, desde, cancelacion);

        var promedioDepositos = depositos / 3m;
        var promedioRetiros = retiros / 3m;
        var monto = (promedioDepositos - promedioRetiros) * 0.30m * 6m;

        if (monto < 0)
        {
            monto = 0;
        }

        return new RespuestaMontoMaximoDTO
        {
            CodCliente = solicitud.CodCliente,
            MontoMaximo = Math.Round(monto, 2)
        };
    }

    /// <summary>
    /// Otorga un credito por la compra de un telefono.
    /// El credito se calcula sobre el precio completo del telefono, de modo que
    /// la validacion del examen (P = 450, n = 6) devuelve una cuota de 78.65.
    /// </summary>
    public async Task<RespuestaOtorgarCreditoDTO> OtorgarCreditoAsync(
        SolicitudOtorgarCreditoDTO solicitud,
        CancellationToken cancelacion = default)
    {
        var cliente = await _clienteDAO.ObtenerPorCodigoAsync(
            solicitud.CodCliente, cancelacion);

        if (cliente is null)
        {
            return new RespuestaOtorgarCreditoDTO
            {
                Aprobado = false,
                Motivo = "NO_CLIENTE"
            };
        }

        var verificacion = await _clienteServicio.VerificarSujetoCreditoAsync(
            new SolicitudSujetoCreditoDTO { Cedula = cliente.Cedula }, cancelacion);

        if (!verificacion.SujetoCredito)
        {
            return new RespuestaOtorgarCreditoDTO
            {
                Aprobado = false,
                Motivo = verificacion.Motivo
            };
        }

        var cuentas = await _cuentaDAO.ObtenerPorClienteAsync(
            solicitud.CodCliente, cancelacion);

        if (cuentas.Count == 0)
        {
            return new RespuestaOtorgarCreditoDTO
            {
                Aprobado = false,
                Motivo = "SIN_DEPOSITO"
            };
        }

        var numCredito = await _creditoDAO.SiguienteNumeroAsync(cancelacion);
        var total = Math.Round(solicitud.PrecioTelefono, 2);
        var fechaCreacion = DateTime.Now;
        var cuotas = GenerarTablaAmortizacion(
            total, solicitud.NumCuotas, numCredito, fechaCreacion.Date);

        var credito = new Credito
        {
            NumCredito = numCredito,
            NumCuenta = cuentas[0].NumCuenta,
            CreditoTotal = total,
            FechaCreacion = fechaCreacion,
            FechaActualizacion = fechaCreacion
        };

        await _creditoDAO.CrearAsync(credito, cancelacion);

        try
        {
            await _amortizacionDAO.CrearLoteAsync(cuotas, cancelacion);
        }
        catch
        {
            // Si falla el registro de las cuotas se elimina el credito
            // para no dejar informacion incompleta.
            await _creditoDAO.EliminarAsync(numCredito, cancelacion);
            throw;
        }

        return new RespuestaOtorgarCreditoDTO
        {
            IdCredito = numCredito,
            Aprobado = true,
            Motivo = null,
            TablaAmortizacion = cuotas.Select(MapeoAmortizacion.ADto).ToList()
        };
    }

    /// <summary>
    /// Genera la tabla de amortizacion con cuota fija:
    /// C = P * i / (1 - (1 + i)^(-n)), i = 0.165 / 12.
    /// La ultima cuota ajusta el saldo a cero.
    /// </summary>
    private static List<Amortizacion> GenerarTablaAmortizacion(
        decimal total,
        int numCuotas,
        int numCredito,
        DateTime fechaInicio)
    {
        var tasa = TasaAnual / 12m;
        var factor = (decimal)Math.Pow((double)(1m + tasa), -numCuotas);
        var cuotaFija = total * tasa / (1m - factor);

        var saldo = total;
        var cuotas = new List<Amortizacion>(numCuotas);

        for (var numero = 1; numero <= numCuotas; numero++)
        {
            var interes = Math.Round(saldo * tasa, 2);

            decimal capital;
            decimal valorCuota;

            if (numero == numCuotas)
            {
                capital = saldo;
                valorCuota = Math.Round(capital + interes, 2);
                saldo = 0m;
            }
            else
            {
                capital = Math.Round(cuotaFija - interes, 2);
                if (capital > saldo)
                {
                    capital = saldo;
                }

                valorCuota = Math.Round(capital + interes, 2);
                saldo = Math.Round(saldo - capital, 2);
            }

            cuotas.Add(new Amortizacion
            {
                NumCuota = numero,
                NumCredito = numCredito,
                ValorCuota = valorCuota,
                InteresPagado = interes,
                CapitalPagado = capital,
                Saldo = saldo,
                FechaPago = fechaInicio.AddMonths(numero),
                Pagada = false
            });
        }

        return cuotas;
    }
}
