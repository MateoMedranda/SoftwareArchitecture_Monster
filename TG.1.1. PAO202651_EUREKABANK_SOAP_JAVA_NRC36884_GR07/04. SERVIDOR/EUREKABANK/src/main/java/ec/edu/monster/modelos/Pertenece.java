package ec.edu.monster.modelos;

/**
 * Modelo de la tabla PERTENECE (relación N:M entre CUENTA y USUARIO).
 *
 * Columnas BDD:
 *   CUENTA_ID   varchar(36) PK, FK → CUENTA
 *   USUARIO_ID  varchar(36) PK, FK → USUARIO
 *
 * Clave primaria compuesta: (CUENTA_ID, USUARIO_ID)
 *
 * @author MONSTER GR07
 */
public class Pertenece {

    private String cuentaId;
    private String usuarioId;

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores
    // ─────────────────────────────────────────────────────────────────────────

    public Pertenece() {}

    public Pertenece(String cuentaId, String usuarioId) {
        this.cuentaId  = cuentaId;
        this.usuarioId = usuarioId;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Getters / Setters
    // ─────────────────────────────────────────────────────────────────────────

    public String getCuentaId() { return cuentaId; }
    public void setCuentaId(String cuentaId) { this.cuentaId = cuentaId; }

    public String getUsuarioId() { return usuarioId; }
    public void setUsuarioId(String usuarioId) { this.usuarioId = usuarioId; }

    // ─────────────────────────────────────────────────────────────────────────
    //  Object
    // ─────────────────────────────────────────────────────────────────────────

    @Override
    public String toString() {
        return "Pertenece{cuentaId='" + cuentaId + "', usuarioId='" + usuarioId + "'}";
    }
}
