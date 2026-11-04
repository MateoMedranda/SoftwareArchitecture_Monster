using System.Runtime.Serialization;

namespace CLICON_Facturacion.Modelos
{
    [DataContract]
    public sealed class Cliente
    {
        [DataMember(Name = "codCliente")]
        public int CodCliente { get; set; }

        [DataMember(Name = "cedula")]
        public string Cedula { get; set; }

        [DataMember(Name = "nombre")]
        public string Nombre { get; set; }

        public Cliente()
        {
            Cedula = string.Empty;
            Nombre = string.Empty;
        }

        public Cliente Copiar()
        {
            return new Cliente
            {
                CodCliente = CodCliente,
                Cedula = Cedula,
                Nombre = Nombre
            };
        }
    }
}
