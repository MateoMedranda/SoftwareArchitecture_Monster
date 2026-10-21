package ec.edu.monster.controladores;

import ec.edu.monster.modelos.Credenciales;
import ec.edu.monster.modelos.SesionBean;
import ec.edu.monster.servicios.ApiRestException;
import ec.edu.monster.servicios.AutenticacionServicio;
import jakarta.faces.application.FacesMessage;
import jakarta.faces.context.FacesContext;
import jakarta.faces.view.ViewScoped;
import jakarta.inject.Inject;
import jakarta.inject.Named;
import java.io.Serializable;

@Named("loginBean")
@ViewScoped
public class ControladorLoginBean implements Serializable {

    private static final long serialVersionUID = 1L;

    private final Credenciales credenciales = new Credenciales();

    @Inject
    private AutenticacionServicio autenticacionServicio;

    @Inject
    private SesionBean sesionBean;

    public String iniciarSesion() {
        String usuario = credenciales.getUsuario();
        String contrasenia = credenciales.getContrasenia();
        if (usuario == null || usuario.isBlank() || contrasenia == null || contrasenia.isBlank()) {
            mensaje(FacesMessage.SEVERITY_WARN, "Debe ingresar usuario y contraseña.");
            return null;
        }

        try {
            String token = autenticacionServicio.login(usuario.trim(), contrasenia);
            sesionBean.iniciarSesion(usuario.trim(), token);
            return "conversor?faces-redirect=true";
        } catch (ApiRestException e) {
            mensaje(FacesMessage.SEVERITY_ERROR, e.getMessage());
            return null;
        }
    }

    private void mensaje(FacesMessage.Severity severidad, String detalle) {
        FacesContext.getCurrentInstance().addMessage(null,
                new FacesMessage(severidad, "", detalle));
    }

    public Credenciales getCredenciales() {
        return credenciales;
    }
}
