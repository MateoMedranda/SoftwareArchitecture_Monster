using System.Runtime.Serialization;

namespace CLIESC_Facturacion.Modelos
{
    /// <summary>
    /// Respuesta estandar de los endpoints de escritura del API.
    /// </summary>
    [DataContract]
    public sealed class RespuestaApi
    {
        [DataMember(Name = "exitoso")]
        public bool Exitoso { get; set; }

        [DataMember(Name = "mensaje")]
        public string Mensaje { get; set; }

        public RespuestaApi()
        {
            Mensaje = string.Empty;
        }
    }
}
