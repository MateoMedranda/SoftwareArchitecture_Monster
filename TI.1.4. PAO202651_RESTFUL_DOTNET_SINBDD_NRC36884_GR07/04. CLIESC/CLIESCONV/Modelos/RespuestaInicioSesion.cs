using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteEscritorioConversion.Modelos
{
    public class RespuestaInicioSesion
    {
        public bool Exito { get; set; }
        public string? Token { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
