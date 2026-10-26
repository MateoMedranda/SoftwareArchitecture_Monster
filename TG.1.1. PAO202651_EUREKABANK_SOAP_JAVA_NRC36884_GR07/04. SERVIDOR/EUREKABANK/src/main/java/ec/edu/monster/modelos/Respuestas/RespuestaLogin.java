package ec.edu.monster.modelos.Respuestas;

import jakarta.xml.bind.annotation.XmlAccessType;
import jakarta.xml.bind.annotation.XmlAccessorType;
import jakarta.xml.bind.annotation.XmlRootElement;

/**
 * DTO de respuesta exitosa del servicio SOAP de login.
 *
 * @author MONSTER GR07
 */
@XmlRootElement(name = "respuestaLogin")
@XmlAccessorType(XmlAccessType.FIELD)
public class RespuestaLogin {

    /** Indica si la operación fue exitosa */
    private boolean exitoso;

    /** Token JWT generado (header.payload.signature) — válido por 8 horas */
    private String token;

    /** ID del usuario autenticado (UUID de la tabla USUARIO) */
    private String usuarioId;

    /** ID del rol del usuario (UUID de la tabla ROL) */
    private String rolId;

    /** Nombre completo del usuario (USUARIO_NOMBRE + USUARIO_APELLIDO) */
    private String usuarioNombre;

    /** Mensaje descriptivo del resultado */
    private String mensaje;

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores
    // ─────────────────────────────────────────────────────────────────────────

    public RespuestaLogin() {}

    public RespuestaLogin(boolean exitoso, String token, String usuarioId,
                          String rolId, String usuarioNombre, String mensaje) {
        this.exitoso       = exitoso;
        this.token         = token;
        this.usuarioId     = usuarioId;
        this.rolId         = rolId;
        this.usuarioNombre = usuarioNombre;
        this.mensaje       = mensaje;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Getters / Setters
    // ─────────────────────────────────────────────────────────────────────────

    public boolean isExitoso() { return exitoso; }
    public void setExitoso(boolean exitoso) { this.exitoso = exitoso; }

    public String getToken() { return token; }
    public void setToken(String token) { this.token = token; }

    public String getUsuarioId() { return usuarioId; }
    public void setUsuarioId(String usuarioId) { this.usuarioId = usuarioId; }

    public String getRolId() { return rolId; }
    public void setRolId(String rolId) { this.rolId = rolId; }

    public String getUsuarioNombre() { return usuarioNombre; }
    public void setUsuarioNombre(String usuarioNombre) { this.usuarioNombre = usuarioNombre; }

    public String getMensaje() { return mensaje; }
    public void setMensaje(String mensaje) { this.mensaje = mensaje; }

    @Override
    public String toString() {
        return "RespuestaLogin{exitoso=" + exitoso + ", usuarioId='" + usuarioId + "', mensaje='" + mensaje + "'}";
    }
}
