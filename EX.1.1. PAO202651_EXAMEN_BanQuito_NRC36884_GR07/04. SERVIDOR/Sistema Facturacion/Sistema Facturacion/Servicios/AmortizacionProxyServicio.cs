using SISTEMA_FACTURACION.Comun;
using SISTEMA_FACTURACION.Modelos.DAO;
using SISTEMA_FACTURACION.Modelos.DTO;

namespace SISTEMA_FACTURACION.Servicios;

/// <summary>
/// Proxy de consulta de la tabla de amortizacion por cedula.
/// Valida que el cliente exista en facturacion y en BanQuito, y solo
/// devuelve la tabla si el cliente tiene al menos un credito en BanQuito.
/// </summary>
public sealed class AmortizacionProxyServicio(ClienteDAO clienteDao,BanquitoCliente banquito)
{
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
