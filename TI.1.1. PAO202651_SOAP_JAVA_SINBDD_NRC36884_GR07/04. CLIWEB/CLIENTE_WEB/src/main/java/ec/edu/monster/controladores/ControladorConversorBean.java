package ec.edu.monster.controladores;

import ec.edu.monster.modelos.SesionBean;
import ec.edu.monster.modelos.enums.UnidadLongitud;
import ec.edu.monster.modelos.enums.UnidadMasa;
import ec.edu.monster.modelos.enums.UnidadTemperatura;
import ec.edu.monster.servicios.ConversorServicio;
import jakarta.annotation.PostConstruct;
import jakarta.faces.application.FacesMessage;
import jakarta.faces.context.FacesContext;
import jakarta.faces.view.ViewScoped;
import jakarta.inject.Inject;
import jakarta.inject.Named;
import java.io.Serializable;

@Named("conversorBean")
@ViewScoped
public class ControladorConversorBean implements Serializable {

    private static final long serialVersionUID = 1L;

    @Inject
    private ConversorServicio conversorServicio;

    @Inject
    private SesionBean sesionBean;

    private double valor;
    private String unidadInicial;
    private String unidadFinal;
    private double resultado;
    private String tipoConversion = "longitud";

    @PostConstruct
    public void init() {
        // Si no hay sesion autenticada, no se deberia acceder al conversor.
        if (!sesionBean.estaAutenticado()) {
            try {
                FacesContext.getCurrentInstance().getExternalContext()
                        .redirect(FacesContext.getCurrentInstance().getExternalContext()
                                .getRequestContextPath() + "/login.xhtml");
            } catch (Exception e) {
              
            }
        }
    }

    public void convertir() {
        try {
            switch (tipoConversion) {
                case "masa":
                    resultado = conversorServicio.convertirMasa(
                            sesionBean.getToken(), valor, unidadInicial, unidadFinal);
                    break;
                case "temperatura":
                    resultado = conversorServicio.convertirTemperatura(
                            sesionBean.getToken(), valor, unidadInicial, unidadFinal);
                    break;
                case "longitud":
                default:
                    resultado = conversorServicio.convertirLongitud(
                            sesionBean.getToken(), valor, unidadInicial, unidadFinal);
                    break;
            }
            mensaje(FacesMessage.SEVERITY_INFO, "Conversion realizada: " + resultado);
        } catch (Exception e) {
            mensaje(FacesMessage.SEVERITY_ERROR, "Error al convertir: " + e.getMessage());
        }
    }

    public String cerrarSesion() {
        sesionBean.cerrarSesion();
        return "login?faces-redirect=true";
    }

    private void mensaje(FacesMessage.Severity severidad, String texto) {
        FacesContext.getCurrentInstance().addMessage(null, new FacesMessage(severidad, texto, texto));
    }

    public double getValor() {
        return valor;
    }

    public void setValor(double valor) {
        this.valor = valor;
    }

    public String getUnidadInicial() {
        return unidadInicial;
    }

    public void setUnidadInicial(String unidadInicial) {
        this.unidadInicial = unidadInicial;
    }

    public String getUnidadFinal() {
        return unidadFinal;
    }

    public void setUnidadFinal(String unidadFinal) {
        this.unidadFinal = unidadFinal;
    }

    public double getResultado() {
        return resultado;
    }

    public String getTipoConversion() {
        return tipoConversion;
    }

    public void setTipoConversion(String tipoConversion) {
        this.tipoConversion = tipoConversion;
    }

    public SesionBean getSesionBean() {
        return sesionBean;
    }

    public UnidadLongitud[] getUnidadesLongitud() {
        return UnidadLongitud.values();
    }

    public UnidadMasa[] getUnidadesMasa() {
        return UnidadMasa.values();
    }

    public UnidadTemperatura[] getUnidadesTemperatura() {
        return UnidadTemperatura.values();
    }

    public void onTipoCambio() {
        unidadInicial = null;
        unidadFinal = null;
        resultado = 0;
    }

    public String getVistaUnidades() {
        switch (tipoConversion) {
            case "masa":
                return "/vistas/masa.xhtml";
            case "temperatura":
                return "/vistas/temperatura.xhtml";
            case "longitud":
            default:
                return "/vistas/longitud.xhtml";
        }
    }
}
