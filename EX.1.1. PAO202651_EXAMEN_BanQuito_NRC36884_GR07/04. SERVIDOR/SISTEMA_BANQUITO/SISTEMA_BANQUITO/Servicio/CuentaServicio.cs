using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo.DAO;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Modelo.Mapeo;

namespace SISTEMA_BANQUITO.Servicio;

/// <summary>
/// Reglas de negocio de las cuentas del core de BanQuito.
/// </summary>
public sealed class CuentaServicio
{
    private readonly ClienteDAO _clienteDAO;
    private readonly CuentaDAO _cuentaDAO;
    private readonly MovimientoDAO _movimientoDAO;
    private readonly CreditoDAO _creditoDAO;

    public CuentaServicio(
        ClienteDAO clienteDAO,
        CuentaDAO cuentaDAO,
        MovimientoDAO movimientoDAO,
        CreditoDAO creditoDAO)
    {
        _clienteDAO = clienteDAO;
        _cuentaDAO = cuentaDAO;
        _movimientoDAO = movimientoDAO;
        _creditoDAO = creditoDAO;
    }

    public async Task<List<RespuestaCuenta>> ListarAsync(
        CancellationToken cancelacion = default)
    {
        var cuentas = await _cuentaDAO.ListarAsync(cancelacion);
        return cuentas.Select(MapeoCuenta.ADto).ToList();
    }

    public async Task<RespuestaCuenta?> ObtenerPorNumeroAsync(
        string numCuenta,
        CancellationToken cancelacion = default)
    {
        var cuenta = await _cuentaDAO.ObtenerPorNumeroAsync(numCuenta.Trim(), cancelacion);
        return cuenta is null ? null : MapeoCuenta.ADto(cuenta);
    }

    public async Task<List<RespuestaCuenta>> ListarPorClienteAsync(
        int codCliente,
        CancellationToken cancelacion = default)
    {
        var cuentas = await _cuentaDAO.ObtenerPorClienteAsync(codCliente, cancelacion);
        return cuentas.Select(MapeoCuenta.ADto).ToList();
    }

    public async Task<RespuestaGenerica> CrearAsync(
        SolicitudCuenta solicitud,
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

        var cuenta = MapeoCuenta.AEntidad(solicitud);
        cuenta.NumCuenta = await _cuentaDAO.SiguienteNumeroAsync(cancelacion);
        await _cuentaDAO.CrearAsync(cuenta, cancelacion);

        return new RespuestaGenerica
        {
            Exitoso = true,
            Mensaje = $"Cuenta {cuenta.NumCuenta} creada correctamente.",
            ProcesoHttp = "CREAR"
        };
    }

    public async Task<RespuestaGenerica> ActualizarAsync(
        string numCuenta,
        SolicitudCuenta solicitud,
        CancellationToken cancelacion = default)
    {
        var existente = await _cuentaDAO.ObtenerPorNumeroAsync(
            numCuenta.Trim(), cancelacion);

        if (existente is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"La cuenta {numCuenta} no existe.");
        }

        var cliente = await _clienteDAO.ObtenerPorCodigoAsync(
            solicitud.CodCliente, cancelacion);

        if (cliente is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El cliente {solicitud.CodCliente} no existe.");
        }

        var cuenta = MapeoCuenta.AEntidad(solicitud);
        cuenta.NumCuenta = existente.NumCuenta;
        await _cuentaDAO.ActualizarAsync(cuenta, cancelacion);

        return new RespuestaGenerica
        {
            Exitoso = true,
            Mensaje = $"Cuenta {existente.NumCuenta} actualizada correctamente.",
            ProcesoHttp = "ACTUALIZAR"
        };
    }

    public async Task<RespuestaGenerica> EliminarAsync(
        string numCuenta,
        CancellationToken cancelacion = default)
    {
        var existente = await _cuentaDAO.ObtenerPorNumeroAsync(
            numCuenta.Trim(), cancelacion);

        if (existente is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"La cuenta {numCuenta} no existe.");
        }

        if (await _movimientoDAO.ExistePorCuentaAsync(existente.NumCuenta, cancelacion))
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status409Conflict,
                $"No se puede eliminar la cuenta {existente.NumCuenta}: tiene movimientos registrados.");
        }

        if (await _creditoDAO.ExistePorCuentaAsync(existente.NumCuenta, cancelacion))
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status409Conflict,
                $"No se puede eliminar la cuenta {existente.NumCuenta}: tiene creditos registrados.");
        }

        await _cuentaDAO.EliminarAsync(existente.NumCuenta, cancelacion);

        return new RespuestaGenerica
        {
            Exitoso = true,
            Mensaje = $"Cuenta {existente.NumCuenta} eliminada correctamente.",
            ProcesoHttp = "ELIMINAR"
        };
    }
}
