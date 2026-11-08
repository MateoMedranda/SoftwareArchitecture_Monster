using ClienteConsolaConversion.Modelos;

namespace ClienteConsolaConversion.Interfaz
{
    /// <summary>
    /// Catálogo de unidades publicadas por el servidor RESTful, por categoría.
    /// Los nombres coinciden exactamente con los enums del servidor
    /// (UnidadTemperatura, UnidadLongitud y UnidadMasa).
    /// </summary>
    public static class CatalogoUnidades
    {
        private static readonly string[] Temperatura =
            { "Celsius", "Fahrenheit", "Kelvin", "Rankine" };

        private static readonly string[] Longitud =
            { "Centimetro", "Metro", "Pie", "Yarda", "Milla" };

        private static readonly string[] Masa =
            { "Kilogramo", "Gramo", "Onza", "Tonelada", "Miligramo" };

        public static IReadOnlyList<string> Obtener(CategoriaUnidad categoria)
        {
            return categoria switch
            {
                CategoriaUnidad.Temperatura => Temperatura,
                CategoriaUnidad.Longitud => Longitud,
                CategoriaUnidad.Masa => Masa,
                _ => Array.Empty<string>()
            };
        }

        public static IReadOnlyList<string> Categorias()
        {
            return new[] { "Temperatura", "Longitud", "Masa" };
        }
    }
}
