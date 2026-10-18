package ec.edu.monster.model;

import java.io.Serializable;
import java.time.LocalDateTime;

public class RespuestaError implements Serializable {

    private int codigo;
    private String mensaje;
    private String detalles;
    private LocalDateTime fechaHora;

    public RespuestaError() {
        this.fechaHora = LocalDateTime.now();
    }

    public RespuestaError(int codigo, String mensaje, String detalles) {
        this();
        this.codigo = codigo;
        this.mensaje = mensaje;
        this.detalles = detalles;
    }

    public int getCodigo() {
        return codigo;
    }

    public void setCodigo(int codigo) {
        this.codigo = codigo;
    }

    public String getMensaje() {
        return mensaje;
    }

    public void setMensaje(String mensaje) {
        this.mensaje = mensaje;
    }

    public String getDetalles() {
        return detalles;
    }

    public void setDetalles(String detalles) {
        this.detalles = detalles;
    }

    public LocalDateTime getFechaHora() {
        return fechaHora;
    }

    public void setFechaHora(LocalDateTime fechaHora) {
        this.fechaHora = fechaHora;
    }
}