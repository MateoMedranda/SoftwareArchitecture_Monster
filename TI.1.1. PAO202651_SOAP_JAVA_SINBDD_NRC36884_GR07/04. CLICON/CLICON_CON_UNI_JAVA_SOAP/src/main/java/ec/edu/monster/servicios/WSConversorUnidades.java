package ec.edu.monster.servicios;

import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import jakarta.jws.WebService;

/** Contrato SOAP del servidor; JAX-WS crea el proxy a partir del WSDL configurado. */
@WebService(name = "WSConversorUnidades", targetNamespace = "http://ws.monster.edu.ec/")
public interface WSConversorUnidades {
    @WebMethod String login(@WebParam(name = "usuario") String usuario,
                            @WebParam(name = "contrasenia") String contrasenia);

    @WebMethod String cambiarContrasenia(@WebParam(name = "contraseniaActual") String actual,
                                         @WebParam(name = "contraseniaNueva") String nueva);

    @WebMethod double convertirTemperatura(@WebParam(name = "valor") double valor,
                                           @WebParam(name = "unidadInicial") String origen,
                                           @WebParam(name = "unidadFinal") String destino);

    @WebMethod double convertirMasa(@WebParam(name = "valor") double valor,
                                    @WebParam(name = "unidadInicial") String origen,
                                    @WebParam(name = "unidadFinal") String destino);

    @WebMethod double convertirLongitud(@WebParam(name = "valor") double valor,
                                        @WebParam(name = "unidadInicial") String origen,
                                        @WebParam(name = "unidadFinal") String destino);
}
