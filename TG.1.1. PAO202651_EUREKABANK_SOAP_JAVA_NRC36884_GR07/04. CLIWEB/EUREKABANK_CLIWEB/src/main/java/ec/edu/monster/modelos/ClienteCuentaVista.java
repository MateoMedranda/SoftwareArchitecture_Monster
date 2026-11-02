package ec.edu.monster.modelos;

/** Opción segura mostrada en el selector para crear una cuenta. */
public record ClienteCuentaVista(String id, String nombre, String usuario) {
    public String getEtiqueta() { return nombre + " · @" + usuario; }
}
