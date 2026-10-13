package ec.edu.monster.controladores;

import ec.edu.monster.modelos.Credenciales;
import ec.edu.monster.modelos.SesionBean;
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

    /**
     * @return la vista destino o null si permanece en la misma pagina.
     */
    public String iniciarSesion() {
        String usuario = credenciales.getUsuario();
        String contrasenia = credenciales.getContrasenia();

        if (usuario == null || usuario.isBlank() || contrasenia == null || contrasenia.isBlank()) {
            mensaje(FacesMessage.SEVERITY_WARN, "Debe ingresar usuario y contrasenia.");
            return null;
        }

        try {
            String token = autenticacionServicio.login(usuario, contrasenia);
            if (token == null || token.isBlank()) {
                mensaje(FacesMessage.SEVERITY_ERROR, "Credenciales invalidas.");
                return null;
            }
            sesionBean.iniciarSesion(usuario, token);
            return "conversor?faces-redirect=true";
        } catch (Exception e) {
            mensaje(FacesMessage.SEVERITY_ERROR,
                    "Error al conectar con el servidor: " + e.getMessage());
            return null;
        }
    }

    private void mensaje(FacesMessage.Severity severidad, String texto) {
        FacesContext.getCurrentInstance().addMessage(null, new FacesMessage(severidad, texto, texto));
    }

    public Credenciales getCredenciales() {
        return credenciales;
    }
}
