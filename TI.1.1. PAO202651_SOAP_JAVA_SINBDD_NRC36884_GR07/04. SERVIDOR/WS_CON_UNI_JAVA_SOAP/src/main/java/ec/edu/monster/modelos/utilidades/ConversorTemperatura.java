/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.modelos.utilidades;

/**
 *
 * @author USER
 */
import ec.edu.monster.modelos.utilidades.enums.UnidadTemperatura;
import static ec.edu.monster.modelos.utilidades.enums.UnidadTemperatura.CELSIUS;
import static ec.edu.monster.modelos.utilidades.enums.UnidadTemperatura.FAHRENHEIT;
import static ec.edu.monster.modelos.utilidades.enums.UnidadTemperatura.KELVIN;
import static ec.edu.monster.modelos.utilidades.enums.UnidadTemperatura.RANKINE;

public class ConversorTemperatura implements IConversor<UnidadTemperatura> {

    public ConversorTemperatura() {}

    @Override
    public double convertir(double valor, UnidadTemperatura origen, UnidadTemperatura destino) {

        if (origen == null || destino == null) {
            throw new IllegalArgumentException("Las unidades no pueden ser null");
        }
        if (origen == destino) {
            return valor;
        }
        

        switch (origen) {
            case CELSIUS -> {
                if (valor < -273.15 || valor > 100)
                    throw new IllegalArgumentException("El valor de Celsius debe estar en el rango de -273.15 a 100 ºC");
            }
            case FAHRENHEIT -> {
                if (valor < -459.67 || valor > 212)
                    throw new IllegalArgumentException("El valor de Fahrenheit debe estar en el rango de -459.67 a 212 ºF");
            }
            case KELVIN -> {
                if (valor < 0 || valor > 373.15)
                    throw new IllegalArgumentException("El valor de Kelvin debe estar en el rango de 0 a 373.15 ºK");
            }
            case RANKINE -> {
                if (valor < 0 || valor > 671.67)
                    throw new IllegalArgumentException("El valor de Rankine debe estar en el rango de 0 a 671.67 ºR");
            }
        }

        return switch (origen) {

            case CELSIUS -> switch (destino) {
                case FAHRENHEIT -> (valor * 9 / 5) + 32;
                case KELVIN -> valor + 273.15;
                case RANKINE -> (valor + 273.15) * 9 / 5;
                default -> throw new IllegalArgumentException("Conversión no soportada");
            };

            case FAHRENHEIT -> switch (destino) {
                case CELSIUS -> (valor - 32) * 5 / 9;
                case KELVIN -> (valor - 32) * 5 / 9 + 273.15;
                case RANKINE -> valor + 459.67;
                default -> throw new IllegalArgumentException("Conversión no soportada");
            };

            case KELVIN -> switch (destino) {
                case CELSIUS -> valor - 273.15;
                case FAHRENHEIT -> (valor - 273.15) * 9 / 5 + 32;
                case RANKINE -> valor * 9 / 5;
                default -> throw new IllegalArgumentException("Conversión no soportada");
            };

            case RANKINE -> switch (destino) {
                case CELSIUS -> (valor - 491.67) * 5 / 9;
                case FAHRENHEIT -> valor - 459.67;
                case KELVIN -> valor * 5 / 9;
                default -> throw new IllegalArgumentException("Conversión no soportada");
            };
        };
    }
}

