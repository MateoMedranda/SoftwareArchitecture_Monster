package ec.edu.monster.servicios;

import ec.edu.monster.configuracion.SoapConfig;
import ec.edu.monster.seguridad.AdministradorTokensCliente;
import ec.edu.monster.seguridad.ManejadorClienteAutenticacion;
import ec.edu.monster.seguridad.TlsInseguro;
import jakarta.xml.ws.BindingProvider;
import jakarta.xml.ws.Service;
import java.util.ArrayList;
import java.util.Map;
import javax.xml.namespace.QName;

/** Capa de acceso al endpoint SOAP. Mantiene el token sólo durante la sesión actual. */
public class WSConversorCliente {
    private final WSConversorUnidades puerto;

    public WSConversorCliente() {
        if (SoapConfig.insecureTls()) TlsInseguro.habilitar();
        String namespace = SoapConfig.targetNamespace();
        Service servicio = Service.create(SoapConfig.wsdl(), new QName(namespace, "WSConversorUnidades"));
        puerto = servicio.getPort(new QName(namespace, "WSConversorUnidadesPort"), WSConversorUnidades.class);
        configurarPuerto();
    }

    private void configurarPuerto() {
        BindingProvider binding = (BindingProvider) puerto;
        Map<String, Object> contexto = binding.getRequestContext();
        contexto.put(BindingProvider.ENDPOINT_ADDRESS_PROPERTY, SoapConfig.endpoint());
        contexto.put("jakarta.xml.ws.client.connectionTimeout", SoapConfig.connectTimeout());
        contexto.put("jakarta.xml.ws.client.receiveTimeout", SoapConfig.readTimeout());

        var manejadores = new ArrayList<>(binding.getBinding().getHandlerChain());
        manejadores.add(new ManejadorClienteAutenticacion());
        binding.getBinding().setHandlerChain(manejadores);
    }

    public boolean login(String usuario, String contrasenia) {
        try {
            String token = puerto.login(usuario, contrasenia);
            if (token == null || token.isBlank()) return false;
            AdministradorTokensCliente.guardarToken(token);
            return true;
        } catch (RuntimeException ex) {
            AdministradorTokensCliente.limpiar();
            return false;
        }
    }

    public String cambiarContrasenia(String actual, String nueva) {
        return puerto.cambiarContrasenia(actual, nueva);
    }

    public double convertirTemperatura(double valor, String origen, String destino) {
        return puerto.convertirTemperatura(valor, origen, destino);
    }

    public double convertirMasa(double valor, String origen, String destino) {
        return puerto.convertirMasa(valor, origen, destino);
    }

    public double convertirLongitud(double valor, String origen, String destino) {
        return puerto.convertirLongitud(valor, origen, destino);
    }

    public void cerrarSesion() { AdministradorTokensCliente.limpiar(); }
}
