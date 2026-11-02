using System.Data;
using Microsoft.Data.SqlClient;
using SISTEMA_BANQUITO.Modelo;

namespace SISTEMA_BANQUITO.Modelo.DAO;

/// <summary>
/// Acceso a datos de la tabla CREDITO.
/// </summary>
public sealed class CreditoDAO
{
    private const string Columnas =
        "NUM_CREDITO, NUM_CUENTA, CREDITO_TOTAL, FECHA_CREACION, FECHA_ACTUALIZACION";

    private readonly Conexion _conexion;

    public CreditoDAO(Conexion conexion)
    {
        _conexion = conexion;
    }

    public async Task<List<Credito>> ListarAsync(CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM CREDITO
            ORDER BY NUM_CREDITO;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        return await LeerCreditosAsync(comando, cancelacion);
    }

    public async Task<List<Credito>> ObtenerPorCuentaAsync(
        string numCuenta,
        CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM CREDITO
            WHERE NUM_CUENTA = @numCuenta
            ORDER BY FECHA_CREACION DESC, NUM_CREDITO DESC;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = numCuenta;

        return await LeerCreditosAsync(comando, cancelacion);
    }

    public async Task<List<Credito>> ObtenerPorClienteAsync(
        int codCliente,
        CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT C.NUM_CREDITO, C.NUM_CUENTA, C.CREDITO_TOTAL,
                   C.FECHA_CREACION, C.FECHA_ACTUALIZACION
            FROM CREDITO C
            INNER JOIN CUENTA CT ON CT.NUM_CUENTA = C.NUM_CUENTA
            WHERE CT.COD_CLIENTE = @codCliente
            ORDER BY C.FECHA_CREACION DESC, C.NUM_CREDITO DESC;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@codCliente", SqlDbType.Int).Value = codCliente;

        return await LeerCreditosAsync(comando, cancelacion);
    }

    public async Task<Credito?> ObtenerPorNumeroAsync(
        int numCredito,
        CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM CREDITO
            WHERE NUM_CREDITO = @numCredito;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@numCredito", SqlDbType.Int).Value = numCredito;

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        return await lector.ReadAsync(cancelacion) ? Mapear(lector) : null;
    }

    /// <summary>
    /// Indica si la cuenta ya tiene al menos un credito registrado.
    /// Se usa para la regla "no debe tener credito activo".
    /// </summary>
    public async Task<bool> ExistePorCuentaAsync(
        string numCuenta,
        CancellationToken cancelacion = default)
    {
        const string consulta = """
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM CREDITO WHERE NUM_CUENTA = @numCuenta
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
    /// Siguiente numero de credito. NUM_CREDITO no es IDENTITY y
    /// comienza en 1001 cuando la tabla esta vacia.
    /// </summary>
    public async Task<int> SiguienteNumeroAsync(CancellationToken cancelacion = default)
    {
        const string consulta = """
            SELECT ISNULL(MAX(NUM_CREDITO), 1000) + 1
            FROM CREDITO;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        var resultado = await comando.ExecuteScalarAsync(cancelacion);
        return Convert.ToInt32(resultado);
    }

    public async Task CrearAsync(Credito credito, CancellationToken cancelacion = default)
    {
        const string consulta = """
            INSERT INTO CREDITO (NUM_CREDITO, NUM_CUENTA, CREDITO_TOTAL,
                                 FECHA_CREACION, FECHA_ACTUALIZACION)
            VALUES (@numCredito, @numCuenta, @creditoTotal, @fechaCreacion, @fechaActualizacion);
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@numCredito", SqlDbType.Int).Value = credito.NumCredito;
        comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = credito.NumCuenta;
        comando.Parameters.AgregarDecimal("@creditoTotal", credito.CreditoTotal);
        comando.Parameters.Add("@fechaCreacion", SqlDbType.DateTime).Value = credito.FechaCreacion;
        comando.Parameters.Add("@fechaActualizacion", SqlDbType.DateTime).Value = credito.FechaActualizacion;

        await comando.ExecuteNonQueryAsync(cancelacion);
    }

    public async Task<int> ActualizarAsync(Credito credito, CancellationToken cancelacion = default)
    {
        const string consulta = """
            UPDATE CREDITO
            SET CREDITO_TOTAL = @creditoTotal,
                FECHA_ACTUALIZACION = @fechaActualizacion
            WHERE NUM_CREDITO = @numCredito;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@numCredito", SqlDbType.Int).Value = credito.NumCredito;
        comando.Parameters.AgregarDecimal("@creditoTotal", credito.CreditoTotal);
        comando.Parameters.Add("@fechaActualizacion", SqlDbType.DateTime).Value = credito.FechaActualizacion;

        return await comando.ExecuteNonQueryAsync(cancelacion);
    }

    /// <summary>
    /// Elimina el credito y todas sus cuotas de amortizacion en una sola transaccion.
    /// Devuelve false si el credito no existia.
    /// </summary>
    public async Task<bool> EliminarAsync(
        int numCredito,
        CancellationToken cancelacion = default)
    {
        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);
        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string existencia =
                "SELECT COUNT(1) FROM CREDITO WHERE NUM_CREDITO = @numCredito;";

            await using (var comando = new SqlCommand(existencia, conexion, transaccion))
            {
                comando.Parameters.Add("@numCredito", SqlDbType.Int).Value = numCredito;
                var cantidad = Convert.ToInt32(await comando.ExecuteScalarAsync(cancelacion));
                if (cantidad == 0)
                {
                    transaccion.Rollback();
                    return false;
                }
            }

            const string eliminarCuotas =
                "DELETE FROM AMORTIZACION WHERE NUM_CREDITO = @numCredito;";

            await using (var comando = new SqlCommand(eliminarCuotas, conexion, transaccion))
            {
                comando.Parameters.Add("@numCredito", SqlDbType.Int).Value = numCredito;
                await comando.ExecuteNonQueryAsync(cancelacion);
            }

            const string eliminarCredito =
                "DELETE FROM CREDITO WHERE NUM_CREDITO = @numCredito;";

            await using (var comando = new SqlCommand(eliminarCredito, conexion, transaccion))
            {
                comando.Parameters.Add("@numCredito", SqlDbType.Int).Value = numCredito;
                await comando.ExecuteNonQueryAsync(cancelacion);
            }

            transaccion.Commit();
            return true;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    private static async Task<List<Credito>> LeerCreditosAsync(
        SqlCommand comando,
        CancellationToken cancelacion)
    {
        var creditos = new List<Credito>();

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        while (await lector.ReadAsync(cancelacion))
        {
            creditos.Add(Mapear(lector));
        }

        return creditos;
    }

    private static Credito Mapear(SqlDataReader lector) => new()
    {
        NumCredito = lector.GetInt32(0),
        NumCuenta = lector.GetString(1),
        CreditoTotal = lector.GetDecimal(2),
        FechaCreacion = lector.GetDateTime(3),
        FechaActualizacion = lector.GetDateTime(4)
    };
}
