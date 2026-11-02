using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo.DAO;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Modelo.Mapeo;

namespace SISTEMA_BANQUITO.Servicio;

/// <summary>
/// Consulta de la tabla de amortizacion y pago de cuotas.
/// </summary>
public sealed class AmortizacionServicio
{
    private readonly CreditoDAO _creditoDAO;
    private readonly AmortizacionDAO _amortizacionDAO;

    public AmortizacionServicio(
        CreditoDAO creditoDAO,
        AmortizacionDAO amortizacionDAO)
    {
        _creditoDAO = creditoDAO;
        _amortizacionDAO = amortizacionDAO;
    }

    /// <summary>
    /// Consulta por numero de credito o por codigo de cliente.
    /// Si llega CodCliente se devuelve la tabla de su credito mas reciente.
    /// </summary>
    public async Task<RespuestaTablaAmortizacionDTO> ObtenerTablaAmortizacionAsync(
        SolicitudTablaAmortizacionDTO solicitud,
        CancellationToken cancelacion = default)
    {
        if (solicitud.NumCredito is > 0)
        {
            return await ObtenerTablaAmortizacionPorCreditoAsync(
                solicitud.NumCredito.Value, cancelacion);
        }

        if (solicitud.CodCliente is > 0)
        {
            return await ObtenerTablaAmortizacionPorClienteAsync(
                solicitud.CodCliente.Value, cancelacion);
        }

        throw new ExcepcionNegocio(
            StatusCodes.Status400BadRequest,
            "Debe indicar el numero de credito o el codigo de cliente.");
    }

    public async Task<RespuestaTablaAmortizacionDTO> ObtenerTablaAmortizacionPorCreditoAsync(
        int numCredito,
        CancellationToken cancelacion = default)
    {
        var credito = await _creditoDAO.ObtenerPorNumeroAsync(numCredito, cancelacion);
        if (credito is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El credito {numCredito} no existe.");
        }

        var cuotas = await _amortizacionDAO.ObtenerPorCreditoAsync(
            numCredito, cancelacion);

        return new RespuestaTablaAmortizacionDTO
        {
            NumCredito = numCredito,
            Cuotas = cuotas.Select(MapeoAmortizacion.ADto).ToList()
        };
    }

    public async Task<RespuestaTablaAmortizacionDTO> ObtenerTablaAmortizacionPorClienteAsync(
        int codCliente,
        CancellationToken cancelacion = default)
    {
        var creditos = await _creditoDAO.ObtenerPorClienteAsync(
            codCliente, cancelacion);

        if (creditos.Count == 0)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El cliente {codCliente} no tiene creditos registrados.");
        }

        return await ObtenerTablaAmortizacionPorCreditoAsync(
            creditos[0].NumCredito, cancelacion);
    }

    public async Task<CuotaDTO?> ObtenerCuotaAsync(
        int numCredito,
        int numCuota,
        CancellationToken cancelacion = default)
    {
        var cuota = await _amortizacionDAO.ObtenerCuotaAsync(
            numCredito, numCuota, cancelacion);

        return cuota is null ? null : MapeoAmortizacion.ADto(cuota);
    }

    /// <summary>
    /// Paga una cuota de amortizacion descontando el valor de la cuenta indicada.
    /// </summary>
    public async Task<RespuestaGenerica> PagarCuotaAsync(
        SolicitudPagoCuota solicitud,
        CancellationToken cancelacion = default)
    {
        var credito = await _creditoDAO.ObtenerPorNumeroAsync(
            solicitud.NumCredito, cancelacion);

        if (credito is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El credito {solicitud.NumCredito} no existe.");
        }

        var saldo = await _amortizacionDAO.RegistrarPagoAsync(
            solicitud.NumCredito,
            solicitud.NumCuota,
            solicitud.NumCuenta.Trim(),
            cancelacion);

        return new RespuestaGenerica
        {
            Exitoso = true,
            Mensaje = $"Cuota {solicitud.NumCuota} del credito {solicitud.NumCredito} " +
                      $"pagada correctamente. Saldo de la cuenta: {saldo:0.00}.",
            ProcesoHttp = "PAGAR"
        };
    }
}
