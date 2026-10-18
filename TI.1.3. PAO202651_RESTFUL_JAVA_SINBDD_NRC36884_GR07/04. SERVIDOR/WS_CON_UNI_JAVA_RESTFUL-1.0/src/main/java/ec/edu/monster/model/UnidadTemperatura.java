package ec.edu.monster.model;

import ec.edu.monster.exception.ExcepcionTemperaturaInvalida;

public enum UnidadTemperatura {
    CELSIUS(-273.15, 100.00),
    FAHRENHEIT(-459.67, 212.00),
    KELVIN(0.00, 373.15),
    RANKINE(0.00, 671.67);

    private final double minimo;
    private final double maximo;

    UnidadTemperatura(double minimo, double maximo) {
        this.minimo = minimo;
        this.maximo = maximo;
    }

    public double getMinimo() {
        return minimo;
    }

    public double getMaximo() {
        return maximo;
    }

    public void validar(double valor) {
        if (valor < minimo || valor > maximo) {
            throw new ExcepcionTemperaturaInvalida(
                String.format("Temperatura fuera de rango para %s: valor %.2f no está entre %.2f y %.2f",
                    this.name(), valor, minimo, maximo)
            );
        }
    }

    public static UnidadTemperatura desdeString(String unidad) {
        if (unidad == null) {
            throw new ExcepcionTemperaturaInvalida("Unidad de temperatura no puede ser nula");
        }
        try {
            return UnidadTemperatura.valueOf(unidad.toUpperCase());
        } catch (IllegalArgumentException e) {
            throw new ExcepcionTemperaturaInvalida(
                String.format("Unidad de temperatura inválida: %s. Válidas: CELSIUS, FAHRENHEIT, KELVIN, RANKINE", unidad)
            );
        }
    }
}