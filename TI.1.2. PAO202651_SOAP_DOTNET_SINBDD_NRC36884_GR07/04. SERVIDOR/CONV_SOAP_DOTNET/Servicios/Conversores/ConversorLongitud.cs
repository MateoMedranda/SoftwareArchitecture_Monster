using System;
using CONV_SOAP_DOTNET.Modelos;

namespace CONV_SOAP_DOTNET.Servicios.Conversores
{
    // TODO: implementación luego (unidades, factores y rangos de longitud)
    public class ConversorLongitud : IConversorUnidad
    {
        private const string MensajeNoImplementado = "La categoría 'Longitud' aún no está implementada.";

        public CategoriaUnidad Categoria
        {
            get { return CategoriaUnidad.Longitud; }
        }

        public bool EsUnidadValida(string unidad)
        {
            return false;
        }

        public void ValidarRango(string unidad, double valor)
        {
            throw new NotSupportedException(MensajeNoImplementado);
        }

        public double Convertir(string unidadOrigen, string unidadDestino, double valor)
        {
            throw new NotSupportedException(MensajeNoImplementado);
        }
    }
}
