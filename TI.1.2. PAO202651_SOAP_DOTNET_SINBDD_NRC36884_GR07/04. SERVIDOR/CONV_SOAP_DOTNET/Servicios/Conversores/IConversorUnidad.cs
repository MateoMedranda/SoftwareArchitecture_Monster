using CONV_SOAP_DOTNET.Modelos;

namespace CONV_SOAP_DOTNET.Servicios.Conversores
{
    public interface IConversorUnidad
    {
        CategoriaUnidad Categoria { get; }

        bool EsUnidadValida(string unidad);

        double Convertir(string unidadOrigen, string unidadDestino, double valor);

        void ValidarRango(string unidad, double valor);
    }
}
