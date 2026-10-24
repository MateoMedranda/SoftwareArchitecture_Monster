using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteEscritorioConversion.Modelos
{
    public class RespuestaConversion
    {
        public bool Exito { get; set; }
        public double ValorConvertido { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
