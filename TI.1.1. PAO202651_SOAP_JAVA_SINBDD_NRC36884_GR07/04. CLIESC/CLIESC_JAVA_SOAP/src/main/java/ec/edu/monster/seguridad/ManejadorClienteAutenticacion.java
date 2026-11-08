package ec.edu.monster.seguridad;

import jakarta.xml.soap.Node;
import jakarta.xml.soap.SOAPElement;
import jakarta.xml.soap.SOAPEnvelope;
import jakarta.xml.soap.SOAPException;
import jakarta.xml.soap.SOAPHeader;
import jakarta.xml.soap.SOAPMessage;
import jakarta.xml.ws.handler.MessageContext;
import jakarta.xml.ws.handler.soap.SOAPHandler;
import jakarta.xml.ws.handler.soap.SOAPMessageContext;
import java.util.Collections;
import java.util.Set;
import javax.xml.namespace.QName;

public class ManejadorClienteAutenticacion implements SOAPHandler<SOAPMessageContext> {

    private static final String NS_SEGURIDAD = "http://ws.monster.edu.ec/seguridad";
    private static final String PREFIJO_SEGURIDAD = "seg";
    private static final String ELEMENTO_TOKEN = "token";
    private static final String OPERACION_LOGIN = "login";

    @Override
    public Set<QName> getHeaders() {
        return Collections.singleton(new QName(NS_SEGURIDAD, ELEMENTO_TOKEN));
    }

    @Override
    public boolean handleMessage(SOAPMessageContext contexto) {
        Boolean saliente = (Boolean) contexto.get(MessageContext.MESSAGE_OUTBOUND_PROPERTY);
        if (Boolean.TRUE.equals(saliente)) {
            agregarToken(contexto);
        }
        return true;
    }

    private void agregarToken(SOAPMessageContext contexto) {
        String token = AdministradorTokensCliente.obtenerToken();
        if (token == null || token.isEmpty()) {
            return;
        }
        try {
            SOAPMessage mensaje = contexto.getMessage();
            if (OPERACION_LOGIN.equalsIgnoreCase(obtenerOperacion(mensaje))) {
                return;
            }
            SOAPEnvelope sobre = mensaje.getSOAPPart().getEnvelope();
            SOAPHeader cabecera = sobre.getHeader();
            if (cabecera == null) {
                cabecera = sobre.addHeader();
            }
            eliminarTokensPrevios(cabecera);
            SOAPElement elemento = cabecera.addChildElement(ELEMENTO_TOKEN, PREFIJO_SEGURIDAD, NS_SEGURIDAD);
            elemento.addTextNode(token);
            mensaje.saveChanges();
        } catch (SOAPException ex) {
            throw new RuntimeException("No fue posible agregar el token a la peticion SOAP", ex);
        }
    }

    private void eliminarTokensPrevios(SOAPHeader cabecera) {
        java.util.List<SOAPElement> tokens = new java.util.ArrayList<>();
        java.util.Iterator<?> hijos = cabecera.getChildElements();
        while (hijos.hasNext()) {
            Object hijo = hijos.next();
            if (hijo instanceof SOAPElement) {
                SOAPElement elemento = (SOAPElement) hijo;
                if (ELEMENTO_TOKEN.equals(elemento.getLocalName())
                        && NS_SEGURIDAD.equals(elemento.getNamespaceURI())) {
                    tokens.add(elemento);
                }
            }
        }
        for (SOAPElement token : tokens) {
            token.detachNode();
        }
    }

    private String obtenerOperacion(SOAPMessage mensaje) throws SOAPException {
        Node cuerpo = (Node) mensaje.getSOAPBody().getFirstChild();
        return (cuerpo instanceof SOAPElement) ? cuerpo.getLocalName() : null;
    }

    @Override
    public boolean handleFault(SOAPMessageContext contexto) {
        return true;
    }

    @Override
    public void close(MessageContext contexto) {
    }
}
