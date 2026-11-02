using SISTEMA_BANQUITO.Modelo.DTO;

namespace SISTEMA_BANQUITO.Modelo.Mapeo;

/// <summary>
/// Mapeo entre la entidad Credito y sus contratos JSON.
/// </summary>
public static class MapeoCredito
{
    public static RespuestaCredito ADto(Credito credito) => new()
    {
        NumCredito = credito.NumCredito,
        NumCuenta = credito.NumCuenta,
        CreditoTotal = credito.CreditoTotal,
        FechaCreacion = credito.FechaCreacion,
        FechaActualizacion = credito.FechaActualizacion
    };
}
