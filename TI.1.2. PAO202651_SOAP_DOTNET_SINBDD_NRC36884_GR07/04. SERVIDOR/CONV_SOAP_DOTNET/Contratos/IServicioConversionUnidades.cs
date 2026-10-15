using System.ServiceModel;

namespace CONV_SOAP_DOTNET.Contratos
{
    [ServiceContract(Namespace = "http://monster/servicioconversionunidades")]
    public interface IServicioConversionUnidades
    {
        [OperationContract]
        RespuestaInicioSesion IniciarSesion(SolicitudInicioSesion solicitud);

        [OperationContract]
        RespuestaConversion Convertir(SolicitudConversion solicitud);
    }
}
