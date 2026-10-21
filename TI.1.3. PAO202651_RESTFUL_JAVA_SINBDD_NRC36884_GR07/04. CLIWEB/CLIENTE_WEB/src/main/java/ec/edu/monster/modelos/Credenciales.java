package ec.edu.monster.modelos;

import java.io.Serializable;

public class Credenciales implements Serializable {

    private static final long serialVersionUID = 1L;

    private String usuario;
    private String contrasenia;

    public String getUsuario() {
        return usuario;
    }

    public void setUsuario(String usuario) {
        this.usuario = usuario;
    }

    public String getContrasenia() {
        return contrasenia;
    }

    public void setContrasenia(String contrasenia) {
        this.contrasenia = contrasenia;
    }
}
