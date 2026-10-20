using System.Collections.Generic;

namespace CONV_SOAP_DOTNET.Modelos
{
    public class RangoUnidad
    {
        private readonly double limiteInferior;
        private readonly double limiteSuperior;

        public RangoUnidad(double limiteInferior, double limiteSuperior)
        {
            this.limiteInferior = limiteInferior;
            this.limiteSuperior = limiteSuperior;
        }

        public double LimiteInferior
        {
            get { return limiteInferior; }
        }

        public double LimiteSuperior
        {
            get { return limiteSuperior; }
        }

        public bool Contiene(double valor)
        {
            return valor > limiteInferior && valor < limiteSuperior;
        }
    }

    public static class Rangos
    {
        // Límites físicos del agua a 1 atm.
        public static readonly Dictionary<UnidadTemperatura, RangoUnidad> Temperatura =
            new Dictionary<UnidadTemperatura, RangoUnidad>
            {
                { UnidadTemperatura.Celsius, new RangoUnidad(0.0, 100.0) },
                { UnidadTemperatura.Fahrenheit, new RangoUnidad(32.0, 212.0) },
                { UnidadTemperatura.Kelvin, new RangoUnidad(273.15, 373.15) },
                { UnidadTemperatura.Rankine, new RangoUnidad(491.67, 671.67) }
            };

        public static readonly Dictionary<UnidadMasa, RangoUnidad> Masa =
            new Dictionary<UnidadMasa, RangoUnidad>
            {
                { UnidadMasa.Gramo, new RangoUnidad(0.0,1.7976931348623157E+308) },
                { UnidadMasa.Onza, new RangoUnidad(0.0, 1.7976931348623157E+308) },
                { UnidadMasa.Tonelada, new RangoUnidad(0.0, 1.7976931348623157E+308) },
                { UnidadMasa.Kilogramo, new RangoUnidad(0.0, 1.7976931348623157E+308) }
            };

        public static readonly Dictionary<UnidadLongitud, RangoUnidad> Longitud =
            new Dictionary<UnidadLongitud, RangoUnidad>
            {
                { UnidadLongitud.Milla, new RangoUnidad(0.0, 1.7976931348623157E+308) },
                { UnidadLongitud.Yarda, new RangoUnidad(0.0, 1.7976931348623157E+308) },
                { UnidadLongitud.Metro, new RangoUnidad(0.0, 1.7976931348623157E+308) },
                { UnidadLongitud.Kilometro, new RangoUnidad(0.0, 1.7976931348623157E+308) },
                { UnidadLongitud.Centimetro, new RangoUnidad(0.0, 1.7976931348623157E+308) }
            };
    }
}
