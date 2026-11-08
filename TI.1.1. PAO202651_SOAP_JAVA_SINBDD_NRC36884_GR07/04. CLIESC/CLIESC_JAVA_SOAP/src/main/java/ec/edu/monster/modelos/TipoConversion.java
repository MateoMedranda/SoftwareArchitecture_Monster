package ec.edu.monster.modelos;

import java.util.List;

public enum TipoConversion {

    TEMPERATURA("Temperatura", List.of("CELSIUS", "FAHRENHEIT", "KELVIN", "RANKINE")),
    MASA("Masa", List.of("MILIGRAMO", "GRAMO", "KILOGRAMO", "TONELADA", "ONZA")),
    LONGITUD("Longitud", List.of("PIE", "CENTIMETRO", "METRO", "MILLA", "YARDA"));

    private final String titulo;
    private final List<String> unidades;

    TipoConversion(String titulo, List<String> unidades) {
        this.titulo = titulo;
        this.unidades = List.copyOf(unidades);
    }

    public String getTitulo() {
        return titulo;
    }

    public List<String> getUnidades() {
        return unidades;
    }
}
