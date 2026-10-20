using System.Runtime.Serialization;

namespace CONV_SOAP_DOTNET.Modelos
{
    [DataContract(Namespace = "http://monster/servicioconversionunidades")]
    public enum UnidadLongitud
    {
        [EnumMember]
        Centimetro,

        [EnumMember]
        Metro,

        [EnumMember]
        Kilometro,

        [EnumMember]
        Yarda,

        [EnumMember]
        Milla
    }
}
