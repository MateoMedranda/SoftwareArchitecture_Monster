/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.modelos.excepciones;

/**
 *
 * @author marpa
 */
public class ExcepcionSesionRechazada extends RuntimeException{
    public ExcepcionSesionRechazada(String mensaje) {
        super(mensaje + "Token inválido o expirado");
    }
}
