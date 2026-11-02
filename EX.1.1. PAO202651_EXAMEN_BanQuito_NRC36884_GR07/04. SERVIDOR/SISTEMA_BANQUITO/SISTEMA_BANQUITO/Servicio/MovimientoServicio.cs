using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo.DAO;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Modelo.Mapeo;

namespace SISTEMA_BANQUITO.Servicio;

/// <summary>
/// Reglas de negocio de los movimientos (depositos y retiros).
/// Cada operacion de escritura ajusta el saldo de la cuenta.
/// </summary>
public sealed class MovimientoServicio
{
    private readonly CuentaDAO _cuentaDAO;
    private readonly MovimientoDAO _movimientoDAO;

    public MovimientoServicio(CuentaDAO cuentaDAO, MovimientoDAO movimientoDAO)
    {
        _cuentaDAO = cuentaDAO;
        _movimientoDAO = movimientoDAO;
    }

    public async Task<List<RespuestaMovimiento>> ListarAsync(
        CancellationToken cancelacion = default)
    {
        var movimientos = await _movimientoDAO.ListarAsync(cancelacion);
        return movimientos.Select(movimiento => MapeoMovimiento.ADto(movimiento)).ToList();
    }

    public async Task<RespuestaMovimiento?> ObtenerPorCodigoAsync(
        int codMovimiento,
        CancellationToken cancelacion = default)
    {
        var movimiento = await _movimientoDAO.ObtenerPorCodigoAsync(
            codMovimiento, cancelacion);

        return movimiento is null ? null : MapeoMovimiento.ADto(movimiento);
    }

    public async Task<List<RespuestaMovimiento>> ListarPorCuentaAsync(
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

        var movimientos = await _movimientoDAO.ObtenerPorCuentaAsync(
            cuenta.NumCuenta, null, cancelacion);

        return movimientos.Select(movimiento => MapeoMovimiento.ADto(movimiento)).ToList();
    }

    public async Task<List<RespuestaMovimiento>> ListarPorClienteAsync(
        int codCliente,
        CancellationToken cancelacion = default)
    {
        var movimientos = await _movimientoDAO.ObtenerPorClienteAsync(
            codCliente, null, cancelacion);

        return movimientos.Select(movimiento => MapeoMovimiento.ADto(movimiento)).ToList();
    }

    /// <summary>
    /// Registra el movimiento y devuelve el saldo resultante de la cuenta.
    /// </summary>
    public async Task<RespuestaMovimiento> CrearAsync(
        SolicitudMovimiento solicitud,
        CancellationToken cancelacion = default)
    {
        var movimiento = MapeoMovimiento.AEntidad(solicitud);
        var saldo = await _movimientoDAO.CrearAsync(movimiento, cancelacion);

        return MapeoMovimiento.ADto(movimiento, saldo);
    }

    public async Task<RespuestaGenerica> ActualizarAsync(
        int codMovimiento,
        SolicitudMovimiento solicitud,
        CancellationToken cancelacion = default)
    {
        var movimiento = MapeoMovimiento.AEntidad(solicitud);
        movimiento.CodMovimiento = codMovimiento;

        await _movimientoDAO.ActualizarAsync(movimiento, cancelacion);

        return new RespuestaGenerica
        {
            Exitoso = true,
            Mensaje = $"Movimiento {codMovimiento} actualizado correctamente.",
            ProcesoHttp = "ACTUALIZAR"
        };
    }

    public async Task<RespuestaGenerica> EliminarAsync(
        int codMovimiento,
        CancellationToken cancelacion = default)
    {
        var eliminado = await _movimientoDAO.EliminarAsync(codMovimiento, cancelacion);

        if (!eliminado)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El movimiento {codMovimiento} no existe.");
        }

        return new RespuestaGenerica
        {
            Exitoso = true,
            Mensaje = $"Movimiento {codMovimiento} eliminado correctamente.",
            ProcesoHttp = "ELIMINAR"
        };
    }
}
