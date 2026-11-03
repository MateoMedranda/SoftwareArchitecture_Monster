using Microsoft.Data.SqlClient;
using System.Data;
using SISTEMA_FACTURACION.Modelos;

namespace SISTEMA_FACTURACION.Modelos.DAO;

public sealed class ProductoDAO(Conexion conexion)
{
    public async Task<List<Producto>> ListarAsync(CancellationToken ct = default)
    {
        await using var cn = conexion.CrearConexion(); await cn.OpenAsync(ct);
        await using var cmd = new SqlCommand("SELECT NUM_INVENTARIO,NOMBRE,PRECIO FROM ITEM_INVENTARIO ORDER BY NUM_INVENTARIO", cn);
        await using var rd = await cmd.ExecuteReaderAsync(ct); var lista = new List<Producto>();
        while (await rd.ReadAsync(ct)) lista.Add(Mapear(rd)); return lista;
    }
    public async Task<Producto?> PorCodigoAsync(int id, CancellationToken ct = default)
    {
        await using var cn = conexion.CrearConexion(); await cn.OpenAsync(ct);
        await using var cmd = new SqlCommand("SELECT NUM_INVENTARIO,NOMBRE,PRECIO FROM ITEM_INVENTARIO WHERE NUM_INVENTARIO=@id", cn); cmd.Parameters.AddWithValue("@id", id);
        await using var rd = await cmd.ExecuteReaderAsync(ct); return await rd.ReadAsync(ct) ? Mapear(rd) : null;
    }
    public async Task<int> CrearAsync(Producto p, CancellationToken ct = default)
    {
        await using var cn = conexion.CrearConexion(); await cn.OpenAsync(ct); await using var tx = (SqlTransaction)await cn.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try {
            await using var cmd = new SqlCommand("DECLARE @id int; SELECT @id=ISNULL(MAX(NUM_INVENTARIO),0)+1 FROM ITEM_INVENTARIO WITH (TABLOCKX,HOLDLOCK); INSERT INTO ITEM_INVENTARIO(NUM_INVENTARIO,NOMBRE,PRECIO) VALUES(@id,@nombre,@precio); SELECT @id", cn, tx);
            cmd.Parameters.AddWithValue("@nombre", p.Nombre); cmd.Parameters.Add("@precio", SqlDbType.Decimal).Precision=10; cmd.Parameters["@precio"].Scale=2; cmd.Parameters["@precio"].Value=p.Precio;
            var id=Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)); await tx.CommitAsync(ct); return id;
        } catch { await tx.RollbackAsync(ct); throw; }
    }
    public async Task<bool> ActualizarAsync(Producto p, CancellationToken ct = default)
    {
        await using var cn=conexion.CrearConexion(); 
        await cn.OpenAsync(ct); 
        await using var cmd=new SqlCommand("UPDATE ITEM_INVENTARIO SET NOMBRE=@nombre,PRECIO=@precio WHERE NUM_INVENTARIO=@id",cn);
        cmd.Parameters.AddWithValue("@nombre",p.Nombre); cmd.Parameters.AddWithValue("@precio",p.Precio); 
        cmd.Parameters.AddWithValue("@id",p.NumInventario); 
        return await cmd.ExecuteNonQueryAsync(ct)==1;
    }
    public async Task<bool> EliminarAsync(int id, CancellationToken ct = default)
    {
        await using var cn=conexion.CrearConexion(); 
        await cn.OpenAsync(ct); 
        await using var cmd=new SqlCommand("DELETE FROM ITEM_INVENTARIO WHERE NUM_INVENTARIO=@id",cn); 
        cmd.Parameters.AddWithValue("@id",id); 
        return await cmd.ExecuteNonQueryAsync(ct)==1;
    }
    private static Producto Mapear(SqlDataReader r)=>new(){NumInventario=r.GetInt32(0),Nombre=r.GetString(1),Precio=r.GetDecimal(2)};
}
