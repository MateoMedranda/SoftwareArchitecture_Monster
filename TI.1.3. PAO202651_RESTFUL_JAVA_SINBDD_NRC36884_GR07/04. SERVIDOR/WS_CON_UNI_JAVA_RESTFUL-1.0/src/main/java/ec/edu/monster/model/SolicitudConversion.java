package ec.edu.monster.model;

import jakarta.validation.constraints.NotNull;
import java.io.Serializable;

public class SolicitudConversion implements Serializable {

    @NotNull(message = "El valor es requerido")
    private Double valor;

    @NotNull(message = "La unidad de origen es requerida")
    private String unidadOrigen;

    @NotNull(message = "La unidad de destino es requerida")
    private String unidadDestino;

    public SolicitudConversion() {}

    public SolicitudConversion(Double valor, String unidadOrigen, String unidadDestino) {
        this.valor = valor;
        this.unidadOrigen = unidadOrigen;
        this.unidadDestino = unidadDestino;
    }

    public Double getValor() {
        return valor;
    }

    public void setValor(Double valor) {
        this.valor = valor;
    }

    public String getUnidadOrigen() {
        return unidadOrigen;
    }

    public void setUnidadOrigen(String unidadOrigen) {
        this.unidadOrigen = unidadOrigen;
    }

    public String getUnidadDestino() {
        return unidadDestino;
    }

    public void setUnidadDestino(String unidadDestino) {
        this.unidadDestino = unidadDestino;
    }
}