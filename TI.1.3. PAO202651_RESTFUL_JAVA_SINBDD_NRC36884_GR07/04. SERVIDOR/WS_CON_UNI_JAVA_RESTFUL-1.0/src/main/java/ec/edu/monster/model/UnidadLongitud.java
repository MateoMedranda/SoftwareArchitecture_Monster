/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Enum.java to edit this template
 */
package ec.edu.monster.model;
import ec.edu.monster.exception.ExcepcionLongitudInvalida;
/**
 *
 * @author marpa
 */
public enum UnidadLongitud {
    PIE(30.48),
    CENTIMETRO(1.0),
    METRO(100.0),
    MILLA(160934.4),
    YARDA(91.44);
    
    private double factor;
    
    UnidadLongitud(double factor){
        this.factor = factor;
    }

    public double getFactor() {
        return factor;
    }
    
    public static UnidadLongitud desdeString(String stringUnidad){
        if(stringUnidad == null) throw new ExcepcionLongitudInvalida("Debe enviarse una unidad de entrada y una de salida.");
        try{
            return UnidadLongitud.valueOf(stringUnidad.toUpperCase());
        }
        catch (IllegalArgumentException e) {
            throw new ExcepcionLongitudInvalida(
                String.format("Unidad de temperatura inválida: %s. Válidas: METRO, PIE, CENTIMETRO, YARDA, MILLA", stringUnidad)
            );
        }
    }
}
