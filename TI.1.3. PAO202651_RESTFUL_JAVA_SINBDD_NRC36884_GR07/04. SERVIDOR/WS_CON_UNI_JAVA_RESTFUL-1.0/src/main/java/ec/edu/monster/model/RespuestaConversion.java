package ec.edu.monster.model;

import java.io.Serializable;
import org.eclipse.microprofile.openapi.annotations.enums.SchemaType;
import org.eclipse.microprofile.openapi.annotations.media.Schema;

@Schema(description = "Objeto devuelto tras una conversión de unidades exitosa")
public class RespuestaConversion implements Serializable {

    @Schema(description = "Valor original en la unidad de origen", type = SchemaType.NUMBER, example = "12.5")
    private Double valorOriginal;
    @Schema(description = "Valor convertido en la unidad de destino", type = SchemaType.NUMBER, example = "13.67")
    private Double valorConvertido;
    @Schema(description = "Unidad usada para el valor original", type = SchemaType.STRING, example = "METRO")
    private String unidadOrigen;
    @Schema(description = "Unidad a la que se convirtió el valor", type = SchemaType.STRING, example = "YARDA")
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