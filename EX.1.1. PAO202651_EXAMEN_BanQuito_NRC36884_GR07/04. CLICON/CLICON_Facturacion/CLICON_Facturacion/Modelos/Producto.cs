using System.Runtime.Serialization;

namespace CLICON_Facturacion.Modelos
{
    [DataContract]
    public sealed class Producto
    {
        [DataMember(Name = "numInventario")]
        public int NumInventario { get; set; }

        [DataMember(Name = "nombre")]
        public string Nombre { get; set; }

        [DataMember(Name = "precio")]
        public decimal Precio { get; set; }

        public Producto()
        {
            Nombre = string.Empty;
        }

        public Producto Copiar()
        {
            return new Producto
            {
                NumInventario = NumInventario,
                Nombre = Nombre,
                Precio = Precio
            };
        }
    }
}
