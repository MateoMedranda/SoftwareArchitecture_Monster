using SISTEMA_BANQUITO.Modelo.DTO;

namespace SISTEMA_BANQUITO.Modelo.Mapeo;

/// <summary>
/// Mapeo entre la entidad Amortizacion y sus contratos JSON.
/// </summary>
public static class MapeoAmortizacion
{
    public static CuotaDTO ADto(Amortizacion cuota) => new()
    {
        NumCuota = cuota.NumCuota,
        ValorCuota = cuota.ValorCuota,
        InteresPagado = cuota.InteresPagado,
        CapitalPagado = cuota.CapitalPagado,
        Saldo = cuota.Saldo,
        FechaPago = cuota.FechaPago,
        Pagada = cuota.Pagada
    };
}
