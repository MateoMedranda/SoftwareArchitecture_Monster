package ec.edu.monster.modelos.enums;

public enum UnidadTemperatura {
    CELSIUS("Celsius"), FAHRENHEIT("Fahrenheit"), KELVIN("Kelvin"), RANKINE("Rankine");

    private final String etiqueta;
    UnidadTemperatura(String etiqueta) { this.etiqueta = etiqueta; }
    public String getEtiqueta() { return etiqueta; }
}
