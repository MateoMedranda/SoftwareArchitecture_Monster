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
            return valor >= limiteInferior && valor <= limiteSuperior;
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

        // TODO: implementación luego (Longitud)
        // TODO: implementación luego (Masa)
    }
}
