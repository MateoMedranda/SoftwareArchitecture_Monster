/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.modelos.utilidades;

/**
 *
 * @author USER
 */
public interface IConversor<T> {

    public double convertir(double valor, T unidadInicial, T unidadFinal);
    
}