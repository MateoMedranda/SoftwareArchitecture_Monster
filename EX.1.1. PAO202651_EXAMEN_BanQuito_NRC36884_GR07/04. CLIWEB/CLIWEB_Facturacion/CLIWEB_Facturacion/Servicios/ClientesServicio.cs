using CLIWEB_Facturacion.Modelos.DAO;
using CLIWEB_Facturacion.Modelos.DTO;

namespace CLIWEB_Facturacion.Servicios;

/// <summary>Reglas del cliente para el mantenimiento de clientes.</summary>
public sealed class ClientesServicio(ClienteDao dao)
{
    public Task<List<ClienteDTO>> ListarAsync(CancellationToken ct = default) =>
        dao.ListarAsync(ct);

    public Task<ClienteDTO> PorCodigoAsync(int codigo, CancellationToken ct = default) =>
        dao.PorCodigoAsync(codigo, ct);

    public Task<ClienteDTO> PorCedulaAsync(string cedula, CancellationToken ct = default)
    {
        cedula = Normalizar(cedula);
        ValidarCedula(cedula);
        return dao.PorCedulaAsync(cedula, ct);
    }

    public Task<RespuestaGenericaDTO> CrearAsync(string cedula, string nombre, CancellationToken ct = default)
    {
        cedula = Normalizar(cedula);
        nombre = NormalizarNombre(nombre);
        ValidarCedula(cedula);
        ValidarNombre(nombre);

        return dao.CrearAsync(new SolicitudClienteDTO { Cedula = cedula, Nombre = nombre }, ct);
    }

    public Task<RespuestaGenericaDTO> ActualizarAsync(int codigo, string cedula, string nombre, CancellationToken ct = default)
    {
        cedula = Normalizar(cedula);
        nombre = NormalizarNombre(nombre);
        ValidarCedula(cedula);
        ValidarNombre(nombre);

        return dao.ActualizarAsync(codigo, new SolicitudClienteDTO { Cedula = cedula, Nombre = nombre }, ct);
    }

    public Task<RespuestaGenericaDTO> EliminarAsync(int codigo, CancellationToken ct = default) =>
        dao.EliminarAsync(codigo, ct);

    public static string Normalizar(string? valor) => (valor ?? string.Empty).Trim();

    private static string NormalizarNombre(string? nombre) => (nombre ?? string.Empty).Trim();

    private static void ValidarCedula(string cedula)
    {
        if (cedula.Length != 10)
        {
            throw new ApiException(400, "La cédula debe tener exactamente 10 caracteres.");
        }

        if (!cedula.All(char.IsAsciiDigit))
        {
            throw new ApiException(400, "La cédula solo debe contener números.");
        }
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ApiException(400, "El nombre es obligatorio.");
        }

        if (nombre.Length > 100)
        {
            throw new ApiException(400, "El nombre no debe superar los 100 caracteres.");
        }
    }
}
