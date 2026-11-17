package ec.edu.monster.recursos;

/**
 * Cuerpo JSON de POST /api/auth/login.
 */
public class SolicitudLogin {

    private String usuario;
    private String contrasenia;

    public String getUsuario() {
        return usuario;
    }

    public void setUsuario(String v) {
        usuario = v;
    }

    public String getContrasenia() {
        return contrasenia;
    }

    public void setContrasenia(String v) {
        contrasenia = v;
    }
}
