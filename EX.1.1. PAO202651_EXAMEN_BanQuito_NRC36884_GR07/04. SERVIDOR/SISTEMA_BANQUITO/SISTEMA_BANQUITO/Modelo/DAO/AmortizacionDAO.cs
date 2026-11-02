using System.Data;
using Microsoft.Data.SqlClient;
using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo;

namespace SISTEMA_BANQUITO.Modelo.DAO;

/// <summary>
/// Acceso a datos de la tabla AMORTIZACION.
/// </summary>
public sealed class AmortizacionDAO
{
    private const string Columnas = """
        NUM_CUOTA, NUM_CREDITO, VALOR_CUOTA, INTERES_PAGADO,
        CAPITAL_PAGADO, SALDO, FECHA_PAGO, PAGADA
        """;

    private readonly Conexion _conexion;

    public AmortizacionDAO(Conexion conexion)
    {
        _conexion = conexion;
    }

    public async Task<List<Amortizacion>> ObtenerPorCreditoAsync(
        int numCredito,
        CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM AMORTIZACION
            WHERE NUM_CREDITO = @numCredito
            ORDER BY NUM_CUOTA;
            """;

        var cuotas = new List<Amortizacion>();

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@numCredito", SqlDbType.Int).Value = numCredito;

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        while (await lector.ReadAsync(cancelacion))
        {
            cuotas.Add(Mapear(lector));
        }

        return cuotas;
    }

    public async Task<Amortizacion?> ObtenerCuotaAsync(
        int numCredito,
        int numCuota,
        CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM AMORTIZACION
            WHERE NUM_CREDITO = @numCredito
              AND NUM_CUOTA = @numCuota;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@numCredito", SqlDbType.Int).Value = numCredito;
        comando.Parameters.Add("@numCuota", SqlDbType.Int).Value = numCuota;

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        return await lector.ReadAsync(cancelacion) ? Mapear(lector) : null;
    }

    /// <summary>
    /// Inserta todas las cuotas de un credito en una sola transaccion.
    /// </summary>
    public async Task CrearLoteAsync(
        IReadOnlyList<Amortizacion> cuotas,
        CancellationToken cancelacion = default)
    {
        if (cuotas.Count == 0)
        {
            return;
        }

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);
        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string consulta = """
                INSERT INTO AMORTIZACION (NUM_CUOTA, NUM_CREDITO, VALOR_CUOTA, INTERES_PAGADO,
                                          CAPITAL_PAGADO, SALDO, FECHA_PAGO, PAGADA)
                VALUES (@numCuota, @numCredito, @valorCuota, @interesPagado,
                        @capitalPagado, @saldo, @fechaPago, @pagada);
                """;

            foreach (var cuota in cuotas)
            {
                await using var comando = new SqlCommand(consulta, conexion, transaccion);
                comando.Parameters.Add("@numCuota", SqlDbType.Int).Value = cuota.NumCuota;
                comando.Parameters.Add("@numCredito", SqlDbType.Int).Value = cuota.NumCredito;
                comando.Parameters.AgregarDecimal("@valorCuota", cuota.ValorCuota);
                comando.Parameters.AgregarDecimal("@interesPagado", cuota.InteresPagado);
                comando.Parameters.AgregarDecimal("@capitalPagado", cuota.CapitalPagado);
                comando.Parameters.AgregarDecimal("@saldo", cuota.Saldo);
                comando.Parameters.Add("@fechaPago", SqlDbType.DateTime).Value = cuota.FechaPago;
                comando.Parameters.Add("@pagada", SqlDbType.Bit).Value = cuota.Pagada;

                await comando.ExecuteNonQueryAsync(cancelacion);
            }

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Registra el pago de una cuota en una sola transaccion:
    /// valida que no este pagada, descuenta el saldo de la cuenta,
    /// inserta el movimiento RET, marca la cuota como pagada y
    /// actualiza la fecha de actualizacion del credito.
    /// Devuelve el saldo restante de la cuenta.
    /// </summary>
    public async Task<decimal> RegistrarPagoAsync(
        int numCredito,
        int numCuota,
        string numCuenta,
        CancellationToken cancelacion = default)
    {
        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);
        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string consultaCuota = """
                SELECT VALOR_CUOTA, PAGADA
                FROM AMORTIZACION WITH (UPDLOCK, ROWLOCK)
                WHERE NUM_CREDITO = @numCredito
                  AND NUM_CUOTA = @numCuota;
                """;

            decimal valorCuota;
            bool pagada;

            await using (var comando = new SqlCommand(consultaCuota, conexion, transaccion))
            {
                comando.Parameters.Add("@numCredito", SqlDbType.Int).Value = numCredito;
                comando.Parameters.Add("@numCuota", SqlDbType.Int).Value = numCuota;

                await using var lector = await comando.ExecuteReaderAsync(cancelacion);
                if (!await lector.ReadAsync(cancelacion))
                {
                    throw new ExcepcionNegocio(
                        StatusCodes.Status404NotFound,
                        $"La cuota {numCuota} del credito {numCredito} no existe.");
                }

                valorCuota = lector.GetDecimal(0);
                pagada = lector.GetBoolean(1);
            }

            if (pagada)
            {
                throw new ExcepcionNegocio(
                    StatusCodes.Status409Conflict,
                    $"La cuota {numCuota} del credito {numCredito} ya fue pagada.");
            }

            const string consultaSaldo = """
                SELECT SALDO_CUENTA
                FROM CUENTA WITH (UPDLOCK, ROWLOCK)
                WHERE NUM_CUENTA = @numCuenta;
                """;

            decimal saldo;

            await using (var comando = new SqlCommand(consultaSaldo, conexion, transaccion))
            {
                comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = numCuenta;
                var resultado = await comando.ExecuteScalarAsync(cancelacion);

                if (resultado is null || resultado == DBNull.Value)
                {
                    throw new ExcepcionNegocio(
                        StatusCodes.Status404NotFound,
                        $"La cuenta '{numCuenta}' no existe.");
                }

                saldo = Convert.ToDecimal(resultado);
            }

            if (saldo < valorCuota)
            {
                throw new ExcepcionNegocio(
                    StatusCodes.Status409Conflict,
                    $"Saldo insuficiente: la cuenta '{numCuenta}' tiene {saldo:0.00} " +
                    $"y la cuota es de {valorCuota:0.00}.");
            }

            var nuevoSaldo = Math.Round(saldo - valorCuota, 2);

            const string siguienteCodigo =
                "SELECT ISNULL(MAX(COD_MOVIMIENTO), 0) + 1 FROM MOVIMIENTO;";

            int codMovimiento;

            await using (var comando = new SqlCommand(siguienteCodigo, conexion, transaccion))
            {
                codMovimiento = Convert.ToInt32(await comando.ExecuteScalarAsync(cancelacion));
            }

            const string insertarMovimiento = """
                INSERT INTO MOVIMIENTO (COD_MOVIMIENTO, NUM_CUENTA, TIPO, VALOR, FECHA)
                VALUES (@codMovimiento, @numCuenta, 'RET', @valor, @fecha);
                """;

            await using (var comando = new SqlCommand(insertarMovimiento, conexion, transaccion))
            {
                comando.Parameters.Add("@codMovimiento", SqlDbType.Int).Value = codMovimiento;
                comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = numCuenta;
                comando.Parameters.AgregarDecimal("@valor", valorCuota);
                comando.Parameters.Add("@fecha", SqlDbType.DateTime).Value = DateTime.Now;

                await comando.ExecuteNonQueryAsync(cancelacion);
            }

            const string actualizarSaldo = """
                UPDATE CUENTA
                SET SALDO_CUENTA = @saldo
                WHERE NUM_CUENTA = @numCuenta;
                """;

            await using (var comando = new SqlCommand(actualizarSaldo, conexion, transaccion))
            {
                comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = numCuenta;
                comando.Parameters.AgregarDecimal("@saldo", nuevoSaldo);

                await comando.ExecuteNonQueryAsync(cancelacion);
            }

            const string marcarPagada = """
                UPDATE AMORTIZACION
                SET PAGADA = 1
                WHERE NUM_CREDITO = @numCredito
                  AND NUM_CUOTA = @numCuota
                  AND PAGADA = 0;
                """;

            await using (var comando = new SqlCommand(marcarPagada, conexion, transaccion))
            {
                comando.Parameters.Add("@numCredito", SqlDbType.Int).Value = numCredito;
                comando.Parameters.Add("@numCuota", SqlDbType.Int).Value = numCuota;

                var filas = await comando.ExecuteNonQueryAsync(cancelacion);
                if (filas == 0)
                {
                    throw new ExcepcionNegocio(
                        StatusCodes.Status409Conflict,
                        $"La cuota {numCuota} del credito {numCredito} ya fue pagada.");
                }
            }

            const string actualizarCredito = """
                UPDATE CREDITO
                SET FECHA_ACTUALIZACION = @fecha
                WHERE NUM_CREDITO = @numCredito;
                """;

            await using (var comando = new SqlCommand(actualizarCredito, conexion, transaccion))
            {
                comando.Parameters.Add("@numCredito", SqlDbType.Int).Value = numCredito;
                comando.Parameters.Add("@fecha", SqlDbType.DateTime).Value = DateTime.Now;

                await comando.ExecuteNonQueryAsync(cancelacion);
            }

            transaccion.Commit();
            return nuevoSaldo;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    private static Amortizacion Mapear(SqlDataReader lector) => new()
    {
        NumCuota = lector.GetInt32(0),
        NumCredito = lector.GetInt32(1),
        ValorCuota = lector.GetDecimal(2),
        InteresPagado = lector.GetDecimal(3),
        CapitalPagado = lector.GetDecimal(4),
        Saldo = lector.GetDecimal(5),
        FechaPago = lector.GetDateTime(6),
        Pagada = lector.GetBoolean(7)
    };
}
