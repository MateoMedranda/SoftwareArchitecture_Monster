package ec.edu.monster.modelos.enums;

public enum UnidadLongitud {
    PIE("Pie"), CENTIMETRO("Centímetro"), METRO("Metro"), MILLA("Milla"), YARDA("Yarda");

    private final String etiqueta;
    UnidadLongitud(String etiqueta) { this.etiqueta = etiqueta; }
    public String getEtiqueta() { return etiqueta; }
}
