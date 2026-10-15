using System.Runtime.Serialization;

namespace CONV_SOAP_DOTNET.Modelos
{
    [DataContract(Namespace = "http://monster/servicioconversionunidades")]
    public enum CategoriaUnidad
    {
        [EnumMember]
        Temperatura,

        [EnumMember]
        Longitud,

        [EnumMember]
        Masa
    }
}
