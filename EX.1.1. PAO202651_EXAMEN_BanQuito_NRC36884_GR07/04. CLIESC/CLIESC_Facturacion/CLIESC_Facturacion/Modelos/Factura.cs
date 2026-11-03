using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CLIESC_Facturacion.Modelos
{
    [DataContract]
    public sealed class Factura
    {
        [DataMember(Name = "numFactura")]
        public int NumFactura { get; set; }

        [DataMember(Name = "codCliente")]
        public int CodCliente { get; set; }

        [DataMember(Name = "cedula")]
        public string Cedula { get; set; }

        [DataMember(Name = "nombreCliente")]
        public string NombreCliente { get; set; }

        [DataMember(Name = "fecha")]
        public string Fecha { get; set; }

        [DataMember(Name = "tipoPago")]
        public string TipoPago { get; set; }

        [DataMember(Name = "valor")]
        public decimal Valor { get; set; }

        [DataMember(Name = "numCredito")]
        public int? NumCredito { get; set; }

        [DataMember(Name = "items")]
        public List<FacturaItem> Items { get; set; }

        public Factura()
        {
            Cedula = string.Empty;
            NombreCliente = string.Empty;
            Fecha = string.Empty;
            TipoPago = string.Empty;
            Items = new List<FacturaItem>();
        }
    }

    [DataContract]
    public sealed class FacturaItem
    {
        [DataMember(Name = "numItem")]
        public int NumItem { get; set; }

        [DataMember(Name = "numInventario")]
        public int NumInventario { get; set; }

        [DataMember(Name = "nombreProducto")]
        public string NombreProducto { get; set; }

        [DataMember(Name = "valor")]
        public decimal Valor { get; set; }

        [DataMember(Name = "cantidad")]
        public int Cantidad { get; set; }

        public FacturaItem()
        {
            NombreProducto = string.Empty;
        }
    }
}
