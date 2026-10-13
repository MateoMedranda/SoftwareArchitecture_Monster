package ec.edu.monster.servicios;

import ec.edu.monster.resources.WSConversorUnidades;
import ec.edu.monster.resources.WSConversorUnidades_Service;
import jakarta.xml.ws.BindingProvider;
import jakarta.xml.ws.handler.Handler;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;

/**
 * Fabrica centralizada del puerto SOAP. El SSL ya queda configurado
 * globalmente por ec.edu.monster.util.SSLConfigListener.
 *
 * El WSDL se carga desde el classpath (WEB-INF/classes/WSConversorUnidades.wsdl)
 * para evitar rutas hardcodeadas que fallan en Docker/Payara.
 *
 * El endpoint se resuelve en este orden:
 * 1. System property "monster.soap.endpoint"
 * 2. Environment variable "MONSTER_SOAP_ENDPOINT"
 * 3. Default del WSDL (localhost:8181)
 */
public final class ClienteSOAPFactory {

    public static final String ENDPOINT_PROPERTY = "monster.soap.endpoint";
    public static final String ENDPOINT_ENV = "MONSTER_SOAP_ENDPOINT";

    private ClienteSOAPFactory() {
    }

    private static URL getWsdlUrl() {
        URL wsdl = ClienteSOAPFactory.class.getResource("/WSConversorUnidades.wsdl");
        if (wsdl == null) {
            throw new IllegalStateException(
                    "No se encontro WSConversorUnidades.wsdl en el classpath. "
                    + "Verifique que el recurso este empaquetado en WEB-INF/classes/");
        }
        return wsdl;
    }

    private static String resolveEndpoint() {
        String prop = System.getProperty(ENDPOINT_PROPERTY);
        if (prop != null && !prop.isBlank()) {
            return prop.trim();
        }
        String env = System.getenv(ENDPOINT_ENV);
        if (env != null && !env.isBlank()) {
            return env.trim();
        }
        return null;
    }

    /**
     * Crea el puerto SOAP. Si el token no es nulo, adjunta el
     * {@link TokenSOAPHandler} para enviarlo en el header SOAP.
     */
    public static WSConversorUnidades crearPuerto(String token) {
        URL wsdlUrl = getWsdlUrl();
        WSConversorUnidades port = new WSConversorUnidades_Service(wsdlUrl).getWSConversorUnidadesPort();
        String endpoint = resolveEndpoint();
        if (endpoint != null) {
            ((BindingProvider) port).getRequestContext()
                    .put(BindingProvider.ENDPOINT_ADDRESS_PROPERTY, endpoint);
        }
        if (token != null && !token.isBlank()) {
            BindingProvider bp = (BindingProvider) port;
            List<Handler> chain = new ArrayList<>();
            chain.add(new TokenSOAPHandler(token));
            bp.getBinding().setHandlerChain(chain);
        }
        return port;
    }

    /**
     * Variante que ademas sobreescribe el endpoint (util para apuntar a IP
     * interna o externa manteniendo el SSL por dominio).
     */
    public static WSConversorUnidades crearPuertoConEndpoint(String token, String endpoint) {
        URL wsdlUrl = getWsdlUrl();
        WSConversorUnidades port = new WSConversorUnidades_Service(wsdlUrl).getWSConversorUnidadesPort();
        String finalEndpoint = endpoint != null && !endpoint.isBlank() ? endpoint.trim() : resolveEndpoint();
        if (finalEndpoint != null) {
            ((BindingProvider) port).getRequestContext()
                    .put(BindingProvider.ENDPOINT_ADDRESS_PROPERTY, finalEndpoint);
        }
        if (token != null && !token.isBlank()) {
            BindingProvider bp = (BindingProvider) port;
            List<Handler> chain = new ArrayList<>();
            chain.add(new TokenSOAPHandler(token));
            bp.getBinding().setHandlerChain(chain);
        }
        return port;
    }
}
