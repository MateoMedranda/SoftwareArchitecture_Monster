using System.Runtime.Serialization;
using CONV_SOAP_DOTNET.Modelos;

namespace CONV_SOAP_DOTNET.Contratos
{
    [DataContract(Namespace = "http://monster/servicioconversionunidades")]
    public class SolicitudInicioSesion
    {
        [DataMember(Order = 1)]
        public string NombreUsuario { get; set; }

        [DataMember(Order = 2)]
        public string Contrasena { get; set; }
    }

    [DataContract(Namespace = "http://monster/servicioconversionunidades")]
    public class RespuestaInicioSesion
    {
        [DataMember(Order = 1)]
        public bool Exito { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public string Mensaje { get; set; }
    }

    [DataContract(Namespace = "http://monster/servicioconversionunidades")]
    public class SolicitudConversion
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public CategoriaUnidad Categoria { get; set; }

        [DataMember(Order = 3)]
        public string UnidadOrigen { get; set; }

        [DataMember(Order = 4)]
        public string UnidadDestino { get; set; }

        [DataMember(Order = 5)]
        public double Valor { get; set; }
    }

    [DataContract(Namespace = "http://monster/servicioconversionunidades")]
    public class RespuestaConversion
    {
        [DataMember(Order = 1)]
        public bool Exito { get; set; }

        [DataMember(Order = 2)]
        public double ValorConvertido { get; set; }

        [DataMember(Order = 3)]
        public string Mensaje { get; set; }
    }
}
