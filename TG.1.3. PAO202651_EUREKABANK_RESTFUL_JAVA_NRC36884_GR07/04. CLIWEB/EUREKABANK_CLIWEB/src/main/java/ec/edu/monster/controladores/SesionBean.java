package ec.edu.monster.controladores;

import ec.edu.monster.servicios.ClienteRestEureka;
import jakarta.enterprise.context.SessionScoped;
import jakarta.inject.Named;
import java.io.Serializable;

/**
 * Controlador MVC para autenticación y estado de sesión web.
 */
@Named
@SessionScoped
public class SesionBean implements Serializable {

    private transient ClienteRestEureka rest = new ClienteRestEureka();
    private String usuario, contrasenia, token, usuarioId, nombre, rol, mensaje;

    public String iniciar() {
        try {
            var r = cliente().login(usuario, contrasenia);
            token = r.token();
            usuarioId = r.usuarioId();
            nombre = r.nombre();
            rol = r.rol();
            mensaje = null;
            return "/panel.xhtml?faces-redirect=true";
        } catch (Exception e) {
            mensaje = limpiar(e.getMessage());
            return null;
        }
    }

    public String salir() {
        token = null;
        usuarioId = null;
        rol = null;
        return "/login.xhtml?faces-redirect=true";
    }

    public boolean getActiva() {
        return token != null && !token.isBlank();
    }

    public boolean getCliente() {
        return "CLIENTE".equalsIgnoreCase(rol);
    }

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

    public String getToken() {
        return token;
    }

    public String getUsuarioId() {
        return usuarioId;
    }

    public String getNombre() {
        return nombre;
    }

    public String getMensaje() {
        return mensaje;
    }

    private ClienteRestEureka cliente() {
        if (rest == null) {
            rest = new ClienteRestEureka();
        }
        return rest;
    }

    private String limpiar(String x) {
        return x == null ? "No fue posible conectar con el servicio." : x.trim();
    }
}
