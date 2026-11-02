using Microsoft.Data.SqlClient;
using System.Data;
using SISTEMA_FACTURACION.Modelos;

namespace SISTEMA_FACTURACION.Modelos.DAO;

public sealed class ClienteDAO(Conexion conexion)
{
    public async Task<List<Cliente>> ListarAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT COD_CLIENTE,CEDULA,NOMBRE FROM CLIENTE ORDER BY COD_CLIENTE";
        await using var cn = conexion.CrearConexion(); await cn.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, cn); await using var rd = await cmd.ExecuteReaderAsync(ct);
        var lista = new List<Cliente>();
        while (await rd.ReadAsync(ct)) lista.Add(Mapear(rd));
        return lista;
    }

    public async Task<Cliente?> PorCodigoAsync(int codigo, CancellationToken ct = default) =>
        await UnoAsync("SELECT COD_CLIENTE,CEDULA,NOMBRE FROM CLIENTE WHERE COD_CLIENTE=@id", "@id", codigo, ct);

    public async Task<Cliente?> PorCedulaAsync(string cedula, CancellationToken ct = default) =>
        await UnoAsync("SELECT COD_CLIENTE,CEDULA,NOMBRE FROM CLIENTE WHERE CEDULA=@id", "@id", cedula, ct);

    private async Task<Cliente?> UnoAsync(string sql, string parametro, object valor, CancellationToken ct)
    {
        await using var cn = conexion.CrearConexion(); await cn.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, cn); cmd.Parameters.AddWithValue(parametro, valor);
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        return await rd.ReadAsync(ct) ? Mapear(rd) : null;
    }

    public async Task<int> CrearAsync(Cliente cliente, CancellationToken ct = default)
    {
        await using var cn = conexion.CrearConexion(); await cn.OpenAsync(ct);
        await using var tx = (SqlTransaction)await cn.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            await using var cmd = new SqlCommand("INSERT INTO CLIENTE(COD_CLIENTE,CEDULA,NOMBRE) SELECT ISNULL(MAX(COD_CLIENTE),0)+1,@cedula,@nombre FROM CLIENTE; SELECT CAST(SCOPE_IDENTITY() AS int)", cn, tx);
            // El esquema usa códigos manuales; la expresión MAX bloqueada calcula el siguiente identificador.
            cmd.CommandText = "DECLARE @id int; SELECT @id=ISNULL(MAX(COD_CLIENTE),0)+1 FROM CLIENTE WITH (TABLOCKX,HOLDLOCK); INSERT INTO CLIENTE(COD_CLIENTE,CEDULA,NOMBRE) VALUES(@id,@cedula,@nombre); SELECT @id";
            cmd.Parameters.Add("@cedula", SqlDbType.VarChar, 100).Value = cliente.Cedula;
            cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = cliente.Nombre;
            var id = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)); await tx.CommitAsync(ct); return id;
        }
        catch { await tx.RollbackAsync(ct); throw; }
    }

    public async Task<bool> ActualizarAsync(Cliente c, CancellationToken ct = default)
    {
        const string sql = "UPDATE CLIENTE SET CEDULA=@cedula,NOMBRE=@nombre WHERE COD_CLIENTE=@id";
        await using var cn = conexion.CrearConexion(); await cn.OpenAsync(ct); await using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@cedula", c.Cedula); cmd.Parameters.AddWithValue("@nombre", c.Nombre); cmd.Parameters.AddWithValue("@id", c.CodCliente);
        return await cmd.ExecuteNonQueryAsync(ct) == 1;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken ct = default)
    {
        await using var cn = conexion.CrearConexion(); await cn.OpenAsync(ct); await using var cmd = new SqlCommand("DELETE FROM CLIENTE WHERE COD_CLIENTE=@id", cn);
        cmd.Parameters.AddWithValue("@id", id); return await cmd.ExecuteNonQueryAsync(ct) == 1;
    }

    private static Cliente Mapear(SqlDataReader r) => new() { CodCliente = r.GetInt32(0), Cedula = r.GetString(1), Nombre = r.GetString(2) };
}
