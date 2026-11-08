package ec.edu.monster.modelos;

import java.io.Serializable;

public class ResultadoConversion implements Serializable {

    private static final long serialVersionUID = 1L;

    private final double valorOriginal;
    private final double valorConvertido;
    private final String unidadOrigen;
    private final String unidadDestino;

    public ResultadoConversion(double valorOriginal, double valorConvertido,
            String unidadOrigen, String unidadDestino) {
        this.valorOriginal = valorOriginal;
        this.valorConvertido = valorConvertido;
        this.unidadOrigen = unidadOrigen;
        this.unidadDestino = unidadDestino;
    }

    public double getValorOriginal() { return valorOriginal; }
    public double getValorConvertido() { return valorConvertido; }
    public String getUnidadOrigen() { return unidadOrigen; }
    public String getUnidadDestino() { return unidadDestino; }
}
