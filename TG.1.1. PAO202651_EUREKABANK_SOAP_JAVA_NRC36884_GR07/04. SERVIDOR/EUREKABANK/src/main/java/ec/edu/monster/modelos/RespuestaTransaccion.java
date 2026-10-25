package ec.edu.monster.modelos;

/**
 * DTO de respuesta exitosa del servicio SOAP de login.
 *
 * @author MONSTER GR07
 */
public class RespuestaTransaccion {

    /** Indica si la operación fue exitosa */
    private boolean exitoso;

    /** Token JWT generado (header.payload.signature) — válido por 8 horas */
    private String mensaje;

    /** ID del usuario autenticado (UUID de la tabla USUARIO) */
    private String usuarioId;

    
    private String tipo;

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores
    // ─────────────────────────────────────────────────────────────────────────

    public RespuestaTransaccion() {}

    public RespuestaTransaccion(boolean exitoso, String mensaje, String usuarioId, String tipo) {
        this.exitoso = exitoso;
        this.mensaje = mensaje;
        this.usuarioId = usuarioId;
        this.tipo = tipo;
    }

    public boolean isExitoso() {
        return exitoso;
    }

    public void setExitoso(boolean exitoso) {
        this.exitoso = exitoso;
    }

    public String getMensaje() {
        return mensaje;
    }

    public void setMensaje(String mensaje) {
        this.mensaje = mensaje;
    }

    public String getUsuarioId() {
        return usuarioId;
    }

    public void setUsuarioId(String usuarioId) {
        this.usuarioId = usuarioId;
    }

    public String getTipo() {
        return tipo;
    }

    public void setTipo(String tipo) {
        this.tipo = tipo;
    }
        
    @Override
    public String toString() {
        return "RespuestaTransaccion{exitoso=" + exitoso + ", usuarioId='" + usuarioId + "', "
                + "mensaje='" + mensaje + "'}";
    }
}
