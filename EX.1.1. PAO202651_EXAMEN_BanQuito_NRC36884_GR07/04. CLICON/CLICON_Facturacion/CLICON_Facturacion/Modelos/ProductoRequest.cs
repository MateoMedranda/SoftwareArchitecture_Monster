using System.Runtime.Serialization;

namespace CLICON_Facturacion.Modelos
{
    [DataContract]
    public sealed class ProductoRequest
    {
        [DataMember(Name = "nombre")]
        public string Nombre { get; set; }

        [DataMember(Name = "precio")]
        public decimal Precio { get; set; }

        public ProductoRequest()
        {
            Nombre = string.Empty;
        }
    }
}
