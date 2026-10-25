package ec.edu.monster.modelos;

/**
 * Modelo de la tabla SUCURSAL.
 *
 * Columnas BDD:
 *   SUCURSAL_ID     varchar(36) PK
 *   SUCURSAL_NOMBRE varchar(64)
 *
 * @author MONSTER GR07
 */
public class Sucursal {

    private String sucursalId;
    private String sucursalNombre;

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores
    // ─────────────────────────────────────────────────────────────────────────

    public Sucursal() {}

    public Sucursal(String sucursalId, String sucursalNombre) {
        this.sucursalId     = sucursalId;
        this.sucursalNombre = sucursalNombre;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Getters / Setters
    // ─────────────────────────────────────────────────────────────────────────

    public String getSucursalId() { return sucursalId; }
    public void setSucursalId(String sucursalId) { this.sucursalId = sucursalId; }

    public String getSucursalNombre() { return sucursalNombre; }
    public void setSucursalNombre(String sucursalNombre) { this.sucursalNombre = sucursalNombre; }

    // ─────────────────────────────────────────────────────────────────────────
    //  Object
    // ─────────────────────────────────────────────────────────────────────────

    @Override
    public String toString() {
        return "Sucursal{sucursalId='" + sucursalId + "', sucursalNombre='" + sucursalNombre + "'}";
    }
}
