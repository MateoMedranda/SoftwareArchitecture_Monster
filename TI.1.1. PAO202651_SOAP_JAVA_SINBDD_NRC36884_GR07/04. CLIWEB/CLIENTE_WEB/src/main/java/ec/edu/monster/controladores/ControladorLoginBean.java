package ec.edu.monster.controladores;

import ec.edu.monster.modelos.Credenciales;
import ec.edu.monster.modelos.SesionBean;
import ec.edu.monster.servicios.AutenticacionServicio;
import jakarta.faces.application.FacesMessage;
import jakarta.faces.context.FacesContext;
import jakarta.faces.view.ViewScoped;
import jakarta.inject.Inject;
import jakarta.inject.Named;
import jakarta.xml.ws.soap.SOAPFaultException;
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
            mensaje(FacesMessage.SEVERITY_WARN, "", "Debe ingresar usuario y contrasenia.");
            return null;
        }

        try {
            String token = autenticacionServicio.login(usuario, contrasenia);
            sesionBean.iniciarSesion(usuario, token);
            return "conversor?faces-redirect=true";
        } catch (SOAPFaultException e) {
            String faultString = e.getFault().getFaultString();
            String friendlyMsg = switch (faultString) {
                case "Usuario o contraseña incorrectos" -> "Usuario o contraseña incorrectos";
                case "Acceso denegado: Se requiere encabezado SOAP con el token de seguridad." ->
                    "Sesión expirada. Inicie sesión nuevamente.";
                case "Acceso denegado: No se encontró el token. Inicie sesión primero." ->
                    "Sesión expirada. Inicie sesión nuevamente.";
                case "Acceso denegado: Token inválido o expirado." ->
                    "Sesión expirada. Inicie sesión nuevamente.";
                default -> "Error de autenticación: " + faultString;
            };
            mensaje(FacesMessage.SEVERITY_ERROR, "", friendlyMsg);
            return null;
        } catch (Exception e) {
            mensaje(FacesMessage.SEVERITY_ERROR, "", "Error de conexión: " + e.getMessage());
            return null;
        }
    }

    private void mensaje(FacesMessage.Severity severidad, String texto, String error) {
        FacesContext.getCurrentInstance().addMessage(null, new FacesMessage(severidad, texto, error));
    }

    public Credenciales getCredenciales() {
        return credenciales;
    }
}
