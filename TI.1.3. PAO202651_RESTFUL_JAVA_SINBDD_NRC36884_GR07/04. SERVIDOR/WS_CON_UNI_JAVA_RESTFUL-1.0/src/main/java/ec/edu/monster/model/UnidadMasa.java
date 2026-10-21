/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Enum.java to edit this template
 */
package ec.edu.monster.model;

import ec.edu.monster.exception.ExcepcionMasaInvalida;

/**
 *
 * @author marpa
 */
public enum UnidadMasa {
    MILIGRAMO(0.001),
    GRAMO(1.0),
    KILOGRAMO(1000.0),
    TONELADA(1_000_000.0),
    ONZA(28.3495);
    
    private double factor;
    
    UnidadMasa(double factor){
        this.factor = factor;
    }

    public double getFactor() {
        return factor;
    }
    
    public static UnidadMasa desdeString(String stringUnidad){
        if(stringUnidad == null) throw new ExcepcionMasaInvalida("Debe enviarse una unidad de entrada y una de salida.");
        try{
            return UnidadMasa.valueOf(stringUnidad.toUpperCase());
        }
        catch (IllegalArgumentException e) {
            throw new ExcepcionMasaInvalida(
                String.format("Unidad de masa inválida: %s. Válidas: MILIGRAMO, GRAMO, KILOGRAMO, TONELADA, ONZA", stringUnidad)
            );
        }
    }
}
