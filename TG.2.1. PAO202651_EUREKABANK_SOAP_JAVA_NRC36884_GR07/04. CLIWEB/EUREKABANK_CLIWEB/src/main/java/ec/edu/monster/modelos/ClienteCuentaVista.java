package ec.edu.monster.modelos;

/** Opción segura mostrada en el selector para crear una cuenta.
 * Se usa un JavaBean convencional porque JSF/EL resuelve sus propiedades
 * mediante getters, no mediante los accesores propios de un record.
 */
public final class ClienteCuentaVista {
    private final String id;
    private final String nombre;
    private final String usuario;

    public ClienteCuentaVista(String id, String nombre, String usuario) {
        this.id = id;
        this.nombre = nombre;
        this.usuario = usuario;
    }

    public String getId() { return id; }
    public String getNombre() { return nombre; }
    public String getUsuario() { return usuario; }
}
