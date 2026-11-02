namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Respuesta estandar para operaciones de creacion, actualizacion,
/// eliminacion y pago que no devuelven una entidad completa.
/// </summary>
public sealed class RespuestaGenerica
{
    public bool Exitoso { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public string ProcesoHttp { get; set; } = string.Empty;
}
