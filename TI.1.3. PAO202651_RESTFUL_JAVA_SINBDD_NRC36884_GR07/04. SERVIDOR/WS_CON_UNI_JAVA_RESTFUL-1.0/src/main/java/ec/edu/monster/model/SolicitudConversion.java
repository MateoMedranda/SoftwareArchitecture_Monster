package ec.edu.monster.model;

import jakarta.validation.constraints.NotNull;
import java.io.Serializable;
import org.eclipse.microprofile.openapi.annotations.enums.SchemaType;
import org.eclipse.microprofile.openapi.annotations.media.Schema;

public class SolicitudConversion implements Serializable {

    @Schema(description = "Valor numérico en la unidad de origen.", type = SchemaType.NUMBER, example = "12.5", required = true)
    @NotNull(message = "El valor es requerido")
    private Double valor;

    @Schema(description = "Unidad de origen.", type = SchemaType.STRING, example = "METRO" , required = true)
    @NotNull(message = "La unidad de origen es requerida")
    private String unidadOrigen;

    @Schema(description = "Unidad de destino.", type = SchemaType.STRING, example = "YARDA" , required = true)
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