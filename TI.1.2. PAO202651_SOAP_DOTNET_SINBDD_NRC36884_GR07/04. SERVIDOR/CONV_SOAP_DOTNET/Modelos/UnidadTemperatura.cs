using System.Runtime.Serialization;

namespace CONV_SOAP_DOTNET.Modelos
{
    [DataContract(Namespace = "http://monster/servicioconversionunidades")]
    public enum UnidadTemperatura
    {
        [EnumMember]
        Celsius,

        [EnumMember]
        Fahrenheit,

        [EnumMember]
        Kelvin,

        [EnumMember]
        Rankine
    }
}
