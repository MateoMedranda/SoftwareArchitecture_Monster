package ec.edu.monster.controladores.manejadoresws;

import ec.edu.monster.seguridad.AdministradorTokens;
import jakarta.xml.ws.handler.soap.SOAPHandler;
import jakarta.xml.ws.handler.soap.SOAPMessageContext;
import jakarta.xml.ws.handler.MessageContext;
import jakarta.xml.ws.soap.SOAPFaultException;

import jakarta.xml.soap.SOAPBody;
import jakarta.xml.soap.SOAPConstants;
import jakarta.xml.soap.SOAPElement;
import jakarta.xml.soap.SOAPException;
import jakarta.xml.soap.SOAPFault;
import jakarta.xml.soap.SOAPHeader;
import jakarta.xml.soap.SOAPMessage;
import jakarta.xml.soap.SOAPFactory;

import javax.xml.namespace.QName;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import java.util.Collections;
import java.util.Set;

public class ManejadorAutenticacion implements SOAPHandler<SOAPMessageContext> {

    private static final QName FAULT_CODE_CLIENT =
            new QName(SOAPConstants.URI_NS_SOAP_ENVELOPE, "Client");

    @Override
    public boolean handleMessage(SOAPMessageContext contexto) {

        Boolean esSalida = (Boolean) contexto.get(MessageContext.MESSAGE_OUTBOUND_PROPERTY);

        if (Boolean.TRUE.equals(esSalida)) {
            return true;
        }

        try {

            SOAPMessage mensajeSOAP = contexto.getMessage();
            SOAPBody cuerpo = mensajeSOAP.getSOAPBody();

            if ("login".equals(nombrePrimerElemento(cuerpo))) {
                return true;
            }

            SOAPHeader encabezado = mensajeSOAP.getSOAPHeader();

            if (encabezado == null) {
                throw fault("Acceso denegado: Se requiere encabezado SOAP con el token de seguridad.");
            }

            NodeList nodosToken = encabezado.getElementsByTagNameNS("*", "token");

            if (nodosToken.getLength() == 0) {
                throw fault("Acceso denegado: No se encontró el token. Inicie sesión primero.");
            }

            String token = nodosToken.item(0).getTextContent();

            if (token == null || !AdministradorTokens.validarToken(token.trim())) {
                throw fault("Acceso denegado: Token inválido o expirado.");
            }

        } catch (SOAPFaultException e) {
            throw e;
        } catch (Exception e) {
            String mensaje = e.getMessage() == null ? e.getClass().getSimpleName() : e.getMessage();
            throw fault(mensaje);
        }
        return true;
    }

    private static String nombrePrimerElemento(SOAPElement padre) {
        NodeList hijos = padre.getChildNodes();
        for (int i = 0; i < hijos.getLength(); i++) {
            Node hijo = hijos.item(i);
            if (hijo.getNodeType() == Node.ELEMENT_NODE) {
                return hijo.getLocalName();
            }
        }
        return null;
    }

    private static SOAPFaultException fault(String mensaje) {
        try {
            SOAPFault falla = SOAPFactory.newInstance(SOAPConstants.SOAP_1_1_PROTOCOL).createFault();
            falla.setFaultCode(FAULT_CODE_CLIENT);
            falla.setFaultString(mensaje);
            return new SOAPFaultException(falla);
        } catch (SOAPException e) {
            throw new RuntimeException(mensaje, e);
        }
    }

    @Override
    public Set<QName> getHeaders() {
        return Collections.emptySet();
    }

    @Override
    public boolean handleFault(SOAPMessageContext contexto) {
        return true;
    }

    @Override
    public void close(MessageContext contexto) {}
}
