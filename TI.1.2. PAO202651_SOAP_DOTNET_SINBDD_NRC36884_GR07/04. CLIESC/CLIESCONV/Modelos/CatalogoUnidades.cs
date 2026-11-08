using System.Collections.Generic;

namespace ClienteEscritorioConversion.Modelos
{
    /// <summary>
    /// Catálogo de unidades publicadas por el servidor SOAP, por categoría.
    /// Los nombres coinciden exactamente con los enums del servidor
    /// (UnidadTemperatura, UnidadLongitud y UnidadMasa).
    /// </summary>
    public static class CatalogoUnidades
    {
        private static readonly string[] Temperatura =
            { "Celsius", "Fahrenheit", "Kelvin", "Rankine" };

        private static readonly string[] Longitud =
            { "Centimetro", "Metro", "Kilometro", "Yarda", "Milla" };

        private static readonly string[] Masa =
            { "Kilogramo", "Gramo", "Onza", "Tonelada" };

        public static IReadOnlyList<CategoriaUnidad> Categorias { get; } =
            new[] { CategoriaUnidad.Temperatura, CategoriaUnidad.Longitud, CategoriaUnidad.Masa };

        public static IReadOnlyList<string> Obtener(CategoriaUnidad categoria)
        {
            return categoria switch
            {
                CategoriaUnidad.Temperatura => Temperatura,
                CategoriaUnidad.Longitud => Longitud,
                CategoriaUnidad.Masa => Masa,
                _ => System.Array.Empty<string>()
            };
        }
    }
}
