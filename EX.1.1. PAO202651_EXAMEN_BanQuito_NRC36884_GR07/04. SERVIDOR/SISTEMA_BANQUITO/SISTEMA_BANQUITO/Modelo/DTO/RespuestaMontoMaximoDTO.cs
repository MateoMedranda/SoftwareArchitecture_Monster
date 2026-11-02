namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Respuesta JSON con el monto maximo calculado para un cliente.
/// </summary>
public sealed class RespuestaMontoMaximoDTO
{
    public int CodCliente { get; set; }

    public decimal MontoMaximo { get; set; }
}
