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

    [OperationContract(Name = "Convertir",
                       Action = Espacios.Servicio + "/IServicioConversionUnidades/Convertir",
                       ReplyAction = Espacios.Servicio + "/IServicioConversionUnidades/ConvertirResponse")]
    Task<RespuestaConversionMensaje> ConvertirAsync(SolicitudConversionMensaje peticion);
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

[DataContract(Name = "SolicitudConversion", Namespace = Espacios.Servicio)]
public class SolicitudConversion
{
    [DataMember(Order = 0)] public string? Categoria { get; set; }
    [DataMember(Order = 1)] public string? UnidadOrigen { get; set; }
    [DataMember(Order = 2)] public string? UnidadDestino { get; set; }
    [DataMember(Order = 3)] public string? Valor { get; set; }
}

[DataContract(Name = "RespuestaConversion", Namespace = Espacios.Servicio)]
public class RespuestaConversion
{
    [DataMember(Order = 0)] public bool Exito { get; set; }
    [DataMember(Order = 1)] public double ValorConvertido { get; set; }
    [DataMember(Order = 2)] public string? Mensaje { get; set; }
}

/// <summary>
/// Mensaje de petición: el WSDL define el encabezado SOAP "Token" y un cuerpo
/// &lt;solicitud&gt;&lt;Conversion&gt;…&lt;/Conversion&gt;&lt;/solicitud&gt;.
/// </summary>
[MessageContract(WrapperName = "solicitud", WrapperNamespace = Espacios.Servicio, IsWrapped = true)]
public class SolicitudConversionMensaje
{
    [MessageHeader(Name = "Token", Namespace = Espacios.Servicio)]
    public string? Token { get; set; }

    [MessageBodyMember(Name = "Conversion", Namespace = Espacios.Servicio, Order = 0)]
    public SolicitudConversion? Conversion { get; set; }
}

/// <summary>Respuesta: &lt;ConvertirResponse&gt;&lt;ConvertirResult&gt;…&lt;/ConvertirResult&gt;&lt;/ConvertirResponse&gt;.</summary>
[MessageContract(WrapperName = "ConvertirResponse", WrapperNamespace = Espacios.Servicio, IsWrapped = true)]
public class RespuestaConversionMensaje
{
    [MessageBodyMember(Name = "ConvertirResult", Namespace = Espacios.Servicio, Order = 0)]
    public RespuestaConversion? ConvertirResult { get; set; }
}