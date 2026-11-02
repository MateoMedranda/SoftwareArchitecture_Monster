using System.Data;
using Microsoft.Data.SqlClient;
using SISTEMA_BANQUITO.Modelo;

namespace SISTEMA_BANQUITO.Modelo.DAO;

/// <summary>
/// Acceso a datos de la tabla CUENTA.
/// </summary>
public sealed class CuentaDAO
{
    private const string Columnas = "NUM_CUENTA, COD_CLIENTE, SALDO_CUENTA";

    private readonly Conexion _conexion;

    public CuentaDAO(Conexion conexion)
    {
        _conexion = conexion;
    }

    public async Task<List<Cuenta>> ListarAsync(CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM CUENTA
            ORDER BY NUM_CUENTA;
            """;

        var cuentas = new List<Cuenta>();

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        while (await lector.ReadAsync(cancelacion))
        {
            cuentas.Add(Mapear(lector));
        }

        return cuentas;
    }

    public async Task<List<Cuenta>> ObtenerPorClienteAsync(
        int codCliente,
        CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM CUENTA
            WHERE COD_CLIENTE = @codCliente
            ORDER BY NUM_CUENTA;
            """;

        var cuentas = new List<Cuenta>();

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@codCliente", SqlDbType.Int).Value = codCliente;

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        while (await lector.ReadAsync(cancelacion))
        {
            cuentas.Add(Mapear(lector));
        }

        return cuentas;
    }

    public async Task<Cuenta?> ObtenerPorNumeroAsync(
        string numCuenta,
        CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM CUENTA
            WHERE NUM_CUENTA = @numCuenta;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = numCuenta;

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        return await lector.ReadAsync(cancelacion) ? Mapear(lector) : null;
    }

    /// <summary>
    /// Calcula el siguiente numero de cuenta. NUM_CUENTA no es IDENTITY,
    /// por eso se usa MAX + 1 y se formatea a 8 digitos.
    /// </summary>
    public async Task<string> SiguienteNumeroAsync(CancellationToken cancelacion = default)
    {
        const string consulta = """
            SELECT ISNULL(MAX(CAST(NUM_CUENTA AS int)), 10000000) + 1
            FROM CUENTA;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        var resultado = await comando.ExecuteScalarAsync(cancelacion);
        var numero = Convert.ToInt32(resultado);
        return numero.ToString("D8");
    }

    public async Task CrearAsync(Cuenta cuenta, CancellationToken cancelacion = default)
    {
        const string consulta = """
            INSERT INTO CUENTA (NUM_CUENTA, COD_CLIENTE, SALDO_CUENTA)
            VALUES (@numCuenta, @codCliente, @saldoCuenta);
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = cuenta.NumCuenta;
        comando.Parameters.Add("@codCliente", SqlDbType.Int).Value = cuenta.CodCliente;
        comando.Parameters.AgregarDecimal("@saldoCuenta", cuenta.SaldoCuenta);

        await comando.ExecuteNonQueryAsync(cancelacion);
    }

    public async Task<int> ActualizarAsync(Cuenta cuenta, CancellationToken cancelacion = default)
    {
        const string consulta = """
            UPDATE CUENTA
            SET COD_CLIENTE = @codCliente,
                SALDO_CUENTA = @saldoCuenta
            WHERE NUM_CUENTA = @numCuenta;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = cuenta.NumCuenta;
        comando.Parameters.Add("@codCliente", SqlDbType.Int).Value = cuenta.CodCliente;
        comando.Parameters.AgregarDecimal("@saldoCuenta", cuenta.SaldoCuenta);

        return await comando.ExecuteNonQueryAsync(cancelacion);
    }

    public async Task<int> EliminarAsync(string numCuenta, CancellationToken cancelacion = default)
    {
        const string consulta = "DELETE FROM CUENTA WHERE NUM_CUENTA = @numCuenta;";

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@numCuenta", SqlDbType.VarChar, 8).Value = numCuenta;

        return await comando.ExecuteNonQueryAsync(cancelacion);
    }

    private static Cuenta Mapear(SqlDataReader lector) => new()
    {
        NumCuenta = lector.GetString(0),
        CodCliente = lector.GetInt32(1),
        SaldoCuenta = lector.GetDecimal(2)
    };
}
