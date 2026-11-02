namespace SISTEMA_BANQUITO.Comun;

/// <summary>
/// Excepcion de negocio con el codigo HTTP que debe devolver la API.
/// </summary>
public sealed class ExcepcionNegocio : Exception
{
    public ExcepcionNegocio(int estado, string mensaje) : base(mensaje)
    {
        Estado = estado;
    }

    /// <summary>
    /// Codigo de estado HTTP asociado al error.
    /// </summary>
    public int Estado { get; }
}
