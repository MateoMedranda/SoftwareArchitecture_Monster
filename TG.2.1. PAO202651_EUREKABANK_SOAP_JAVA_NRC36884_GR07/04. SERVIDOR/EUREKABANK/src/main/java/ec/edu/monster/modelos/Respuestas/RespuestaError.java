package ec.edu.monster.modelos.Respuestas;


import jakarta.xml.bind.annotation.XmlAccessType;
import jakarta.xml.bind.annotation.XmlAccessorType;
import jakarta.xml.bind.annotation.XmlRootElement;

/**
 * DTO de respuesta por error
 *
 * @author MONSTER GR07
 */
@XmlRootElement(name = "respuestaError")
@XmlAccessorType(XmlAccessType.FIELD)
public class RespuestaError extends Exception{

    /** Código de error técnico (constante identificadora) */
    private String codigo;

    /** Mensaje amigable para el usuario final */
    private String mensaje;

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
