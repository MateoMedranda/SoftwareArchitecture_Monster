package ec.edu.monster.model;

import java.io.Serializable;

public class RespuestaConversion implements Serializable {

    private Double valorOriginal;
    private Double valorConvertido;
    private String unidadOrigen;
    private String unidadDestino;

    public RespuestaConversion() {}

    public RespuestaConversion(Double valorOriginal, Double valorConvertido, String unidadOrigen, String unidadDestino) {
        this.valorOriginal = valorOriginal;
        this.valorConvertido = valorConvertido;
        this.unidadOrigen = unidadOrigen;
        this.unidadDestino = unidadDestino;
    }

    public Double getValorOriginal() {
        return valorOriginal;
    }

    public void setValorOriginal(Double valorOriginal) {
        this.valorOriginal = valorOriginal;
    }

    public Double getValorConvertido() {
        return valorConvertido;
    }

    public void setValorConvertido(Double valorConvertido) {
        this.valorConvertido = valorConvertido;
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