using SISTEMA_FACTURACION.Modelos;
using SISTEMA_FACTURACION.Modelos.DTO;

namespace SISTEMA_FACTURACION.Modelos.Mapeo;
public static class MapeoFacturacion
{
    public static RespuestaClienteDTO ADto(Cliente c)=>new(){CodCliente=c.CodCliente,Cedula=c.Cedula,Nombre=c.Nombre};
    public static RespuestaProductoDTO ADto(Producto p)=>new(){NumInventario=p.NumInventario,Nombre=p.Nombre,Precio=p.Precio};
    public static RespuestaFacturaDTO ADto(Factura f)=>new(){NumFactura=f.NumFactura,CodCliente=f.CodCliente,Cedula=f.Cliente?.Cedula??"",NombreCliente=f.Cliente?.Nombre??"",Fecha=f.Fecha,TipoPago=f.TipoPago,Valor=f.Valor,NumCredito=f.NumCredito,Items=f.Items.Select(i=>new RespuestaItemFacturaDTO{NumItem=i.NumItem,NumInventario=i.NumInventario,NombreProducto=i.NombreProducto,Valor=i.Valor,Cantidad=i.Cantidad}).ToList()};
}
