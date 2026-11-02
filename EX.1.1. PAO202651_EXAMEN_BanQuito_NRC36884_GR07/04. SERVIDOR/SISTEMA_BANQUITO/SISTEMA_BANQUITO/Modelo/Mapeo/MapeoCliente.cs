using SISTEMA_BANQUITO.Modelo.DTO;

namespace SISTEMA_BANQUITO.Modelo.Mapeo;

/// <summary>
/// Mapeo entre la entidad Cliente y sus contratos JSON.
/// </summary>
public static class MapeoCliente
{
    public static Cliente AEntidad(SolicitudCliente solicitud) => new()
    {
        Cedula = solicitud.Cedula.Trim(),
        Nombre = solicitud.Nombre.Trim(),
        Genero = solicitud.Genero.Trim().ToUpperInvariant(),
        FechaNacimiento = solicitud.FechaNacimiento.Date
    };

    public static RespuestaCliente ADto(Cliente cliente) => new()
    {
        CodCliente = cliente.CodCliente,
        Cedula = cliente.Cedula,
        Nombre = cliente.Nombre,
        Genero = cliente.Genero,
        FechaNacimiento = cliente.FechaNacimiento,
        Edad = CalcularEdad(cliente.FechaNacimiento)
    };

    public static int CalcularEdad(DateTime fechaNacimiento)
    {
        var hoy = DateTime.Today;
        var edad = hoy.Year - fechaNacimiento.Year;

        if (fechaNacimiento.Date > hoy.AddYears(-edad))
        {
            edad--;
        }

        return edad < 0 ? 0 : edad;
    }
}
