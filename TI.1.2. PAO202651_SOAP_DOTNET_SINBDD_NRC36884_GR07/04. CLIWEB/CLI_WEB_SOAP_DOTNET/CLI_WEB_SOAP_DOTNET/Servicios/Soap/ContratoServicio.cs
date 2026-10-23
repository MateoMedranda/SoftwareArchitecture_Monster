using System.Runtime.Serialization;
using System.ServiceModel;

namespace CLI_WEB_SOAP_DOTNET.Servicios.Soap;

/// <summary>
/// Contrato WCF reconstruido a partir del WSDL (targetNamespace http://monster/servicioconversionunidades).
/// Por ahora solo se expone IniciarSesion; Convertir se añadirá en el siguiente paso
/// (requiere un MessageContract por el encabezado SOAP "Token").
/// </summary>
[ServiceContract(Name = "IServicioConversionUnidades", Namespace = Espacios.Servicio)]
public interface IServicioConversionUnidadesSoap
{
    [OperationContract(Action = Espacios.Servicio + "/IServicioConversionUnidades/IniciarSesion",
                       ReplyAction = Espacios.Servicio + "/IServicioConversionUnidades/IniciarSesionResponse")]
    Task<RespuestaInicioSesion> IniciarSesionAsync(SolicitudInicioSesion solicitud);
}

public static class Espacios
{
    public const string Servicio = "http://monster/servicioconversionunidades";
}

[DataContract(Name = "SolicitudInicioSesion", Namespace = Espacios.Servicio)]
public class SolicitudInicioSesion
{
    [DataMember(Order = 0)] public string? NombreUsuario { get; set; }
    [DataMember(Order = 1)] public string? Contrasena { get; set; }
}

[DataContract(Name = "RespuestaInicioSesion", Namespace = Espacios.Servicio)]
public class RespuestaInicioSesion
{
    [DataMember(Order = 0)] public bool Exito { get; set; }
    [DataMember(Order = 1)] public string? Token { get; set; }
    [DataMember(Order = 2)] public string? Mensaje { get; set; }
}
