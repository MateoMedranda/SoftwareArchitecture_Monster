namespace CLIWEB_Facturacion.Servicios;

/// <summary>
/// Error controlado del cliente. Conserva el estado HTTP y el mensaje
/// devuelto por el API para mostrarlo en la interfaz.
/// </summary>
public sealed class ApiException : Exception
{
    public int Estado { get; }

    public ApiException(int estado, string mensaje) : base(mensaje)
    {
        Estado = estado;
    }
}
