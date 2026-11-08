package ec.edu.monster.modelos;

import java.io.Serializable;

/**
 * Modelo con las credenciales capturadas en el formulario de login.
 *
 * Si se necesitan mas campos (p.ej. un tercer dato de autenticacion),
 * agrega el atributo con getter/setter y referencia {@code credenciales.xxx}
 * en login.xhtml.
 */
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
