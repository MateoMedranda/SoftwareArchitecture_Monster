package ec.edu.monster.modelos;

import jakarta.enterprise.context.SessionScoped;
import jakarta.inject.Named;
import java.io.Serializable;

/**
 * Modelo de sesion. Guarda el token devuelto por el login y el usuario
 * autenticado. El token se reutiliza para las siguientes llamadas SOAP
 * (ver ec.edu.monster.servicio.TokenSOAPHandler).
 *
 * Agregar cualquier dato que deba sobrevivir durante la
 * sesion del usuario (roles, nombre para mostrar, etc.).
 */
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
        this.usuarioAutenticado = null;
        this.token = null;
    }

    public String getUsuarioAutenticado() {
        return usuarioAutenticado;
    }

    public String getToken() {
        return token;
    }
}
