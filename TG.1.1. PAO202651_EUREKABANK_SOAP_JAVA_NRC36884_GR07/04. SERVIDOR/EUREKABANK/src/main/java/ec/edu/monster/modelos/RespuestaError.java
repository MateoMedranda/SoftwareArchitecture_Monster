package ec.edu.monster.modelos;

/**
 * DTO de error para el servicio SOAP.
 *
 * Se usa como faultBean en {@link LoginExcepcion} (@WebFault).
 * JAXB lo serializa como elemento &lt;detail&gt; dentro del SOAPFault,
 * permitiendo mensajes de error estructurados y amigables.
 *
 * Códigos de error definidos:
 *   CAMPO_REQUERIDO       — falta un campo obligatorio en la petición
 *   USUARIO_NO_ENCONTRADO — el nombre de usuario no existe en la BDD
 *   CREDENCIALES_INVALIDAS — contraseña incorrecta
 *   TOKEN_REQUERIDO       — se invocó una operación protegida sin token
 *   TOKEN_INVALIDO        — el token es inválido, expirado o fue revocado
 *   ACCESO_DENEGADO       — el token no tiene el rol requerido
 *   LOGOUT_FALLIDO        — no se pudo revocar el token
 *   ERROR_BASE_DATOS      — fallo en la conexión o consulta a la BDD
 *   ERROR_CIFRADO         — error interno al procesar la contraseña
 *
 * @author MONSTER GR07
 */
public class RespuestaError {

    /** Código de error técnico (constante identificadora) */
    private String codigo;

    /** Mensaje amigable para el usuario final */
    private String mensaje;

    /**
     * Detalle técnico adicional (puede ser null).
     * Útil para diagnóstico en desarrollo; omitir en producción si es sensible.
     */
    private String detalle;

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores
    // ─────────────────────────────────────────────────────────────────────────

    public RespuestaError() {}

    public RespuestaError(String codigo, String mensaje) {
        this.codigo  = codigo;
        this.mensaje = mensaje;
        this.detalle = null;
    }

    public RespuestaError(String codigo, String mensaje, String detalle) {
        this.codigo  = codigo;
        this.mensaje = mensaje;
        this.detalle = detalle;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Getters / Setters
    // ─────────────────────────────────────────────────────────────────────────

    public String getCodigo() { return codigo; }
    public void setCodigo(String codigo) { this.codigo = codigo; }

    public String getMensaje() { return mensaje; }
    public void setMensaje(String mensaje) { this.mensaje = mensaje; }

    public String getDetalle() { return detalle; }
    public void setDetalle(String detalle) { this.detalle = detalle; }

    @Override
    public String toString() {
        return "RespuestaError{codigo='" + codigo + "', mensaje='" + mensaje + "'}";
    }
}
