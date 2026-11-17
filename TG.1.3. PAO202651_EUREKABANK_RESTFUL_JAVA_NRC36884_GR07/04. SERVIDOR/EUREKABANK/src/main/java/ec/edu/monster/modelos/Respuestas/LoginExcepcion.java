package ec.edu.monster.modelos.Respuestas;

/** Error de autenticación con detalle estructurado para la API REST. */
public class LoginExcepcion extends Exception {

    private RespuestaError faultInfo;

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores requeridos por JAX-WS
    // ─────────────────────────────────────────────────────────────────────────

    public LoginExcepcion() {
        super();
    }

    /**
     * @param message   Mensaje técnico
     * @param faultInfo Detalle estructurado del error
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


    public RespuestaError getFaultInfo() {
        return faultInfo;
    }
}
