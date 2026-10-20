using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CONV_SOAP_DOTNET.Modelos
{
    [DataContract(Namespace = "http://monster/servicioconversionunidades")]
    public enum CategoriaUnidad
    {
        [EnumMember]
        Temperatura,

        [EnumMember]
        Longitud,

        [EnumMember]
        Masa
    }

    /// <summary>
    /// Resuelve el nombre de una categoría sin importar mayúsculas/minúsculas
    /// ("temperatura", "TEMPERATURA" y "Temperatura" son equivalentes).
    /// </summary>
    public static class CategoriasUnidad
    {
        private static readonly Dictionary<string, CategoriaUnidad> Mapa =
            new Dictionary<string, CategoriaUnidad>(StringComparer.Ordinal)
            {
                { "TEMPERATURA", CategoriaUnidad.Temperatura },
                { "LONGITUD", CategoriaUnidad.Longitud },
                { "MASA", CategoriaUnidad.Masa }
            };

        public static bool TryResolver(string categoria, out CategoriaUnidad resultado)
        {
            resultado = default(CategoriaUnidad);
            if (string.IsNullOrWhiteSpace(categoria))
            {
                return false;
            }

            return Mapa.TryGetValue(categoria.Trim().ToUpperInvariant(), out resultado);
        }

        public static bool EsValida(string categoria)
        {
            CategoriaUnidad resultado;
            return TryResolver(categoria, out resultado);
        }
    }
}
