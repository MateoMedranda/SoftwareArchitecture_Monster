package ec.edu.monster.recursos;

/**
 * Cuerpo JSON para asociar una cuenta a un usuario.
 */
public class SolicitudAsignacion {

    private String cuentaId;
    private String usuarioId;

    public String getCuentaId() {
        return cuentaId;
    }

    public void setCuentaId(String v) {
        cuentaId = v;
    }

    public String getUsuarioId() {
        return usuarioId;
    }

    public void setUsuarioId(String v) {
        usuarioId = v;
    }
}
