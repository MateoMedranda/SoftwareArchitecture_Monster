package ec.edu.monster.modelos.Respuestas;

import jakarta.xml.bind.annotation.XmlAccessType;
import jakarta.xml.bind.annotation.XmlAccessorType;
import jakarta.xml.bind.annotation.XmlRootElement;

/**
 * DTO de respuesta exitosa del servicio SOAP de login.
 *
 * @author MONSTER GR07
 */
@XmlRootElement(name = "respuestaTransaccion")
@XmlAccessorType(XmlAccessType.FIELD)
public class RespuestaTransaccion {

    /** Indica si la operación fue exitosa */
    private boolean exitoso;

    /** Token JWT generado (header.payload.signature) — válido por 8 horas */
    private String mensaje;
    
    private String tipo;

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores
    // ─────────────────────────────────────────────────────────────────────────

    public RespuestaTransaccion() {}

    public RespuestaTransaccion(boolean exitoso, String mensaje,  String tipo) {
        this.exitoso = exitoso;
        this.mensaje = mensaje;
        this.tipo = tipo;
    }

    public boolean isExitoso() {
        return exitoso;
    }

    public void setExitoso(boolean exitoso) {
        this.exitoso = exitoso;
    }

    public String getMensaje() {
        return mensaje;
    }

    public void setMensaje(String mensaje) {
        this.mensaje = mensaje;
    }


    public String getTipo() {
        return tipo;
    }

    public void setTipo(String tipo) {
        this.tipo = tipo;
    }
        
    @Override
    public String toString() {
        return "RespuestaTransaccion{exitoso=" + exitoso
                + ", tipo='" + tipo + '\''
                + ", mensaje='" + mensaje + '\''  
                + '}';
    }
}
