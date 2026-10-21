package ec.edu.monster.modelos;

import jakarta.enterprise.context.SessionScoped;
import jakarta.inject.Named;
import java.io.Serializable;

@Named("sesionBean")
@SessionScoped
public class SesionBean implements Serializable {

    private static final long serialVersionUID = 1L;

    private String usuarioAutenticado;
    private String token;

    public boolean estaAutenticado() {
        return token != null && !token.isBlank();
    }

    public void iniciarSesion(String usuario, String token) {
        this.usuarioAutenticado = usuario;
        this.token = token;
    }

    public void cerrarSesion() {
        usuarioAutenticado = null;
        token = null;
    }

    public String getUsuarioAutenticado() {
        return usuarioAutenticado;
    }

    public String getToken() {
        return token;
    }
}
