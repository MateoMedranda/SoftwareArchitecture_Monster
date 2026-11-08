using System;
using System.Collections.Generic;
using CONV_SOAP_DOTNET.Modelos;

namespace CONV_SOAP_DOTNET.Servicios.Conversores
{
    public class RegistroConversores
    {
        private static readonly RegistroConversores InstanciaCompartida = new RegistroConversores();

        private readonly Dictionary<CategoriaUnidad, IConversorUnidad> conversores;

        private RegistroConversores()
        {
            conversores = new Dictionary<CategoriaUnidad, IConversorUnidad>();
            Registrar(new ConversorTemperatura());
            Registrar(new ConversorLongitud());
            Registrar(new ConversorMasa());
        }

        public static RegistroConversores Actual
        {
            get { return InstanciaCompartida; }
        }

        public void Registrar(IConversorUnidad conversor)
        {
            if (conversor == null)
            {
                throw new ArgumentNullException("conversor");
            }

            conversores[conversor.Categoria] = conversor;
        }

        public IConversorUnidad Obtener(CategoriaUnidad categoria)
        {
            IConversorUnidad conversor;
            if (!conversores.TryGetValue(categoria, out conversor))
            {
                throw new ArgumentOutOfRangeException(
                    "categoria",
                    string.Format("No existe un conversor registrado para la categoría '{0}'.", categoria));
            }

            return conversor;
        }
    }
}
