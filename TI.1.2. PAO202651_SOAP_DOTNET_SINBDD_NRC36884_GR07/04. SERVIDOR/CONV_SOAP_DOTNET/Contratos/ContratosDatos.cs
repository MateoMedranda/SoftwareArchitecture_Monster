using System.Runtime.Serialization;
using System.ServiceModel;
using CONV_SOAP_DOTNET.Modelos;

namespace CONV_SOAP_DOTNET.Contratos
{
    [MessageContract(IsWrapped = true, WrapperName = "solicitud", WrapperNamespace = "http://monster/servicioconversionunidades")]
    public class SolicitudConversionMensaje
    {
        [MessageHeader(Namespace = "http://monster/servicioconversionunidades")]
        public string Token { get; set; }

        [MessageBodyMember(Order = 1, Namespace = "http://monster/servicioconversionunidades")]
        public SolicitudConversion Conversion { get; set; }
    }

    [MessageContract(IsWrapped = true, WrapperName = "ConvertirResponse", WrapperNamespace = "http://monster/servicioconversionunidades")]
    public class RespuestaConversionMensaje
    {
        [MessageBodyMember(Name = "ConvertirResult", Order = 1, Namespace = "http://monster/servicioconversionunidades")]
        public RespuestaConversion Resultado { get; set; }
    }

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
        public string Categoria { get; set; }

        [DataMember(Order = 2)]
        public string UnidadOrigen { get; set; }

        [DataMember(Order = 3)]
        public string UnidadDestino { get; set; }

        [DataMember(Order = 4)]
        public string Valor { get; set; }
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
