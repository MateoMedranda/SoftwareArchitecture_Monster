using System;
using System.Collections.Generic;
using CONV_SOAP_DOTNET.Modelos;

namespace CONV_SOAP_DOTNET.Servicios.Conversores
{
    public class ConversorTemperatura : IConversorUnidad
    {
        private readonly Dictionary<string, UnidadTemperatura> unidades =
            new Dictionary<string, UnidadTemperatura>(StringComparer.OrdinalIgnoreCase)
            {
                { "Celsius", UnidadTemperatura.Celsius },
                { "Fahrenheit", UnidadTemperatura.Fahrenheit },
                { "Kelvin", UnidadTemperatura.Kelvin },
                { "Rankine", UnidadTemperatura.Rankine }
            };

        public CategoriaUnidad Categoria
        {
            get { return CategoriaUnidad.Temperatura; }
        }

        public bool EsUnidadValida(string unidad)
        {
            return !string.IsNullOrWhiteSpace(unidad) && unidades.ContainsKey(unidad.Trim());
        }

        public void ValidarRango(string unidad, double valor)
        {
            UnidadTemperatura temperatura = Resolver(unidad);
            RangoUnidad rango = Rangos.Temperatura[temperatura];

            if (!rango.Contiene(valor))
            {
                throw new ArgumentOutOfRangeException(
                    "valor",
                    string.Format(
                        "El valor {0} está fuera del rango físico del agua a 1 atm para {1} ({2} a {3}).",
                        valor,
                        temperatura,
                        rango.LimiteInferior,
                        rango.LimiteSuperior));
            }
        }

        public double Convertir(string unidadOrigen, string unidadDestino, double valor)
        {
            double celsius = ConvertirACelsius(Resolver(unidadOrigen), valor);
            return ConvertirDesdeCelsius(Resolver(unidadDestino), celsius);
        }

        private UnidadTemperatura Resolver(string unidad)
        {
            UnidadTemperatura temperatura;
            if (string.IsNullOrWhiteSpace(unidad) || !unidades.TryGetValue(unidad.Trim(), out temperatura))
            {
                throw new ArgumentException(
                    string.Format("La unidad '{0}' no es válida para la categoría Temperatura.", unidad),
                    "unidad");
            }

            return temperatura;
        }

        private static double ConvertirACelsius(UnidadTemperatura unidad, double valor)
        {
            switch (unidad)
            {
                case UnidadTemperatura.Celsius:
                    return valor;
                case UnidadTemperatura.Fahrenheit:
                    return (valor - 32.0) * 5.0 / 9.0;
                case UnidadTemperatura.Kelvin:
                    return valor - 273.15;
                case UnidadTemperatura.Rankine:
                    return (valor - 491.67) * 5.0 / 9.0;
                default:
                    throw new ArgumentOutOfRangeException("unidad");
            }
        }

        private static double ConvertirDesdeCelsius(UnidadTemperatura unidad, double celsius)
        {
            switch (unidad)
            {
                case UnidadTemperatura.Celsius:
                    return celsius;
                case UnidadTemperatura.Fahrenheit:
                    return celsius * 9.0 / 5.0 + 32.0;
                case UnidadTemperatura.Kelvin:
                    return celsius + 273.15;
                case UnidadTemperatura.Rankine:
                    return (celsius + 273.15) * 9.0 / 5.0;
                default:
                    throw new ArgumentOutOfRangeException("unidad");
            }
        }
    }
}
