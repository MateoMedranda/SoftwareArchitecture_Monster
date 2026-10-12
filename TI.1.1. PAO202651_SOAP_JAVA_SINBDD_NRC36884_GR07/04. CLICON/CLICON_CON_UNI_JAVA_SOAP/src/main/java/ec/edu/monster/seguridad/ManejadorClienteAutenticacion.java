package ec.edu.monster.seguridad;

import jakarta.xml.soap.Node;
import jakarta.xml.soap.SOAPElement;
import jakarta.xml.soap.SOAPEnvelope;
import jakarta.xml.soap.SOAPHeader;
import jakarta.xml.soap.SOAPMessage;
import jakarta.xml.ws.handler.MessageContext;
import jakarta.xml.ws.handler.soap.SOAPHandler;
import jakarta.xml.ws.handler.soap.SOAPMessageContext;
import java.util.Collections;
import java.util.Set;
import javax.xml.namespace.QName;

/** Inserta el token que exige el manejador de autenticación del servidor. */
public final class ManejadorClienteAutenticacion implements SOAPHandler<SOAPMessageContext> {
    private static final String NS_SEGURIDAD = "http://ws.monster.edu.ec/seguridad";

    @Override
    public boolean handleMessage(SOAPMessageContext contexto) {
        if (Boolean.TRUE.equals(contexto.get(MessageContext.MESSAGE_OUTBOUND_PROPERTY))) {
            agregarToken(contexto);
        }
        return true;
    }

    private void agregarToken(SOAPMessageContext contexto) {
        String token = AdministradorTokensCliente.obtenerToken();
        if (token == null || token.isBlank()) return;
        try {
            SOAPMessage mensaje = contexto.getMessage();
            Node cuerpo = (Node) mensaje.getSOAPBody().getFirstChild();
            if (cuerpo instanceof SOAPElement elemento && "login".equals(elemento.getLocalName())) return;
            SOAPEnvelope sobre = mensaje.getSOAPPart().getEnvelope();
            SOAPHeader cabecera = sobre.getHeader() == null ? sobre.addHeader() : sobre.getHeader();
            cabecera.addChildElement("token", "seg", NS_SEGURIDAD).addTextNode(token);
            mensaje.saveChanges();
        } catch (Exception ex) {
            throw new IllegalStateException("No fue posible agregar el token SOAP", ex);
        }
    }

    @Override public Set<QName> getHeaders() { return Collections.singleton(new QName(NS_SEGURIDAD, "token")); }
    @Override public boolean handleFault(SOAPMessageContext contexto) { return true; }
    @Override public void close(MessageContext contexto) { }
}
