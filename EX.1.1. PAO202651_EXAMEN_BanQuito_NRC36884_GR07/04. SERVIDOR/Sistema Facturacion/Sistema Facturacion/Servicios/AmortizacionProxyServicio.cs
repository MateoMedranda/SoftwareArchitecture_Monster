using SISTEMA_FACTURACION.Comun;
using SISTEMA_FACTURACION.Modelos.DAO;
using SISTEMA_FACTURACION.Modelos.DTO;

namespace SISTEMA_FACTURACION.Servicios;

/// <summary>
/// Proxy de consulta de la tabla de amortizacion por cedula.
/// Valida que el cliente exista en facturacion y en BanQuito, y solo
/// devuelve la tabla si el cliente tiene al menos un credito en BanQuito.
/// </summary>
public sealed class AmortizacionProxyServicio(ClienteDAO clienteDao,FacturaDAO facturaDao,BanquitoCliente banquito)
{
    public async Task<RespuestaAmortizacionBanquitoDTO> ObtenerPorFacturaAsync(int numFactura,CancellationToken ct)
    {
        if(numFactura<=0) throw new ExcepcionNegocio(400,"El número de factura debe ser mayor que cero.");

        var factura=await facturaDao.PorNumeroAsync(numFactura,ct)
            ?? throw new ExcepcionNegocio(404,$"No existe la factura {numFactura}.");

        var numCredito=factura.NumCredito
            ?? throw new ExcepcionNegocio(409,$"La factura {numFactura} fue pagada en efectivo y no tiene crédito asociado.");

        return await banquito.TablaPorCreditoAsync(numCredito,ct)
            ?? throw new ExcepcionNegocio(404,$"El crédito {numCredito} no existe en BanQuito.");
    }
    public async Task<RespuestaAmortizacionBanquitoDTO> ObtenerPorCedulaAsync(string cedula,CancellationToken ct)
    {
        cedula=(cedula??string.Empty).Trim();
        if(cedula.Length==0) throw new ExcepcionNegocio(400,"La cédula es obligatoria.");

        var clienteLocal=await clienteDao.PorCedulaAsync(cedula,ct)
            ?? throw new ExcepcionNegocio(404,$"No existe un cliente con la cédula '{cedula}' en facturación.");

        var clienteBanco=await banquito.ClientePorCedulaAsync(clienteLocal.Cedula,ct)
            ?? throw new ExcepcionNegocio(404,$"El cliente con cédula '{cedula}' no existe en BanQuito.");

        return await banquito.TablaPorClienteAsync(clienteBanco.CodCliente,ct)
            ?? throw new ExcepcionNegocio(409,$"El cliente con cédula '{cedula}' no tiene créditos registrados en BanQuito.");
    }
}
