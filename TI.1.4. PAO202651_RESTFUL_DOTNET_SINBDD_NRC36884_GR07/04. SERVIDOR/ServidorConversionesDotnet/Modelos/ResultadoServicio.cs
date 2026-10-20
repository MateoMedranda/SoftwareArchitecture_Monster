namespace ServidorConversionesDotnet.Modelos;

/// <summary>
/// Resultado genérico de un servicio: éxito con valor o error con RespuestaError.
/// </summary>
public sealed class ResultadoServicio<T>
{
    /// <summary>Indica si la operación se resolvió correctamente.</summary>
    public bool Exitoso { get; private init; }

    /// <summary>Valor obtenido cuando <see cref="Exitoso"/> es verdadero.</summary>
    public T? Valor { get; private init; }

    /// <summary>Error devuelto cuando <see cref="Exitoso"/> es falso.</summary>
    public RespuestaError? Error { get; private init; }

    /// <summary>Crea un resultado exitoso con el valor indicado.</summary>
    public static ResultadoServicio<T> ConExito(T valor) =>
        new() { Exitoso = true, Valor = valor };

    /// <summary>Crea un resultado fallido con el error indicado.</summary>
    public static ResultadoServicio<T> ConError(RespuestaError error) =>
        new() { Exitoso = false, Error = error };
}
