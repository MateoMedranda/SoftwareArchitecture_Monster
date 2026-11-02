using SISTEMA_FACTURACION.Comun;
using SISTEMA_FACTURACION.Modelos;
using SISTEMA_FACTURACION.Modelos.DAO;
using SISTEMA_FACTURACION.Modelos.DTO;
using SISTEMA_FACTURACION.Modelos.Mapeo;

namespace SISTEMA_FACTURACION.Servicios;
public sealed class FacturaServicio(FacturaDAO facturaDao,ClienteDAO clienteDao,ProductoDAO productoDao,BanquitoCliente banquito)
{
    public async Task<RespuestaFacturaDTO> CrearAsync(SolicitudFacturaDTO s,CancellationToken ct)
    {
        var cliente=await clienteDao.PorCodigoAsync(s.CodCliente,ct)??throw new ExcepcionNegocio(404,"No existe el cliente indicado.");
        var pago=s.TipoPago.Trim().ToUpperInvariant();
        if(pago is "CREDITO_DIRECTO") pago="CREDITO";
        if(pago is not ("EFECTIVO" or "CREDITO")) throw new ExcepcionNegocio(400,"TipoPago debe ser EFECTIVO o CREDITO.");
        if(pago=="CREDITO"&&(s.NumCuotas is null or <3 or >18)) throw new ExcepcionNegocio(400,"El plazo del crédito debe estar entre 3 y 18 meses.");
        if(pago=="EFECTIVO"&&s.NumCuotas is not null) throw new ExcepcionNegocio(400,"No se debe indicar plazo para una compra en efectivo.");

        var lineas=new List<ItemFactura>();
        foreach(var entrada in s.Items)
        {
            var producto=await productoDao.PorCodigoAsync(entrada.NumInventario,ct)??throw new ExcepcionNegocio(404,$"No existe el producto {entrada.NumInventario}.");
            lineas.Add(new(){NumInventario=producto.NumInventario,NombreProducto=producto.Nombre,Valor=producto.Precio,Cantidad=entrada.Cantidad});
        }
        var subtotal=lineas.Sum(i=>i.Valor*i.Cantidad);
        var total=pago=="EFECTIVO"?Math.Round(subtotal*0.58m,2,MidpointRounding.AwayFromZero):subtotal;
        if(total<=0) throw new ExcepcionNegocio(400,"El total de la factura debe ser mayor que cero.");

        int? creditoBanquito=null;
        if(pago=="CREDITO")
        {
            var elegibilidad=await banquito.VerificarAsync(cliente.Cedula,ct);
            if(!elegibilidad.SujetoCredito||elegibilidad.CodCliente is null) throw new ExcepcionNegocio(409,$"Cliente no elegible para crédito: {elegibilidad.Motivo??"RECHAZADO"}.");
            var maximo=await banquito.MontoMaximoAsync(elegibilidad.CodCliente.Value,ct);
            if(total>maximo) throw new ExcepcionNegocio(409,$"El total {total:C} supera el crédito máximo {maximo:C}.");
            var otorgamiento=await banquito.OtorgarAsync(elegibilidad.CodCliente.Value,total,s.NumCuotas!.Value,ct);
            if(!otorgamiento.Aprobado) throw new ExcepcionNegocio(409,$"BanQuito rechazó el crédito: {otorgamiento.Motivo??"RECHAZADO"}.");
            creditoBanquito=otorgamiento.IdCredito;
        }

        var factura=new Factura{CodCliente=cliente.CodCliente,Fecha=DateTime.Now,TipoPago=pago,Valor=total,Cliente=cliente,Items=lineas};
        int id;
        try { id=await facturaDao.CrearAsync(factura,ct); }
        catch(Exception) when(creditoBanquito.HasValue)
        {
            throw new ExcepcionNegocio(500,$"BanQuito otorgó el crédito {creditoBanquito}, pero no se pudo guardar la factura local. Requiere conciliación manual.");
        }
        return await BuscarAsync(id,ct);
    }
    public async Task<RespuestaFacturaDTO> BuscarAsync(int id,CancellationToken ct)=>MapeoFacturacion.ADto(await facturaDao.PorNumeroAsync(id,ct)??throw new ExcepcionNegocio(404,$"No existe la factura {id}."));
    public async Task<List<RespuestaFacturaDTO>> BuscarClienteAsync(int? codigo,string? cedula,CancellationToken ct)
    {
        if((codigo is null)==string.IsNullOrWhiteSpace(cedula)) throw new ExcepcionNegocio(400,"Indique exactamente uno de codigoCliente o cedula.");
        return (await facturaDao.PorClienteAsync(codigo,cedula,ct)).Select(MapeoFacturacion.ADto).ToList();
    }
    public async Task<List<RespuestaFacturaDTO>> ListarAsync(CancellationToken ct)=>(await facturaDao.ListarAsync(ct)).Select(MapeoFacturacion.ADto).ToList();
}
