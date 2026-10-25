package ec.edu.monster.servicios;

import ec.edu.monster.modelos.RespuestaError;
import jakarta.xml.ws.WebFault;

/**
 * Excepción SOAP personalizada para errores del servicio de login.
 *
 * Anotada con @WebFault para que JAX-WS la serialice correctamente
 * como un &lt;Fault&gt; en el mensaje SOAP de respuesta, incluyendo
 * el detalle de error estructurado (faultBean = RespuestaError).
 *
 * Uso en servicios SOAP:
 * <pre>
 *   throw new LoginExcepcion(
 *       "Mensaje técnico (log interno)",
 *       new RespuestaError("CODIGO", "Mensaje amigable", "detalle opcional")
 *   );
 * </pre>
 *
 * En SoapUI, la respuesta de error se verá como:
 * <pre>
 *   &lt;S:Fault&gt;
 *     &lt;faultcode&gt;S:Server&lt;/faultcode&gt;
 *     &lt;faultstring&gt;Mensaje técnico&lt;/faultstring&gt;
 *     &lt;detail&gt;
 *       &lt;ns2:LoginFault&gt;
 *         &lt;codigo&gt;CODIGO&lt;/codigo&gt;
 *         &lt;mensaje&gt;Mensaje amigable&lt;/mensaje&gt;
 *         &lt;detalle&gt;detalle opcional&lt;/detalle&gt;
 *       &lt;/ns2:LoginFault&gt;
 *     &lt;/detail&gt;
 *   &lt;/S:Fault&gt;
 * </pre>
 *
 * @author MONSTER GR07
 */
@WebFault(
    name        = "LoginFault",
    faultBean   = "ec.edu.monster.servicios.RespuestaError",
    targetNamespace = "http://servicios.monster.edu.ec/"
)
public class LoginExcepcion extends Exception {

    private RespuestaError faultInfo;

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores requeridos por JAX-WS
    // ─────────────────────────────────────────────────────────────────────────

    /** Constructor requerido por JAXB/JAX-WS */
    public LoginExcepcion() {
        super();
    }

    /**
     * @param message   Mensaje técnico (aparece en faultstring del SOAP)
     * @param faultInfo Detalle estructurado del error (aparece en detail del SOAP)
     */
    public LoginExcepcion(String message, RespuestaError faultInfo) {
        super(message);
        this.faultInfo = faultInfo;
    }

    /**
     * @param message   Mensaje técnico
     * @param faultInfo Detalle estructurado del error
     * @param cause     Causa original de la excepción
     */
    public LoginExcepcion(String message, RespuestaError faultInfo, Throwable cause) {
        super(message, cause);
        this.faultInfo = faultInfo;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Getter requerido por JAX-WS para serializar el faultBean
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Retorna el detalle del error. JAX-WS utiliza este método
     * para serializar el faultBean en el elemento &lt;detail&gt; del SOAP Fault.
     */
    public RespuestaError getFaultInfo() {
        return faultInfo;
    }
}
