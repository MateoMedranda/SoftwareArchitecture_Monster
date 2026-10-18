/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.servicios;

import jakarta.xml.ws.BindingProvider;
import jakarta.xml.ws.WebServiceException;
import jakarta.xml.ws.handler.Handler;

import ec.edu.monster.controladores.config.SoapConfig;
import ec.edu.monster.seguridad.AdministradorTokensCliente;
import ec.edu.monster.seguridad.ManejadorClienteAutenticacion;
import ec.edu.monster.seguridad.TlsInseguro;
import java.net.URL;
import java.util.List;
import java.util.Map;
/**
 *
 * @author gmlop
 */
public class WSConversorCliente {
    private final WSConversorUnidades puerto;
    
    public WSConversorCliente() throws Exception {

        if (SoapConfig.insecureTls()) {
            TlsInseguro.habilitar();
        }

        URL wsdlUrl = SoapConfig.wsdlLocal();
        WSConversorUnidades_Service service = new WSConversorUnidades_Service(wsdlUrl);
        this.puerto = service.getWSConversorUnidadesPort();

        configurarBindingProvider();
    }
    
    private void configurarBindingProvider() {
        BindingProvider bp = (BindingProvider) puerto;
        Map<String, Object> ctx = bp.getRequestContext();

        ctx.put(BindingProvider.ENDPOINT_ADDRESS_PROPERTY, SoapConfig.endpoint());

        // Timeouts: usa las claves de Jakarta para compatibilidad general
        // Si usas Metro, también puedes usar com.sun.xml.ws.developer.JAXWSProperties
        ctx.put("jakarta.xml.ws.client.connectionTimeout", SoapConfig.connectTimeout());
        ctx.put("jakarta.xml.ws.client.receiveTimeout",    SoapConfig.readTimeout());

        List<Handler> cadenaManejadores = bp.getBinding().getHandlerChain();
        cadenaManejadores.add(new ManejadorClienteAutenticacion());
        bp.getBinding().setHandlerChain(cadenaManejadores);
    }
    
    public boolean login(String usuario, String contrasenia) {
        try {
            String token = puerto.login(usuario, contrasenia);
            if (token == null || token.isBlank()) {
                return false;
            }
            AdministradorTokensCliente.guardarToken(token);
            return true;
        } catch (WebServiceException e) {
            System.err.println("[SOAP] login falló: " + e.getMessage());
            return false;
        }
    }

    public boolean cambiarContrasenia(String usuario, String actual, String nueva) {
        try {
            return !puerto.cambiarContrasenia(actual, nueva).isBlank();
        } catch (WebServiceException e) {
            System.err.println("[SOAP] cambiarContrasenia falló: " + e.getMessage());
            return false;
        }
    }

    public double convertirTemperatura(double valor, String origen, String destino) {
        try {
            return puerto.convertirTemperatura(valor, origen, destino);
        } catch (WebServiceException e) {
            throw new RuntimeException("Error al convertir temperatura: " + e.getMessage(), e);
        }
    }

    public double convertirLongitud(double valor, String origen, String destino) {
        try {
            return puerto.convertirLongitud(valor, origen, destino);
        } catch (WebServiceException e) {
            throw new RuntimeException("Error al convertir longitud: " + e.getMessage(), e);
        }
    }

    public double convertirMasa(double valor, String origen, String destino) {
        try {
            return puerto.convertirMasa(valor, origen, destino);
        } catch (WebServiceException e) {
            throw new RuntimeException("Error al convertir masa: " + e.getMessage(), e);
        }
    }

}
