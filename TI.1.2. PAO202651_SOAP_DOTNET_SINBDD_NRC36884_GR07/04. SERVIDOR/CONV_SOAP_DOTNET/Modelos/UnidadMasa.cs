using System.Runtime.Serialization;

namespace CONV_SOAP_DOTNET.Modelos
{
    [DataContract(Namespace = "http://monster/servicioconversionunidades")]
    public enum UnidadMasa
    {
        [EnumMember]
        Kilogramo,

        [EnumMember]
        Gramo,

        [EnumMember]
        Onza,

        [EnumMember]
        Tonelada
    }
}
