/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.modelos.utilidades;

/**
 *
 * @author USER
 */
import ec.edu.monster.modelos.utilidades.enums.UnidadLongitud;
import java.util.Map;

public class ConversorLongitud implements IConversor<UnidadLongitud> {

    private final Map<UnidadLongitud, Double> factores = Map.of(
        UnidadLongitud.PIE, 30.48,
        UnidadLongitud.CENTIMETRO, 1.0,
        UnidadLongitud.METRO, 100.0,
        UnidadLongitud.MILLA, 160900.0,
        UnidadLongitud.YARDA, 91.44
    );

    public ConversorLongitud() {}

    @Override
    public double convertir(double valor, UnidadLongitud unidadOrigen, UnidadLongitud unidadFinal) {

        if (unidadOrigen == null || unidadFinal == null) {
            throw new IllegalArgumentException("Debe enviarse una unidad de entrada y una de salida.");
        }
        if (!factores.containsKey(unidadOrigen) || !factores.containsKey(unidadFinal)) {
            throw new IllegalArgumentException("Unidad no soportada");
        }

        double valorEnBase = valor * factores.get(unidadOrigen);

        return valorEnBase / factores.get(unidadFinal);
    }
}
