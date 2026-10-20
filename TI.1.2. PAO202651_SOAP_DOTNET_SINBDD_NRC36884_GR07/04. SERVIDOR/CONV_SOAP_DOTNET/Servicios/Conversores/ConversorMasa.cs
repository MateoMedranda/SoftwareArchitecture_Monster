using System;
using System.Collections.Generic;
using CONV_SOAP_DOTNET.Modelos;

namespace CONV_SOAP_DOTNET.Servicios.Conversores
{
    public class ConversorMasa : IConversorUnidad
    {
        private const double KilogramosPorOnza = 0.028349523125;

        private readonly Dictionary<string, UnidadMasa> unidades =
            new Dictionary<string, UnidadMasa>(StringComparer.Ordinal)
            {
                { "TONELADA", UnidadMasa.Tonelada },
                { "ONZA", UnidadMasa.Onza },
                { "KILOGRAMO", UnidadMasa.Kilogramo },
                { "GRAMO", UnidadMasa.Gramo }
            };

        public CategoriaUnidad Categoria
        {
            get { return CategoriaUnidad.Masa; }
        }

        public bool EsUnidadValida(string unidad)
        {
            string clave = Normalizar(unidad);
            return clave != null && unidades.ContainsKey(clave);
        }

        public void ValidarRango(string unidad, double valor)
        {
            UnidadMasa masa = Resolver(unidad);
            RangoUnidad rango = Rangos.Masa[masa];

            if (!rango.Contiene(valor))
            {
                throw new ArgumentException(MensajesError.Formatear(
                    MensajesError.ValorFueraDeRango,
                    valor,
                    masa,
                    rango.LimiteInferior,
                    rango.LimiteSuperior));
            }
        }

        public double Convertir(string unidadOrigen, string unidadDestino, double valor)
        {
            double kilogramos = ConvertirAKilogramos(Resolver(unidadOrigen), valor);
            return ConvertirDesdeKilogramos(Resolver(unidadDestino), kilogramos);
        }

        private UnidadMasa Resolver(string unidad)
        {
            string clave = Normalizar(unidad);
            UnidadMasa masa;
            if (clave == null || !unidades.TryGetValue(clave, out masa))
            {
                throw new ArgumentException(MensajesError.Formatear(
                    MensajesError.UnidadInvalidaCategoria, unidad, Categoria));
            }

            return masa;
        }

        private static string Normalizar(string unidad)
        {
            return string.IsNullOrWhiteSpace(unidad) ? null : unidad.Trim().ToUpperInvariant();
        }

        private static double ConvertirAKilogramos(UnidadMasa unidad, double valor)
        {
            switch (unidad)
            {
                case UnidadMasa.Kilogramo:
                    return valor;
                case UnidadMasa.Gramo:
                    return valor / 1000.0;
                case UnidadMasa.Onza:
                    return valor * KilogramosPorOnza;
                case UnidadMasa.Tonelada:
                    return valor * 1000.0;
                default:
                    throw new ArgumentOutOfRangeException("unidad");
            }
        }

        private static double ConvertirDesdeKilogramos(UnidadMasa unidad, double kilogramos)
        {
            switch (unidad)
            {
                case UnidadMasa.Kilogramo:
                    return kilogramos;
                case UnidadMasa.Gramo:
                    return kilogramos * 1000.0;
                case UnidadMasa.Onza:
                    return kilogramos / KilogramosPorOnza;
                case UnidadMasa.Tonelada:
                    return kilogramos / 1000.0;
                default:
                    throw new ArgumentOutOfRangeException("unidad");
            }
        }
    }
}
