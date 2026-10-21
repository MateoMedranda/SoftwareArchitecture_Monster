package ec.edu.monster.modelos.enums;

public enum UnidadMasa {
    MILIGRAMO("Miligramo"), GRAMO("Gramo"), KILOGRAMO("Kilogramo"), TONELADA("Tonelada"), ONZA("Onza");

    private final String etiqueta;
    UnidadMasa(String etiqueta) { this.etiqueta = etiqueta; }
    public String getEtiqueta() { return etiqueta; }
}
