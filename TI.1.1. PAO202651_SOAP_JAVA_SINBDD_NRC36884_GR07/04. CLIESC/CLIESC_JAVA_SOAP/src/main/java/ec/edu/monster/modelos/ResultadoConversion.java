/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.modelos;

/**
 *
 * @author gmlop
 */
public class ResultadoConversion {
    private final double valor;
    private final String unidadOrigen;
    private final String unidadFin;
    private final double resultado;
    
    public ResultadoConversion(String unidadOrigen,String unidadFin,double resultado,double valor) {
        this.valor = valor;
        this.unidadOrigen = unidadOrigen;
        this.unidadFin = unidadFin;
        this.resultado = resultado;
    }

    public double getValor() {
        return valor;
    }

    public String getUnidadOrigen() {
        return unidadOrigen;
    }

    public String getUnidadFin() {
        return unidadFin;
    }

    public double getResultado() {
        return resultado;
    }
    
    
    
}
