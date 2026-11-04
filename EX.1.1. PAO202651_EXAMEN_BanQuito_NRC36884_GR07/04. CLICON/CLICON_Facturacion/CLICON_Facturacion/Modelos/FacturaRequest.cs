using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CLICON_Facturacion.Modelos
{
    [DataContract]
    public sealed class FacturaRequest
    {
        [DataMember(Name = "codCliente")]
        public int CodCliente { get; set; }

        [DataMember(Name = "tipoPago")]
        public string TipoPago { get; set; }

        [DataMember(Name = "numCuotas", EmitDefaultValue = false)]
        public int? NumCuotas { get; set; }

        [DataMember(Name = "items")]
        public List<FacturaItemRequest> Items { get; set; }

        public FacturaRequest()
        {
            TipoPago = string.Empty;
            Items = new List<FacturaItemRequest>();
        }
    }

    [DataContract]
    public sealed class FacturaItemRequest
    {
        [DataMember(Name = "numInventario")]
        public int NumInventario { get; set; }

        [DataMember(Name = "cantidad")]
        public int Cantidad { get; set; }
    }
}
