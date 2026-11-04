using System.Runtime.Serialization;

namespace CLICON_Facturacion.Modelos
{
    [DataContract]
    public sealed class ClienteRequest
    {
        [DataMember(Name = "cedula")]
        public string Cedula { get; set; }

        [DataMember(Name = "nombre")]
        public string Nombre { get; set; }

        public ClienteRequest()
        {
            Cedula = string.Empty;
            Nombre = string.Empty;
        }
    }
}
