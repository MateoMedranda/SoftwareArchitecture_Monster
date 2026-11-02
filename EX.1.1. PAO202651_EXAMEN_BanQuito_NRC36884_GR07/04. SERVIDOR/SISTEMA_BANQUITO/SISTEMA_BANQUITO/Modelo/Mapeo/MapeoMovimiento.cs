using SISTEMA_BANQUITO.Modelo.DTO;

namespace SISTEMA_BANQUITO.Modelo.Mapeo;

/// <summary>
/// Mapeo entre la entidad Movimiento y sus contratos JSON.
/// </summary>
public static class MapeoMovimiento
{
    public static Movimiento AEntidad(SolicitudMovimiento solicitud) => new()
    {
        NumCuenta = solicitud.NumCuenta.Trim(),
        Tipo = solicitud.Tipo,
        Valor = solicitud.Valor,
        Fecha = solicitud.Fecha ?? DateTime.Now
    };

    public static RespuestaMovimiento ADto(Movimiento movimiento, decimal? saldoCuenta = null) => new()
    {
        CodMovimiento = movimiento.CodMovimiento,
        NumCuenta = movimiento.NumCuenta,
        Tipo = movimiento.Tipo.ToString(),
        Valor = movimiento.Valor,
        Fecha = movimiento.Fecha,
        SaldoCuenta = saldoCuenta
    };
}
