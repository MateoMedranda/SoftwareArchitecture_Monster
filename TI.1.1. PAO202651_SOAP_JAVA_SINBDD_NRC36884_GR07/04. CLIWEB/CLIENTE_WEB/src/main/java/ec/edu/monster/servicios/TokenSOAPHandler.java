package ec.edu.monster.servicios;

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

/**
 * Handler JAX-WS que inyecta el token de sesion como header SOAP en cada
 * mensaje saliente.
 *
 * Formato generado:
 *   &lt;mon:token xmlns:mon="http://ws.monster.edu.ec/token"&gt;VALOR&lt;/mon:token&gt;
 *
 */
public class TokenSOAPHandler implements SOAPHandler<SOAPMessageContext> {

    public static final String TOKEN_NAMESPACE = "http://ws.monster.edu.ec/token";
    public static final String TOKEN_PREFIX = "mon";
    public static final String TOKEN_ELEMENT = "token";

    private final String token;

    public TokenSOAPHandler(String token) {
        this.token = token;
    }

    @Override
    public boolean handleMessage(SOAPMessageContext context) {
        Boolean outbound = (Boolean) context.get(MessageContext.MESSAGE_OUTBOUND_PROPERTY);
        if (Boolean.TRUE.equals(outbound) && token != null && !token.isBlank()) {
            try {
                SOAPMessage message = context.getMessage();
                SOAPEnvelope envelope = message.getSOAPPart().getEnvelope();
                SOAPHeader header = envelope.getHeader();
                if (header == null) {
                    header = envelope.addHeader();
                }
                SOAPElement tokenElement =
                        header.addChildElement(TOKEN_ELEMENT, TOKEN_PREFIX, TOKEN_NAMESPACE);
                tokenElement.addTextNode(token);
                message.saveChanges();
            } catch (SOAPException e) {
                throw new RuntimeException("No se pudo agregar el token al header SOAP", e);
            }
        }
        return true;
    }

    @Override
    public boolean handleFault(SOAPMessageContext context) {
        return true;
    }

    @Override
    public void close(MessageContext context) {
        // Sin recursos que liberar.
    }

    @Override
    public Set<QName> getHeaders() {
        return Collections.emptySet();
    }
}
