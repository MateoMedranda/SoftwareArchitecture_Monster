/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.exception;

/**
 *
 * @author marpa
 */
public class ExcepcionMasaInvalida extends IllegalArgumentException{
    public ExcepcionMasaInvalida(String mensaje) {
        super(mensaje);
    }

    public ExcepcionMasaInvalida(String mensaje, Throwable causa) {
        super(mensaje, causa);
    }
}
