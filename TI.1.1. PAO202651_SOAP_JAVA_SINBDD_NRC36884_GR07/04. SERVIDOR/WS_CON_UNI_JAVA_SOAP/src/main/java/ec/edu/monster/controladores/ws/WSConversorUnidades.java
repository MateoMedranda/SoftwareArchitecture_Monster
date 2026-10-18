/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/WebServices/WebService.java to edit this template
 */
package ec.edu.monster.controladores.ws;

import ec.edu.monster.modelos.utilidades.ConversorLongitud;
import ec.edu.monster.modelos.utilidades.ConversorMasa;
import ec.edu.monster.modelos.utilidades.ConversorTemperatura;
import ec.edu.monster.modelos.utilidades.enums.UnidadLongitud;
import ec.edu.monster.modelos.utilidades.enums.UnidadMasa;
import ec.edu.monster.modelos.utilidades.enums.UnidadTemperatura;
import ec.edu.monster.modelos.utilidades.mapeadores.UnidadMapper;
import ec.edu.monster.seguridad.AdministradorCredenciales;
import ec.edu.monster.seguridad.AdministradorTokens;
import ec.edu.monster.servicios.ServicioConversor;
import jakarta.enterprise.inject.spi.CDI;
import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import jakarta.jws.HandlerChain;
import jakarta.xml.soap.SOAPConstants;
import jakarta.xml.soap.SOAPException;
import jakarta.xml.soap.SOAPFactory;
import jakarta.xml.soap.SOAPFault;
import jakarta.xml.ws.soap.SOAPFaultException;
import javax.xml.namespace.QName;

/**
 *
 * @author USER
 */
@WebService(
        serviceName = "WSConversorUnidades",
        targetNamespace = "http://ws.monster.edu.ec/")
@HandlerChain(file = "manejador-auth.xml")
public class WSConversorUnidades {

    private final ServicioConversor<UnidadLongitud> servicioLongitud =
            new ServicioConversor<>(new ConversorLongitud());

    private final ServicioConversor<UnidadMasa> servicioMasa =
            new ServicioConversor<>(new ConversorMasa());

    private final ServicioConversor<UnidadTemperatura> servicioTemperatura =
            new ServicioConversor<>(new ConversorTemperatura());

    @WebMethod(operationName = "login")
    public String login(
            @WebParam(name = "usuario") String usuario,
            @WebParam(name = "contrasenia") String contrasenia) {
        
        AdministradorTokens administradorTokens = CDI.current().select(AdministradorTokens.class).get();
        
        if (AdministradorCredenciales.validarCredenciales(usuario, contrasenia)) {
            return administradorTokens.generarToken();
        }
        
        try {
            SOAPFault fault = SOAPFactory.newInstance(SOAPConstants.SOAP_1_1_PROTOCOL).createFault();
            fault.setFaultCode(new QName(SOAPConstants.URI_NS_SOAP_ENVELOPE, "Client"));
            fault.setFaultString("Usuario o contraseña incorrectos");
            throw new SOAPFaultException(fault);
        } catch (SOAPException e) {
            throw new RuntimeException("Error de autenticación", e);
        }
    }

    @WebMethod(operationName = "cambiarContrasenia")
    public String cambiarContrasenia(
            @WebParam(name = "contraseniaActual") String contraseniaActual,
            @WebParam(name = "contraseniaNueva") String contraseniaNueva) {
        try {
            AdministradorCredenciales.cambiarContrasenia(contraseniaActual, contraseniaNueva);
            return "Contrasenia actualizada correctamente";
        } catch (IllegalArgumentException e) {
            throw crearFaultValidacion(e.getMessage());
        }
    }
   
    @WebMethod(operationName = "convertirLongitud")
    public double convertirLongitud(
            @WebParam(name = "valor") double valor,
            @WebParam(name = "unidadInicial") String unidadInicial,
            @WebParam(name = "unidadFinal") String unidadFinal) {

        try {
            UnidadLongitud origen = UnidadMapper.toLongitud(unidadInicial);
            UnidadLongitud destino = UnidadMapper.toLongitud(unidadFinal);

            return servicioLongitud.convertir(valor, origen, destino);
        } catch (IllegalArgumentException e) {
            throw crearFaultValidacion(e.getMessage());
        }
    }
    
    @WebMethod(operationName = "convertirMasa")
    public double convertirMasa(
            @WebParam(name = "valor") double valor,
            @WebParam(name = "unidadInicial") String unidadInicial,
            @WebParam(name = "unidadFinal") String unidadFinal) {

        try {
            UnidadMasa origen = UnidadMapper.toMasa(unidadInicial);
            UnidadMasa destino = UnidadMapper.toMasa(unidadFinal);

            return servicioMasa.convertir(valor, origen, destino);
        } catch (IllegalArgumentException e) {
            throw crearFaultValidacion(e.getMessage());
        }
    }

    @WebMethod(operationName = "convertirTemperatura")
    public double convertirTemperatura(
            @WebParam(name = "valor") double valor,
            @WebParam(name = "unidadInicial") String unidadInicial,
            @WebParam(name = "unidadFinal") String unidadFinal) {

        try {
            UnidadTemperatura origen = UnidadMapper.toTemperatura(unidadInicial);
            UnidadTemperatura destino = UnidadMapper.toTemperatura(unidadFinal);

            return servicioTemperatura.convertir(valor, origen, destino);
        } catch (IllegalArgumentException e) {
            throw crearFaultValidacion(e.getMessage());
        }
    }

    private SOAPFaultException crearFaultValidacion(String mensaje) {
        try {
            SOAPFault fault = SOAPFactory.newInstance(SOAPConstants.SOAP_1_1_PROTOCOL).createFault();
            fault.setFaultCode(new QName(SOAPConstants.URI_NS_SOAP_ENVELOPE, "Client"));
            fault.setFaultString(mensaje);
            return new SOAPFaultException(fault);
        } catch (SOAPException e) {
            throw new RuntimeException("Error de validación", e);
        }
    }
}
