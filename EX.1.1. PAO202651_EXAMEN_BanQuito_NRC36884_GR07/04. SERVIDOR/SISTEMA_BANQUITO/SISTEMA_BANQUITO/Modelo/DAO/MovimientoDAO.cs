using System.Data;
using Microsoft.Data.SqlClient;
using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo;

namespace SISTEMA_BANQUITO.Modelo.DAO;

/// <summary>
/// Acceso a datos de la tabla MOVIMIENTO.
/// El campo TIPO contiene DEP (deposito) o RET (retiro).
/// Cada operacion que crea, actualiza o elimina un movimiento ajusta el
/// saldo de la cuenta dentro de la misma transaccion.
/// </summary>
public sealed class MovimientoDAO
{
    private const string Columnas = "COD_MOVIMIENTO, NUM_CUENTA, TIPO, VALOR, FECHA";

    private readonly Conexion _conexion;

    public MovimientoDAO(Conexion conexion)
    {
        _conexion = conexion;
    }

    public async Task<List<Movimiento>> ListarAsync(CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM MOVIMIENTO
            ORDER BY FECHA, COD_MOVIMIENTO;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        return await LeerMovimientosAsync(comando, cancelacion);
    }

    public async Task<Movimiento?> ObtenerPorCodigoAsync(
        int codMovimiento,
        CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM MOVIMIENTO
            WHERE COD_MOVIMIENTO = @codMovimiento;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@codMovimiento", SqlDbType.Int).Value = codMovimiento;

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        return await lector.ReadAsync(cancelacion) ? Mapear(lector) : null;
    }

    public async Task<List<Movimiento>> ObtenerPorCuentaAsync(
        string numCuenta,
        DateTime? desde = null,
        CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM MOVIMIENTO
            WHERE NUM_CUENTA = @numCuenta
              AND (@desde IS NULL OR FECHA >= @desde)
            ORDER BY FECHA, COD_MOVIMIENTO;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = numCuenta;
        comando.Parameters.Add("@desde", SqlDbType.DateTime).Value =
            desde.HasValue ? desde.Value : DBNull.Value;

        return await LeerMovimientosAsync(comando, cancelacion);
    }

    public async Task<List<Movimiento>> ObtenerPorClienteAsync(
        int codCliente,
        DateTime? desde = null,
        CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM MOVIMIENTO M
            INNER JOIN CUENTA C ON C.NUM_CUENTA = M.NUM_CUENTA
            WHERE C.COD_CLIENTE = @codCliente
              AND (@desde IS NULL OR M.FECHA >= @desde)
            ORDER BY M.FECHA, M.COD_MOVIMIENTO;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@codCliente", SqlDbType.Int).Value = codCliente;
        comando.Parameters.Add("@desde", SqlDbType.DateTime).Value =
            desde.HasValue ? desde.Value : DBNull.Value;

        return await LeerMovimientosAsync(comando, cancelacion);
    }

    /// <summary>
    /// Suma de depositos y retiros de todas las cuentas del cliente
    /// desde una fecha indicada. Se usa en el calculo del monto maximo.
    /// </summary>
    public async Task<(decimal Depositos, decimal Retiros)> ObtenerTotalesAsync(
        int codCliente,
        DateTime desde,
        CancellationToken cancelacion = default)
    {
        const string consulta = """
            SELECT
                ISNULL(SUM(CASE WHEN M.TIPO = 'DEP' THEN M.VALOR ELSE 0 END), 0) AS DEPOSITOS,
                ISNULL(SUM(CASE WHEN M.TIPO = 'RET' THEN M.VALOR ELSE 0 END), 0) AS RETIROS
            FROM MOVIMIENTO M
            INNER JOIN CUENTA C ON C.NUM_CUENTA = M.NUM_CUENTA
            WHERE C.COD_CLIENTE = @codCliente
              AND M.FECHA >= @desde;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@codCliente", SqlDbType.Int).Value = codCliente;
        comando.Parameters.Add("@desde", SqlDbType.DateTime).Value = desde;

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        if (!await lector.ReadAsync(cancelacion))
        {
            return (0m, 0m);
        }

        return (lector.GetDecimal(0), lector.GetDecimal(1));
    }

    /// <summary>
    /// Indica si la cuenta tiene movimientos registrados.
    /// </summary>
    public async Task<bool> ExistePorCuentaAsync(
        string numCuenta,
        CancellationToken cancelacion = default)
    {
        const string consulta = """
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM MOVIMIENTO WHERE NUM_CUENTA = @numCuenta
            ) THEN 1 ELSE 0 END;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = numCuenta;

        var resultado = await comando.ExecuteScalarAsync(cancelacion);
        return Convert.ToInt32(resultado) == 1;
    }

    /// <summary>
    /// Registra el movimiento y actualiza el saldo de la cuenta.
    /// Devuelve el saldo resultante de la cuenta.
    /// </summary>
    public async Task<decimal> CrearAsync(
        Movimiento movimiento,
        CancellationToken cancelacion = default)
    {
        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);
        using var transaccion = conexion.BeginTransaction();

        try
        {
            var saldo = await BloquearSaldoAsync(
                conexion, transaccion, movimiento.NumCuenta, cancelacion);

            var nuevoSaldo = Math.Round(saldo + Efecto(movimiento), 2);
            if (nuevoSaldo < 0)
            {
                throw new ExcepcionNegocio(
                    StatusCodes.Status409Conflict,
                    "Saldo insuficiente para realizar el retiro.");
            }

            movimiento.CodMovimiento = await SiguienteCodigoAsync(
                conexion, transaccion, cancelacion);

            const string insercion = """
                INSERT INTO MOVIMIENTO (COD_MOVIMIENTO, NUM_CUENTA, TIPO, VALOR, FECHA)
                VALUES (@codMovimiento, @numCuenta, @tipo, @valor, @fecha);
                """;

            await using (var comando = new SqlCommand(insercion, conexion, transaccion))
            {
                comando.Parameters.Add("@codMovimiento", SqlDbType.Int).Value = movimiento.CodMovimiento;
                comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = movimiento.NumCuenta;
                comando.Parameters.Add("@tipo", SqlDbType.VarChar, 3).Value = movimiento.Tipo.ToString();
                comando.Parameters.AgregarDecimal("@valor", movimiento.Valor);
                comando.Parameters.Add("@fecha", SqlDbType.DateTime).Value = movimiento.Fecha;

                await comando.ExecuteNonQueryAsync(cancelacion);
            }

            await EscribirSaldoAsync(
                conexion, transaccion, movimiento.NumCuenta, nuevoSaldo, cancelacion);

            transaccion.Commit();
            return nuevoSaldo;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Actualiza un movimiento revirtiendo su efecto anterior y aplicando
    /// el nuevo sobre la cuenta correspondiente. Devuelve el saldo final.
    /// </summary>
    public async Task<decimal> ActualizarAsync(
        Movimiento movimiento,
        CancellationToken cancelacion = default)
    {
        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);
        using var transaccion = conexion.BeginTransaction();

        try
        {
            var anterior = await LeerMovimientoBloqueadoAsync(
                conexion, transaccion, movimiento.CodMovimiento, cancelacion);

            if (anterior is null)
            {
                throw new ExcepcionNegocio(
                    StatusCodes.Status404NotFound,
                    $"El movimiento {movimiento.CodMovimiento} no existe.");
            }

            var saldoAnterior = await BloquearSaldoAsync(
                conexion, transaccion, anterior.NumCuenta, cancelacion);

            saldoAnterior = Math.Round(saldoAnterior - Efecto(anterior), 2);
            if (saldoAnterior < 0)
            {
                throw new ExcepcionNegocio(
                    StatusCodes.Status409Conflict,
                    "No se puede actualizar el movimiento: el saldo de la cuenta quedaria negativo.");
            }

            decimal saldoFinal;

            if (string.Equals(anterior.NumCuenta, movimiento.NumCuenta, StringComparison.Ordinal))
            {
                saldoFinal = Math.Round(saldoAnterior + Efecto(movimiento), 2);
                if (saldoFinal < 0)
                {
                    throw new ExcepcionNegocio(
                        StatusCodes.Status409Conflict,
                        "Saldo insuficiente para realizar el retiro.");
                }

                await EscribirSaldoAsync(
                    conexion, transaccion, anterior.NumCuenta, saldoFinal, cancelacion);
            }
            else
            {
                await EscribirSaldoAsync(
                    conexion, transaccion, anterior.NumCuenta, saldoAnterior, cancelacion);

                var saldoNuevo = await BloquearSaldoAsync(
                    conexion, transaccion, movimiento.NumCuenta, cancelacion);

                saldoFinal = Math.Round(saldoNuevo + Efecto(movimiento), 2);
                if (saldoFinal < 0)
                {
                    throw new ExcepcionNegocio(
                        StatusCodes.Status409Conflict,
                        "Saldo insuficiente para realizar el retiro.");
                }

                await EscribirSaldoAsync(
                    conexion, transaccion, movimiento.NumCuenta, saldoFinal, cancelacion);
            }

            const string actualizacion = """
                UPDATE MOVIMIENTO
                SET NUM_CUENTA = @numCuenta,
                    TIPO = @tipo,
                    VALOR = @valor,
                    FECHA = @fecha
                WHERE COD_MOVIMIENTO = @codMovimiento;
                """;

            await using (var comando = new SqlCommand(actualizacion, conexion, transaccion))
            {
                comando.Parameters.Add("@codMovimiento", SqlDbType.Int).Value = movimiento.CodMovimiento;
                comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = movimiento.NumCuenta;
                comando.Parameters.Add("@tipo", SqlDbType.VarChar, 3).Value = movimiento.Tipo.ToString();
                comando.Parameters.AgregarDecimal("@valor", movimiento.Valor);
                comando.Parameters.Add("@fecha", SqlDbType.DateTime).Value = movimiento.Fecha;

                await comando.ExecuteNonQueryAsync(cancelacion);
            }

            transaccion.Commit();
            return saldoFinal;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Elimina el movimiento revirtiendo su efecto en la cuenta.
    /// Devuelve false si el movimiento no existia.
    /// </summary>
    public async Task<bool> EliminarAsync(
        int codMovimiento,
        CancellationToken cancelacion = default)
    {
        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);
        using var transaccion = conexion.BeginTransaction();

        try
        {
            var anterior = await LeerMovimientoBloqueadoAsync(
                conexion, transaccion, codMovimiento, cancelacion);

            if (anterior is null)
            {
                transaccion.Rollback();
                return false;
            }

            var saldo = await BloquearSaldoAsync(
                conexion, transaccion, anterior.NumCuenta, cancelacion);

            saldo = Math.Round(saldo - Efecto(anterior), 2);
            if (saldo < 0)
            {
                throw new ExcepcionNegocio(
                    StatusCodes.Status409Conflict,
                    "No se puede eliminar el movimiento: el saldo de la cuenta quedaria negativo.");
            }

            const string eliminacion =
                "DELETE FROM MOVIMIENTO WHERE COD_MOVIMIENTO = @codMovimiento;";

            await using (var comando = new SqlCommand(eliminacion, conexion, transaccion))
            {
                comando.Parameters.Add("@codMovimiento", SqlDbType.Int).Value = codMovimiento;
                await comando.ExecuteNonQueryAsync(cancelacion);
            }

            await EscribirSaldoAsync(
                conexion, transaccion, anterior.NumCuenta, saldo, cancelacion);

            transaccion.Commit();
            return true;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    private static decimal Efecto(Movimiento movimiento) =>
        movimiento.Tipo == TipoMov.DEP ? movimiento.Valor : -movimiento.Valor;

    private static async Task<decimal> BloquearSaldoAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        string numCuenta,
        CancellationToken cancelacion)
    {
        const string consulta = """
            SELECT SALDO_CUENTA
            FROM CUENTA WITH (UPDLOCK, ROWLOCK)
            WHERE NUM_CUENTA = @numCuenta;
            """;

        await using var comando = new SqlCommand(consulta, conexion, transaccion);
        comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = numCuenta;

        var resultado = await comando.ExecuteScalarAsync(cancelacion);
        if (resultado is null || resultado == DBNull.Value)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"La cuenta '{numCuenta}' no existe.");
        }

        return Convert.ToDecimal(resultado);
    }

    private static async Task EscribirSaldoAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        string numCuenta,
        decimal saldo,
        CancellationToken cancelacion)
    {
        const string consulta = """
            UPDATE CUENTA
            SET SALDO_CUENTA = @saldo
            WHERE NUM_CUENTA = @numCuenta;
            """;

        await using var comando = new SqlCommand(consulta, conexion, transaccion);
        comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = numCuenta;
        comando.Parameters.AgregarDecimal("@saldo", saldo);

        await comando.ExecuteNonQueryAsync(cancelacion);
    }

    private static async Task<Movimiento?> LeerMovimientoBloqueadoAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        int codMovimiento,
        CancellationToken cancelacion)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM MOVIMIENTO WITH (UPDLOCK, ROWLOCK)
            WHERE COD_MOVIMIENTO = @codMovimiento;
            """;

        await using var comando = new SqlCommand(consulta, conexion, transaccion);
        comando.Parameters.Add("@codMovimiento", SqlDbType.Int).Value = codMovimiento;

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        return await lector.ReadAsync(cancelacion) ? Mapear(lector) : null;
    }

    private static async Task<int> SiguienteCodigoAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        CancellationToken cancelacion)
    {
        const string consulta =
            "SELECT ISNULL(MAX(COD_MOVIMIENTO), 0) + 1 FROM MOVIMIENTO;";

        await using var comando = new SqlCommand(consulta, conexion, transaccion);
        var resultado = await comando.ExecuteScalarAsync(cancelacion);
        return Convert.ToInt32(resultado);
    }

    private static async Task<List<Movimiento>> LeerMovimientosAsync(
        SqlCommand comando,
        CancellationToken cancelacion)
    {
        var movimientos = new List<Movimiento>();

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        while (await lector.ReadAsync(cancelacion))
        {
            movimientos.Add(Mapear(lector));
        }

        return movimientos;
    }

    private static Movimiento Mapear(SqlDataReader lector) => new()
    {
        CodMovimiento = lector.GetInt32(0),
        NumCuenta = lector.GetString(1),
        Tipo = Enum.Parse<TipoMov>(lector.GetString(2).Trim(), ignoreCase: true),
        Valor = lector.GetDecimal(3),
        Fecha = lector.GetDateTime(4)
    };
}
