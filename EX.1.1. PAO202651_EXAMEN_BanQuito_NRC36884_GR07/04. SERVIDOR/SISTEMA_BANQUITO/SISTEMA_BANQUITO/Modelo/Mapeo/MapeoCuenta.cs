using SISTEMA_BANQUITO.Modelo.DTO;

namespace SISTEMA_BANQUITO.Modelo.Mapeo;

/// <summary>
/// Mapeo entre la entidad Cuenta y sus contratos JSON.
/// </summary>
public static class MapeoCuenta
{
    public static Cuenta AEntidad(SolicitudCuenta solicitud) => new()
    {
        CodCliente = solicitud.CodCliente,
        SaldoCuenta = solicitud.SaldoCuenta
    };

    public static RespuestaCuenta ADto(Cuenta cuenta) => new()
    {
        NumCuenta = cuenta.NumCuenta,
        CodCliente = cuenta.CodCliente,
        SaldoCuenta = cuenta.SaldoCuenta
    };
}
