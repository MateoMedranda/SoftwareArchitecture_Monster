/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.service;

import ec.edu.monster.exception.ExcepcionMasaInvalida;
import ec.edu.monster.model.RespuestaConversion;
import ec.edu.monster.model.SolicitudConversion;
import ec.edu.monster.model.UnidadMasa;
import jakarta.enterprise.context.ApplicationScoped;

/**
 *
 * @author marpa
 */
@ApplicationScoped
public class ServicioMasa {
    public RespuestaConversion convertir(SolicitudConversion solicitud) {
        UnidadMasa unidadOrigen = UnidadMasa.desdeString(solicitud.getUnidadOrigen());
        UnidadMasa unidadDestino = UnidadMasa.desdeString(solicitud.getUnidadDestino());
        double valor = solicitud.getValor();
        if (!Double.isFinite(valor) || valor<0)
        {
            throw new ExcepcionMasaInvalida("No se permite ingresar valores negativos");
        }
        
        double valorConvertido = calcularConversion(valor, unidadOrigen, unidadDestino); 

        return new RespuestaConversion(valor, valorConvertido, unidadOrigen.name(), unidadDestino.name());
    }
    
    private double calcularConversion(double valor, UnidadMasa unidadOrigen, UnidadMasa unidadFinal)
    {
        double valorABase = valor * unidadOrigen.getFactor();
        double valorDesdeBase = valorABase / unidadFinal.getFactor();

        return valorDesdeBase;
    }
}
