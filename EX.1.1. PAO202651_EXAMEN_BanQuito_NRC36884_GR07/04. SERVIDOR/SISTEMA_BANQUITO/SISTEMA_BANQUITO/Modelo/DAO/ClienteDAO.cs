using System.Data;
using Microsoft.Data.SqlClient;
using SISTEMA_BANQUITO.Modelo;

namespace SISTEMA_BANQUITO.Modelo.DAO;

/// <summary>
/// Acceso a datos de la tabla CLIENTE.
/// </summary>
public sealed class ClienteDAO
{
    private const string Columnas =
        "COD_CLIENTE, CEDULA, NOMBRE, GENERO, FECHA_NACIMIENTO";

    private readonly Conexion _conexion;

    public ClienteDAO(Conexion conexion)
    {
        _conexion = conexion;
    }

    public async Task<List<Cliente>> ListarAsync(CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM CLIENTE
            ORDER BY COD_CLIENTE;
            """;

        var clientes = new List<Cliente>();

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        while (await lector.ReadAsync(cancelacion))
        {
            clientes.Add(Mapear(lector));
        }

        return clientes;
    }

    public async Task<Cliente?> ObtenerPorCedulaAsync(
        string cedula,
        CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM CLIENTE
            WHERE CEDULA = @cedula;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@cedula", SqlDbType.VarChar, 100).Value = cedula;

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        return await lector.ReadAsync(cancelacion) ? Mapear(lector) : null;
    }

    public async Task<Cliente?> ObtenerPorCodigoAsync(
        int codCliente,
        CancellationToken cancelacion = default)
    {
        const string consulta = $"""
            SELECT {Columnas}
            FROM CLIENTE
            WHERE COD_CLIENTE = @codCliente;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@codCliente", SqlDbType.Int).Value = codCliente;

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        return await lector.ReadAsync(cancelacion) ? Mapear(lector) : null;
    }

    /// <summary>
    /// Indica si ya existe un cliente con la cedula indicada.
    /// </summary>
    public async Task<bool> ExisteCedulaAsync(
        string cedula,
        CancellationToken cancelacion = default)
    {
        const string consulta = """
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM CLIENTE WHERE CEDULA = @cedula
            ) THEN 1 ELSE 0 END;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@cedula", SqlDbType.VarChar, 100).Value = cedula;

        var resultado = await comando.ExecuteScalarAsync(cancelacion);
        return Convert.ToInt32(resultado) == 1;
    }

    /// <summary>
    /// Calcula el siguiente codigo de cliente (COD_CLIENTE no es IDENTITY).
    /// </summary>
    public async Task<int> SiguienteCodigoAsync(CancellationToken cancelacion = default)
    {
        const string consulta = "SELECT ISNULL(MAX(COD_CLIENTE), 0) + 1 FROM CLIENTE;";

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        var resultado = await comando.ExecuteScalarAsync(cancelacion);
        return Convert.ToInt32(resultado);
    }

    public async Task CrearAsync(Cliente cliente, CancellationToken cancelacion = default)
    {
        const string consulta = """
            INSERT INTO CLIENTE (COD_CLIENTE, CEDULA, NOMBRE, GENERO, FECHA_NACIMIENTO)
            VALUES (@codCliente, @cedula, @nombre, @genero, @fechaNacimiento);
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@codCliente", SqlDbType.Int).Value = cliente.CodCliente;
        comando.Parameters.Add("@cedula", SqlDbType.VarChar, 100).Value = cliente.Cedula;
        comando.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = cliente.Nombre;
        comando.Parameters.Add("@genero", SqlDbType.VarChar, 100).Value = cliente.Genero;
        comando.Parameters.Add("@fechaNacimiento", SqlDbType.DateTime).Value = cliente.FechaNacimiento;

        await comando.ExecuteNonQueryAsync(cancelacion);
    }

    public async Task<int> ActualizarAsync(Cliente cliente, CancellationToken cancelacion = default)
    {
        const string consulta = """
            UPDATE CLIENTE
            SET CEDULA = @cedula,
                NOMBRE = @nombre,
                GENERO = @genero,
                FECHA_NACIMIENTO = @fechaNacimiento
            WHERE COD_CLIENTE = @codCliente;
            """;

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@codCliente", SqlDbType.Int).Value = cliente.CodCliente;
        comando.Parameters.Add("@cedula", SqlDbType.VarChar, 100).Value = cliente.Cedula;
        comando.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = cliente.Nombre;
        comando.Parameters.Add("@genero", SqlDbType.VarChar, 100).Value = cliente.Genero;
        comando.Parameters.Add("@fechaNacimiento", SqlDbType.DateTime).Value = cliente.FechaNacimiento;

        return await comando.ExecuteNonQueryAsync(cancelacion);
    }

    public async Task<int> EliminarAsync(int codCliente, CancellationToken cancelacion = default)
    {
        const string consulta = "DELETE FROM CLIENTE WHERE COD_CLIENTE = @codCliente;";

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@codCliente", SqlDbType.Int).Value = codCliente;

        return await comando.ExecuteNonQueryAsync(cancelacion);
    }

    private static Cliente Mapear(SqlDataReader lector) => new()
    {
        CodCliente = lector.GetInt32(0),
        Cedula = lector.GetString(1),
        Nombre = lector.GetString(2),
        Genero = lector.GetString(3),
        FechaNacimiento = lector.GetDateTime(4)
    };
}
