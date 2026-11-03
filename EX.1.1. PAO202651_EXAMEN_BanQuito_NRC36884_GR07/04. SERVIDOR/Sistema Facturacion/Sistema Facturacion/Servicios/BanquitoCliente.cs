using System.Net;
using System.Net.Http.Json;
using SISTEMA_FACTURACION.Comun;
using SISTEMA_FACTURACION.Modelos.DTO;

namespace SISTEMA_FACTURACION.Servicios;

public sealed class BanquitoCliente(HttpClient http)
{
    public async Task<RespuestaSujetoBanquitoDTO> VerificarAsync(string cedula,CancellationToken ct)
    {
        using var response=await http.PostAsJsonAsync("api/sujeto-credito/verificar",new SolicitudSujetoBanquitoDTO{Cedula=cedula},ct);
        if(!response.IsSuccessStatusCode) throw new ExcepcionNegocio(StatusCodes.Status502BadGateway,"BanQuito no pudo verificar el cliente.");
        return await response.Content.ReadFromJsonAsync<RespuestaSujetoBanquitoDTO>(cancellationToken:ct) ?? throw new HttpRequestException("Respuesta vacía de BanQuito.");
    }
    public async Task<decimal> MontoMaximoAsync(int codigo,CancellationToken ct)
    {
        using var response=await http.PostAsJsonAsync("api/monto-maximo/calcular",new SolicitudMontoBanquitoDTO{CodCliente=codigo},ct);
        if(!response.IsSuccessStatusCode) throw new ExcepcionNegocio(StatusCodes.Status502BadGateway,"BanQuito no pudo calcular el monto máximo.");
        var result=await response.Content.ReadFromJsonAsync<RespuestaMontoBanquitoDTO>(cancellationToken:ct) ?? throw new HttpRequestException("Respuesta vacía de BanQuito."); return result.MontoMaximo;
    }
    public async Task<RespuestaOtorgarBanquitoDTO> OtorgarAsync(int codigo,decimal valor,int cuotas,CancellationToken ct)
    {
        using var response=await http.PostAsJsonAsync("api/credito/otorgar",new SolicitudOtorgarBanquitoDTO{CodCliente=codigo,PrecioTelefono=valor,NumCuotas=cuotas},ct);
        if(!response.IsSuccessStatusCode) throw new ExcepcionNegocio(StatusCodes.Status502BadGateway,"BanQuito no pudo procesar el crédito.");
        return await response.Content.ReadFromJsonAsync<RespuestaOtorgarBanquitoDTO>(cancellationToken:ct) ?? throw new HttpRequestException("Respuesta vacía de BanQuito.");
    }
    public async Task<RespuestaClienteBanquitoDTO?> ClientePorCedulaAsync(string cedula,CancellationToken ct)
    {
        using var response=await http.PostAsJsonAsync("api/clientes/buscar-cedula",new SolicitudTextoBanquitoDTO{Valor=cedula},ct);
        if(response.StatusCode==HttpStatusCode.NotFound) return null;
        if(!response.IsSuccessStatusCode) throw new ExcepcionNegocio(StatusCodes.Status502BadGateway,"BanQuito no pudo consultar el cliente.");
        return await response.Content.ReadFromJsonAsync<RespuestaClienteBanquitoDTO>(cancellationToken:ct) ?? throw new HttpRequestException("Respuesta vacía de BanQuito.");
    }
    public async Task<RespuestaAmortizacionBanquitoDTO?> TablaPorClienteAsync(int codCliente,CancellationToken ct)
    {
        using var response=await http.PostAsJsonAsync("api/amortizaciones/tabla-por-cliente",new SolicitudNumeroBanquitoDTO{Valor=codCliente},ct);
        if(response.StatusCode==HttpStatusCode.NotFound) return null;
        if(!response.IsSuccessStatusCode) throw new ExcepcionNegocio(StatusCodes.Status502BadGateway,"BanQuito no pudo consultar la tabla de amortización.");
        return await response.Content.ReadFromJsonAsync<RespuestaAmortizacionBanquitoDTO>(cancellationToken:ct) ?? throw new HttpRequestException("Respuesta vacía de BanQuito.");
    }
}
