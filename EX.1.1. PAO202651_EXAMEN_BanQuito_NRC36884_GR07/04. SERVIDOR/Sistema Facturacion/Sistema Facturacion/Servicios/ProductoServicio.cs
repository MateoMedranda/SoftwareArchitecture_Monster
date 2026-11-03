using Microsoft.Data.SqlClient;
using SISTEMA_FACTURACION.Comun;
using SISTEMA_FACTURACION.Modelos;
using SISTEMA_FACTURACION.Modelos.DAO;
using SISTEMA_FACTURACION.Modelos.DTO;
using SISTEMA_FACTURACION.Modelos.Mapeo;

namespace SISTEMA_FACTURACION.Servicios;
public sealed class ProductoServicio(ProductoDAO dao)
{
    public async Task<List<RespuestaProductoDTO>> ListarAsync(CancellationToken ct)=>
        (await dao.ListarAsync(ct)).
        Select(MapeoFacturacion.ADto)
        .ToList();

    public async Task<RespuestaProductoDTO> BuscarAsync(int id,CancellationToken ct)=>
        MapeoFacturacion.ADto(await dao.PorCodigoAsync(id,ct)??
            throw new ExcepcionNegocio(404,$"No existe el producto {id}.")
            );
    public async Task<RespuestaGenericaDTO> CrearAsync(SolicitudProductoDTO s,CancellationToken ct)
    {
        var id=await dao.CrearAsync(new(){Nombre=s.Nombre.Trim(),Precio=s.Precio},ct);return new(){Exitoso=true,Mensaje=$"Producto creado con código {id}."};}
    public async Task<RespuestaGenericaDTO> ActualizarAsync(int id,SolicitudProductoDTO s,CancellationToken ct)
    {
        _=await BuscarAsync(id,ct);
        await dao.ActualizarAsync(new(){NumInventario=id,Nombre=s.Nombre.Trim(),Precio=s.Precio},ct);
        return new(){Exitoso=true,Mensaje="Producto actualizado."};
    }
    public async Task<RespuestaGenericaDTO> EliminarAsync(int id,CancellationToken ct)
    {try{
            if(!await dao.EliminarAsync(id,ct))throw new ExcepcionNegocio(404,"Producto no encontrado.");
            return new(){Exitoso=true,Mensaje="Producto eliminado."};
        }catch(SqlException ex)when(ex.Number==547)
        {
            throw new ExcepcionNegocio(409,"No se puede eliminar el producto porque aparece en facturas.");
        }
    }
}
