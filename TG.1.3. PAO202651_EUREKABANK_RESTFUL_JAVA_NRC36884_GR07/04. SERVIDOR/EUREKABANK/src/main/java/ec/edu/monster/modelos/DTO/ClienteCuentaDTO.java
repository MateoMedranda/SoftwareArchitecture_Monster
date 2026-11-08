package ec.edu.monster.modelos.DTO;

/**
 * Datos mínimos y no sensibles para seleccionar el titular de una cuenta.
 */
public class ClienteCuentaDTO {

    private String usuarioId;
    private String nombreCompleto;
    private String usuario;

    public ClienteCuentaDTO() {
    }

    public ClienteCuentaDTO(String usuarioId, String nombreCompleto, String usuario) {
        this.usuarioId = usuarioId;
        this.nombreCompleto = nombreCompleto;
        this.usuario = usuario;
    }

    public String getUsuarioId() {
        return usuarioId;
    }

    public void setUsuarioId(String v) {
        usuarioId = v;
    }

    public String getNombreCompleto() {
        return nombreCompleto;
    }

    public void setNombreCompleto(String v) {
        nombreCompleto = v;
    }

    public String getUsuario() {
        return usuario;
    }

    public void setUsuario(String v) {
        usuario = v;
    }
}
