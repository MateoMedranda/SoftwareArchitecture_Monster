using Microsoft.Data.SqlClient;
using SISTEMA_FACTURACION.Comun;
using SISTEMA_FACTURACION.Modelos;
using SISTEMA_FACTURACION.Modelos.DAO;
using SISTEMA_FACTURACION.Modelos.DTO;
using SISTEMA_FACTURACION.Modelos.Mapeo;

namespace SISTEMA_FACTURACION.Servicios;
public sealed class ClienteServicio(ClienteDAO dao)
{
    public async Task<List<RespuestaClienteDTO>> ListarAsync(CancellationToken ct)=> (await dao.ListarAsync(ct)).Select(MapeoFacturacion.ADto).ToList();
    public async Task<RespuestaClienteDTO> BuscarAsync(int id,CancellationToken ct)=>MapeoFacturacion.ADto(await dao.PorCodigoAsync(id,ct)??throw new ExcepcionNegocio(404,$"No existe el cliente {id}."));
    public async Task<RespuestaClienteDTO> BuscarCedulaAsync(string cedula,CancellationToken ct)=>MapeoFacturacion.ADto(await dao.PorCedulaAsync(cedula,ct)??throw new ExcepcionNegocio(404,"No existe el cliente con esa cédula."));
    public async Task<RespuestaGenericaDTO> CrearAsync(SolicitudClienteDTO s,CancellationToken ct)
    {
        if(await dao.PorCedulaAsync(s.Cedula,ct) is not null) throw new ExcepcionNegocio(409,"Ya existe un cliente con esa cédula.");
        try { var id=await dao.CrearAsync(new Cliente{Cedula=s.Cedula.Trim(),Nombre=s.Nombre.Trim()},ct); return new(){Exitoso=true,Mensaje=$"Cliente creado con código {id}."}; }
        catch(SqlException ex) when(ex.Number is 2601 or 2627){throw new ExcepcionNegocio(409,"Ya existe un cliente con esa cédula.");}
    }
    public async Task<RespuestaGenericaDTO> ActualizarAsync(int id,SolicitudClienteDTO s,CancellationToken ct)
    {
        _=await BuscarAsync(id,ct); var dup=await dao.PorCedulaAsync(s.Cedula,ct); if(dup is not null&&dup.CodCliente!=id) throw new ExcepcionNegocio(409,"La cédula ya pertenece a otro cliente.");
        try { if(!await dao.ActualizarAsync(new(){CodCliente=id,Cedula=s.Cedula.Trim(),Nombre=s.Nombre.Trim()},ct)) throw new ExcepcionNegocio(404,"Cliente no encontrado."); return new(){Exitoso=true,Mensaje="Cliente actualizado."}; }
        catch(SqlException ex) when(ex.Number is 2601 or 2627){throw new ExcepcionNegocio(409,"La cédula ya está registrada.");}
    }
    public async Task<RespuestaGenericaDTO> EliminarAsync(int id,CancellationToken ct)
    {
        try { if(!await dao.EliminarAsync(id,ct)) throw new ExcepcionNegocio(404,"Cliente no encontrado."); return new(){Exitoso=true,Mensaje="Cliente eliminado."}; }
        catch(SqlException ex) when(ex.Number==547){throw new ExcepcionNegocio(409,"No se puede eliminar el cliente porque tiene facturas.");}
    }
}
