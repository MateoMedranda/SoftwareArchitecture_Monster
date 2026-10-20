using System;
using System.Collections.Generic;
using CONV_SOAP_DOTNET.Modelos;

namespace CONV_SOAP_DOTNET.Servicios.Conversores
{
    public class ConversorLongitud : IConversorUnidad
    {
        private const double MetrosPorYarda = 0.9144;
        private const double MetrosPorMilla = 1609.344;

        private readonly Dictionary<string, UnidadLongitud> unidades =
            new Dictionary<string, UnidadLongitud>(StringComparer.Ordinal)
            {
                { "MILLA", UnidadLongitud.Milla },
                { "YARDA", UnidadLongitud.Yarda },
                { "KILOMETRO", UnidadLongitud.Kilometro },
                { "METRO", UnidadLongitud.Metro },
                { "CENTIMETRO", UnidadLongitud.Centimetro }
            };

        public CategoriaUnidad Categoria
        {
            get { return CategoriaUnidad.Longitud; }
        }

        public bool EsUnidadValida(string unidad)
        {
            string clave = Normalizar(unidad);
            return clave != null && unidades.ContainsKey(clave);
        }

        public void ValidarRango(string unidad, double valor)
        {
            UnidadLongitud longitud = Resolver(unidad);
            RangoUnidad rango = Rangos.Longitud[longitud];

            if (!rango.Contiene(valor))
            {
                throw new ArgumentException(MensajesError.Formatear(
                    MensajesError.ValorFueraDeRango,
                    valor,
                    longitud,
                    rango.LimiteInferior,
                    rango.LimiteSuperior));
            }
        }

        public double Convertir(string unidadOrigen, string unidadDestino, double valor)
        {
            double metros = ConvertirAMetros(Resolver(unidadOrigen), valor);
            return ConvertirDesdeMetros(Resolver(unidadDestino), metros);
        }

        private UnidadLongitud Resolver(string unidad)
        {
            string clave = Normalizar(unidad);
            UnidadLongitud longitud;
            if (clave == null || !unidades.TryGetValue(clave, out longitud))
            {
                throw new ArgumentException(MensajesError.Formatear(
                    MensajesError.UnidadInvalidaCategoria, unidad, Categoria));
            }

            return longitud;
        }

        private static string Normalizar(string unidad)
        {
            return string.IsNullOrWhiteSpace(unidad) ? null : unidad.Trim().ToUpperInvariant();
        }

        private static double ConvertirAMetros(UnidadLongitud unidad, double valor)
        {
            switch (unidad)
            {
                case UnidadLongitud.Centimetro:
                    return valor / 100.0;
                case UnidadLongitud.Metro:
                    return valor;
                case UnidadLongitud.Kilometro:
                    return valor * 1000.0;
                case UnidadLongitud.Yarda:
                    return valor * MetrosPorYarda;
                case UnidadLongitud.Milla:
                    return valor * MetrosPorMilla;
                default:
                    throw new ArgumentOutOfRangeException("unidad");
            }
        }

        private static double ConvertirDesdeMetros(UnidadLongitud unidad, double metros)
        {
            switch (unidad)
            {
                case UnidadLongitud.Centimetro:
                    return metros * 100.0;
                case UnidadLongitud.Metro:
                    return metros;
                case UnidadLongitud.Kilometro:
                    return metros / 1000.0;
                case UnidadLongitud.Yarda:
                    return metros / MetrosPorYarda;
                case UnidadLongitud.Milla:
                    return metros / MetrosPorMilla;
                default:
                    throw new ArgumentOutOfRangeException("unidad");
            }
        }
    }
}
