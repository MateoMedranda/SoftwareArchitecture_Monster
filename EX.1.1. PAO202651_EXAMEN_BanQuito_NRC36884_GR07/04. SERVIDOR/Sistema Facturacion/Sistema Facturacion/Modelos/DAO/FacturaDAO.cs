using Microsoft.Data.SqlClient;
using System.Data;
using SISTEMA_FACTURACION.Modelos;

namespace SISTEMA_FACTURACION.Modelos.DAO;

public sealed class FacturaDAO(Conexion conexion)
{
    public async Task<int> CrearAsync(Factura f, CancellationToken ct = default)
    {
        await using var cn=conexion.CrearConexion(); await cn.OpenAsync(ct); await using var tx=(SqlTransaction)await cn.BeginTransactionAsync(IsolationLevel.Serializable,ct);
        try {
            await using var cmd=new SqlCommand("DECLARE @factura int; SELECT @factura=ISNULL(MAX(NUM_FACTURA),0)+1 FROM FACTURA WITH (TABLOCKX,HOLDLOCK); INSERT INTO FACTURA(NUM_FACTURA,COD_CLIENTE,FECHA,TIPO_PAGO,VALOR,NUM_CREDITO) VALUES(@factura,@cliente,@fecha,@tipo,@valor,@credito); SELECT @factura",cn,tx);
            cmd.Parameters.AddWithValue("@cliente",f.CodCliente); cmd.Parameters.AddWithValue("@fecha",f.Fecha); cmd.Parameters.AddWithValue("@tipo",f.TipoPago); AddDecimal(cmd,"@valor",f.Valor); cmd.Parameters.AddWithValue("@credito",(object?)f.NumCredito??DBNull.Value);
            var id=Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
            foreach(var it in f.Items) {
                await using var det=new SqlCommand("DECLARE @item int; SELECT @item=ISNULL(MAX(NUM_ITEM),0)+1 FROM ITEM_FACTURA WITH (TABLOCKX,HOLDLOCK); INSERT INTO ITEM_FACTURA(NUM_ITEM,NUM_FACTURA,NUM_INVENTARIO,VALOR,CANTIDAD) VALUES(@item,@factura,@producto,@precio,@cantidad)",cn,tx);
                det.Parameters.AddWithValue("@factura",id); det.Parameters.AddWithValue("@producto",it.NumInventario); det.Parameters.AddWithValue("@cantidad",it.Cantidad); AddDecimal(det,"@precio",it.Valor); await det.ExecuteNonQueryAsync(ct);
            }
            await tx.CommitAsync(ct); return id;
        } catch { await tx.RollbackAsync(ct); throw; }
    }

    public async Task<Factura?> PorNumeroAsync(int numero, CancellationToken ct=default) => await ConsultarAsync("WHERE f.NUM_FACTURA=@valor", "@valor", numero, ct);
    public async Task<List<Factura>> PorClienteAsync(int? codigo, string? cedula, CancellationToken ct=default)
    {
        var condicion=codigo.HasValue?"WHERE f.COD_CLIENTE=@valor":"WHERE c.CEDULA=@valor";
        var facturas=await VariasAsync(condicion,"@valor",(object?)codigo??cedula!,ct); return facturas;
    }
    public async Task<List<Factura>> ListarAsync(CancellationToken ct=default)=>await VariasAsync("","",0,ct);

    private async Task<Factura?> ConsultarAsync(string where,string parametro,object valor,CancellationToken ct)
    { var lista=await VariasAsync(where,parametro,valor,ct); return lista.FirstOrDefault(); }
    private async Task<List<Factura>> VariasAsync(string where,string parametro,object valor,CancellationToken ct)
    {
        var sql=$"SELECT f.NUM_FACTURA,f.COD_CLIENTE,f.FECHA,f.TIPO_PAGO,f.VALOR,f.NUM_CREDITO,c.CEDULA,c.NOMBRE,i.NUM_ITEM,i.NUM_INVENTARIO,p.NOMBRE,i.VALOR,i.CANTIDAD FROM FACTURA f JOIN CLIENTE c ON c.COD_CLIENTE=f.COD_CLIENTE LEFT JOIN ITEM_FACTURA i ON i.NUM_FACTURA=f.NUM_FACTURA LEFT JOIN ITEM_INVENTARIO p ON p.NUM_INVENTARIO=i.NUM_INVENTARIO {where} ORDER BY f.NUM_FACTURA DESC,i.NUM_ITEM";
        await using var cn=conexion.CrearConexion(); await cn.OpenAsync(ct); await using var cmd=new SqlCommand(sql,cn); if(where.Length>0) cmd.Parameters.AddWithValue(parametro,valor);
        await using var rd=await cmd.ExecuteReaderAsync(ct); var mapa=new Dictionary<int,Factura>();
        while(await rd.ReadAsync(ct)) {
            var n=rd.GetInt32(0); if(!mapa.TryGetValue(n,out var f)){f=new Factura{NumFactura=n,CodCliente=rd.GetInt32(1),Fecha=rd.GetDateTime(2),TipoPago=rd.GetString(3),Valor=rd.GetDecimal(4),NumCredito=rd.IsDBNull(5)?null:rd.GetInt32(5),Cliente=new Cliente{CodCliente=rd.GetInt32(1),Cedula=rd.GetString(6),Nombre=rd.GetString(7)}};mapa.Add(n,f);}
            if(!rd.IsDBNull(8)) f.Items.Add(new ItemFactura{NumItem=rd.GetInt32(8),NumFactura=n,NumInventario=rd.GetInt32(9),NombreProducto=rd.GetString(10),Valor=rd.GetDecimal(11),Cantidad=rd.GetInt32(12)});
        } return mapa.Values.ToList();
    }
    private static void AddDecimal(SqlCommand c,string name,decimal val){var p=c.Parameters.Add(name,SqlDbType.Decimal);p.Precision=10;p.Scale=2;p.Value=val;}
}
