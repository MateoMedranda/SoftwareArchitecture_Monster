/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.service;
import ec.edu.monster.exception.ExcepcionLongitudInvalida;
import ec.edu.monster.model.RespuestaConversion;
import ec.edu.monster.model.SolicitudConversion;
import java.util.Map;
import ec.edu.monster.model.UnidadLongitud;
import jakarta.enterprise.context.ApplicationScoped;
/**
 *
 * @author marpa
 */
@ApplicationScoped
public class ServicioLongitud {

    public RespuestaConversion convertir(SolicitudConversion solicitud) {
        UnidadLongitud unidadOrigen = UnidadLongitud.desdeString(solicitud.getUnidadOrigen());
        UnidadLongitud unidadDestino = UnidadLongitud.desdeString(solicitud.getUnidadDestino());
        double valor = solicitud.getValor();
        if (!Double.isFinite(valor) || valor<0)
        {
            throw new ExcepcionLongitudInvalida("No se permite ingresar valores negativos");
        }
        
        double valorConvertido = calcularConversion(valor, unidadOrigen, unidadDestino); 

        return new RespuestaConversion(valor, valorConvertido, unidadOrigen.name(), unidadDestino.name());
    }
    
    private double calcularConversion(double valor, UnidadLongitud unidadOrigen, UnidadLongitud unidadFinal)
    {
        double valorABase = valor * unidadOrigen.getFactor();
        double valorDesdeBase = valorABase / unidadFinal.getFactor();

        return valorDesdeBase;
    }
    
}
