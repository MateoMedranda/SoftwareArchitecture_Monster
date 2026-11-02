using SISTEMA_BANQUITO.Comun;
using SISTEMA_BANQUITO.Modelo;
using SISTEMA_BANQUITO.Modelo.DAO;
using SISTEMA_BANQUITO.Modelo.DTO;
using SISTEMA_BANQUITO.Modelo.Mapeo;

namespace SISTEMA_BANQUITO.Servicio;

/// <summary>
/// Reglas de negocio relacionadas con el cliente y la verificacion
/// de si es sujeto de credito.
/// </summary>
public sealed class ClienteServicio
{
    private readonly ClienteDAO _clienteDAO;
    private readonly CuentaDAO _cuentaDAO;
    private readonly MovimientoDAO _movimientoDAO;
    private readonly CreditoDAO _creditoDAO;

    public ClienteServicio(
        ClienteDAO clienteDAO,
        CuentaDAO cuentaDAO,
        MovimientoDAO movimientoDAO,
        CreditoDAO creditoDAO)
    {
        _clienteDAO = clienteDAO;
        _cuentaDAO = cuentaDAO;
        _movimientoDAO = movimientoDAO;
        _creditoDAO = creditoDAO;
    }

    public async Task<List<RespuestaCliente>> ListarAsync(
        CancellationToken cancelacion = default)
    {
        var clientes = await _clienteDAO.ListarAsync(cancelacion);
        return clientes.Select(MapeoCliente.ADto).ToList();
    }

    public async Task<RespuestaCliente?> ObtenerPorCodigoAsync(
        int codCliente,
        CancellationToken cancelacion = default)
    {
        var cliente = await _clienteDAO.ObtenerPorCodigoAsync(codCliente, cancelacion);
        return cliente is null ? null : MapeoCliente.ADto(cliente);
    }

    public async Task<RespuestaCliente?> ObtenerPorCedulaAsync(
        string cedula,
        CancellationToken cancelacion = default)
    {
        var cliente = await _clienteDAO.ObtenerPorCedulaAsync(cedula.Trim(), cancelacion);
        return cliente is null ? null : MapeoCliente.ADto(cliente);
    }

    public async Task<RespuestaGenerica> CrearAsync(
        SolicitudCliente solicitud,
        CancellationToken cancelacion = default)
    {
        var cliente = MapeoCliente.AEntidad(solicitud);

        if (await _clienteDAO.ExisteCedulaAsync(cliente.Cedula, cancelacion))
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status409Conflict,
                $"Ya existe un cliente con la cedula '{cliente.Cedula}'.");
        }

        cliente.CodCliente = await _clienteDAO.SiguienteCodigoAsync(cancelacion);
        await _clienteDAO.CrearAsync(cliente, cancelacion);

        return new RespuestaGenerica
        {
            Exitoso = true,
            Mensaje = $"Cliente creado correctamente con el codigo {cliente.CodCliente}.",
            ProcesoHttp = "CREAR"
        };
    }

    public async Task<RespuestaGenerica> ActualizarAsync(
        int codCliente,
        SolicitudCliente solicitud,
        CancellationToken cancelacion = default)
    {
        var existente = await _clienteDAO.ObtenerPorCodigoAsync(codCliente, cancelacion);
        if (existente is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El cliente {codCliente} no existe.");
        }

        var cliente = MapeoCliente.AEntidad(solicitud);
        cliente.CodCliente = codCliente;

        if (!string.Equals(existente.Cedula, cliente.Cedula, StringComparison.Ordinal)
            && await _clienteDAO.ExisteCedulaAsync(cliente.Cedula, cancelacion))
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status409Conflict,
                $"Ya existe un cliente con la cedula '{cliente.Cedula}'.");
        }

        await _clienteDAO.ActualizarAsync(cliente, cancelacion);

        return new RespuestaGenerica
        {
            Exitoso = true,
            Mensaje = $"Cliente {codCliente} actualizado correctamente.",
            ProcesoHttp = "ACTUALIZAR"
        };
    }

    public async Task<RespuestaGenerica> EliminarAsync(
        int codCliente,
        CancellationToken cancelacion = default)
    {
        var existente = await _clienteDAO.ObtenerPorCodigoAsync(codCliente, cancelacion);
        if (existente is null)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status404NotFound,
                $"El cliente {codCliente} no existe.");
        }

        var cuentas = await _cuentaDAO.ObtenerPorClienteAsync(codCliente, cancelacion);
        if (cuentas.Count > 0)
        {
            throw new ExcepcionNegocio(
                StatusCodes.Status409Conflict,
                $"No se puede eliminar el cliente {codCliente}: tiene " +
                $"{cuentas.Count} cuenta(s) asociada(s).");
        }

        await _clienteDAO.EliminarAsync(codCliente, cancelacion);

        return new RespuestaGenerica
        {
            Exitoso = true,
            Mensaje = $"Cliente {codCliente} eliminado correctamente.",
            ProcesoHttp = "ELIMINAR"
        };
    }

    /// <summary>
    /// Reglas de sujeto de credito en este orden:
    /// 1. El cliente debe existir -> NO_CLIENTE.
    /// 2. Debe tener una cuenta con al menos un deposito en los ultimos 30 dias
    ///    -> SIN_DEPOSITO.
    /// 3. Si es hombre, debe tener 25 anos o mas -> EDAD_INSUFICIENTE.
    /// 4. No debe tener un credito activo -> CREDITO_ACTIVO.
    /// </summary>
    public async Task<RespuestaSujetoCreditoDTO> VerificarSujetoCreditoAsync(
        SolicitudSujetoCreditoDTO solicitud,
        CancellationToken cancelacion = default)
    {
        var cliente = await _clienteDAO.ObtenerPorCedulaAsync(
            solicitud.Cedula.Trim(), cancelacion);

        if (cliente is null)
        {
            return new RespuestaSujetoCreditoDTO
            {
                SujetoCredito = false,
                CodCliente = null,
                Motivo = "NO_CLIENTE"
            };
        }

        var cuentas = await _cuentaDAO.ObtenerPorClienteAsync(
            cliente.CodCliente, cancelacion);

        var desde = DateTime.Today.AddDays(-30);
        var tieneDeposito = false;

        foreach (var cuenta in cuentas)
        {
            var movimientos = await _movimientoDAO.ObtenerPorCuentaAsync(
                cuenta.NumCuenta, desde, cancelacion);

            if (movimientos.Any(movimiento => movimiento.Tipo == TipoMov.DEP))
            {
                tieneDeposito = true;
                break;
            }
        }

        if (!tieneDeposito)
        {
            return new RespuestaSujetoCreditoDTO
            {
                SujetoCredito = false,
                CodCliente = cliente.CodCliente,
                Motivo = "SIN_DEPOSITO"
            };
        }

        var edad = MapeoCliente.CalcularEdad(cliente.FechaNacimiento);
        if (string.Equals(cliente.Genero, "M", StringComparison.OrdinalIgnoreCase)
            && edad < 25)
        {
            return new RespuestaSujetoCreditoDTO
            {
                SujetoCredito = false,
                CodCliente = cliente.CodCliente,
                Motivo = "EDAD_INSUFICIENTE"
            };
        }

        foreach (var cuenta in cuentas)
        {
            if (await _creditoDAO.ExistePorCuentaAsync(cuenta.NumCuenta, cancelacion))
            {
                return new RespuestaSujetoCreditoDTO
                {
                    SujetoCredito = false,
                    CodCliente = cliente.CodCliente,
                    Motivo = "CREDITO_ACTIVO"
                };
            }
        }

        return new RespuestaSujetoCreditoDTO
        {
            SujetoCredito = true,
            CodCliente = cliente.CodCliente,
            Motivo = null
        };
    }
}
