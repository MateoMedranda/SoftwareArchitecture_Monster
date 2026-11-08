namespace ServidorConversionesDotnet.Utilidades;

/// <summary>
/// Convierte cadenas de texto a los enums de unidades, sin distinción de
/// mayúsculas/minúsculas y rechazando valores numéricos o desconocidos.
/// </summary>
public static class ConversorEnum
{
    /// <summary>
    /// Intenta convertir el texto recibido al enum <typeparamref name="T"/>.
    /// Devuelve falso si el texto está vacío, contiene dígitos o no coincide
    /// con ninguno de los nombres del enum.
    /// </summary>
    public static bool IntentarConvertir<T>(string? texto, out T unidad) where T : struct, Enum
    {
        unidad = default;

        if (string.IsNullOrWhiteSpace(texto) || texto.Any(char.IsDigit))
        {
            return false;
        }

        var candidato = texto.Trim();

        foreach (var nombre in Enum.GetNames<T>())
        {
            if (string.Equals(nombre, candidato, StringComparison.OrdinalIgnoreCase))
            {
                unidad = Enum.Parse<T>(nombre);
                return true;
            }
        }

        return false;
    }
}
