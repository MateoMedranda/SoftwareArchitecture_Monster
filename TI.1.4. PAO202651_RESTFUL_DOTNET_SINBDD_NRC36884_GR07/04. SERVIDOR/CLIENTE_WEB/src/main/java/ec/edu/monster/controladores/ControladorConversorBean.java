package ec.edu.monster.controladores;

import ec.edu.monster.modelos.ResultadoConversion;
import ec.edu.monster.modelos.SesionBean;
import ec.edu.monster.modelos.enums.UnidadLongitud;
import ec.edu.monster.modelos.enums.UnidadMasa;
import ec.edu.monster.modelos.enums.UnidadTemperatura;
import ec.edu.monster.servicios.ApiRestException;
import ec.edu.monster.servicios.ConversorServicio;
import jakarta.annotation.PostConstruct;
import jakarta.faces.application.FacesMessage;
import jakarta.faces.context.FacesContext;
import jakarta.faces.view.ViewScoped;
import jakarta.inject.Inject;
import jakarta.inject.Named;
import java.io.IOException;
import java.io.Serializable;

@Named("conversorBean")
@ViewScoped
public class ControladorConversorBean implements Serializable {

    private static final long serialVersionUID = 1L;

    @Inject
    private ConversorServicio conversorServicio;

    @Inject
    private SesionBean sesionBean;

    private Double valor;
    private String unidadInicial;
    private String unidadFinal;
    private ResultadoConversion resultado;
    private String tipoConversion = "longitud";

    @PostConstruct
    public void init() {
        if (!sesionBean.estaAutenticado()) {
            try {
                FacesContext contexto = FacesContext.getCurrentInstance();
                contexto.getExternalContext().redirect(
                        contexto.getExternalContext().getRequestContextPath() + "/login.xhtml");
            } catch (IOException ignored) {
                // El filtro de la vista volverá a solicitar el inicio de sesión.
            }
        }
    }

    public void convertir() {
        if (valor == null || unidadInicial == null || unidadInicial.isBlank()
                || unidadFinal == null || unidadFinal.isBlank()) {
            mensaje(FacesMessage.SEVERITY_WARN, "Ingrese un valor y seleccione ambas unidades.");
            return;
        }

        try {
            resultado = switch (tipoConversion) {
                case "masa" -> conversorServicio.convertirMasa(
                        sesionBean.getToken(), valor, unidadInicial, unidadFinal);
                case "temperatura" -> conversorServicio.convertirTemperatura(
                        sesionBean.getToken(), valor, unidadInicial, unidadFinal);
                default -> conversorServicio.convertirLongitud(
                        sesionBean.getToken(), valor, unidadInicial, unidadFinal);
            };
            mensaje(FacesMessage.SEVERITY_INFO, "Conversión realizada correctamente.");
        } catch (ApiRestException e) {
            if (e.getMessage().startsWith("Sesión expirada")) {
                sesionBean.cerrarSesion();
            }
            mensaje(FacesMessage.SEVERITY_ERROR, e.getMessage());
        }
    }

    public String cerrarSesion() {
        sesionBean.cerrarSesion();
        return "login?faces-redirect=true";
    }

    public void onTipoCambio() {
        unidadInicial = null;
        unidadFinal = null;
        resultado = null;
    }

    private void mensaje(FacesMessage.Severity severidad, String detalle) {
        FacesContext.getCurrentInstance().addMessage(null,
                new FacesMessage(severidad, "", detalle));
    }

    public Double getValor() { return valor; }
    public void setValor(Double valor) { this.valor = valor; }
    public String getUnidadInicial() { return unidadInicial; }
    public void setUnidadInicial(String unidadInicial) { this.unidadInicial = unidadInicial; }
    public String getUnidadFinal() { return unidadFinal; }
    public void setUnidadFinal(String unidadFinal) { this.unidadFinal = unidadFinal; }
    public ResultadoConversion getResultado() { return resultado; }
    public String getTipoConversion() { return tipoConversion; }
    public void setTipoConversion(String tipoConversion) { this.tipoConversion = tipoConversion; }
    public UnidadLongitud[] getUnidadesLongitud() { return UnidadLongitud.values(); }
    public UnidadMasa[] getUnidadesMasa() { return UnidadMasa.values(); }
    public UnidadTemperatura[] getUnidadesTemperatura() { return UnidadTemperatura.values(); }

    public String getVistaUnidades() {
        return switch (tipoConversion) {
            case "masa" -> "/vistas/masa.xhtml";
            case "temperatura" -> "/vistas/temperatura.xhtml";
            default -> "/vistas/longitud.xhtml";
        };
    }
}
