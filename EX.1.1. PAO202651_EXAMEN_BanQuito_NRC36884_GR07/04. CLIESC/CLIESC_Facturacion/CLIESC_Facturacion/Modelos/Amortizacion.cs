using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CLIESC_Facturacion.Modelos
{
    [DataContract]
    public sealed class RespuestaAmortizacion
    {
        [DataMember(Name = "numCredito")]
        public int NumCredito { get; set; }

        [DataMember(Name = "cuotas")]
        public List<CuotaAmortizacion> Cuotas { get; set; }

        public RespuestaAmortizacion()
        {
            Cuotas = new List<CuotaAmortizacion>();
        }
    }

    [DataContract]
    public sealed class CuotaAmortizacion
    {
        [DataMember(Name = "numCuota")]
        public int NumCuota { get; set; }

        [DataMember(Name = "valorCuota")]
        public decimal ValorCuota { get; set; }

        [DataMember(Name = "interesPagado")]
        public decimal InteresPagado { get; set; }

        [DataMember(Name = "capitalPagado")]
        public decimal CapitalPagado { get; set; }

        [DataMember(Name = "saldo")]
        public decimal Saldo { get; set; }

        [DataMember(Name = "fechaPago")]
        public string FechaPago { get; set; }

        [DataMember(Name = "pagada")]
        public bool Pagada { get; set; }

        public CuotaAmortizacion()
        {
            FechaPago = string.Empty;
        }
    }
}
