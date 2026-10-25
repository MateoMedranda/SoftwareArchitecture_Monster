package ec.edu.monster.modelos;

/**
 * Modelo de la tabla ROL.
 *
 * Columnas BDD:
 *   ROL_ID     varchar(36) PK
 *   ROL_NOMBRE varchar(36) UNIQUE
 *
 * Roles iniciales: ADMIN, CLIENTE, CAJERO
 *
 * @author MONSTER GR07
 */
public class Rol {

    private String rolId;
    private String rolNombre;

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores
    // ─────────────────────────────────────────────────────────────────────────

    public Rol() {}

    public Rol(String rolId, String rolNombre) {
        this.rolId     = rolId;
        this.rolNombre = rolNombre;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Getters / Setters
    // ─────────────────────────────────────────────────────────────────────────

    public String getRolId() { return rolId; }
    public void setRolId(String rolId) { this.rolId = rolId; }

    public String getRolNombre() { return rolNombre; }
    public void setRolNombre(String rolNombre) { this.rolNombre = rolNombre; }

    // ─────────────────────────────────────────────────────────────────────────
    //  Object
    // ─────────────────────────────────────────────────────────────────────────

    @Override
    public String toString() {
        return "Rol{rolId='" + rolId + "', rolNombre='" + rolNombre + "'}";
    }
}
