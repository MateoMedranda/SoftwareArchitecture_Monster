package ec.edu.monster.modelos;

import java.math.BigDecimal;

/**
 * Modelo de la tabla CUENTA.
 *
 * Columnas BDD:
 *   CUENTA_ID     varchar(36)    PK
 *   SUCURSAL_ID   varchar(36)    FK → SUCURSAL
 *   CUENTA_SALDO  decimal(15,2)
 *   CUENTA_ESTADO varchar(24)    — p.ej. "ACTIVA", "INACTIVA", "BLOQUEADA"
 *
 * @author MONSTER GR07
 */
public class Cuenta {

    private String     cuentaId;
    private String     sucursalId;
    private BigDecimal cuentaSaldo;
    private String     cuentaEstado;

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores
    // ─────────────────────────────────────────────────────────────────────────

    public Cuenta() {}

    public Cuenta(String cuentaId, String sucursalId, BigDecimal cuentaSaldo, String cuentaEstado) {
        this.cuentaId    = cuentaId;
        this.sucursalId  = sucursalId;
        this.cuentaSaldo = cuentaSaldo;
        this.cuentaEstado = cuentaEstado;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Getters / Setters
    // ─────────────────────────────────────────────────────────────────────────

    public String getCuentaId() { return cuentaId; }
    public void setCuentaId(String cuentaId) { this.cuentaId = cuentaId; }

    public String getSucursalId() { return sucursalId; }
    public void setSucursalId(String sucursalId) { this.sucursalId = sucursalId; }

    public BigDecimal getCuentaSaldo() { return cuentaSaldo; }
    public void setCuentaSaldo(BigDecimal cuentaSaldo) { this.cuentaSaldo = cuentaSaldo; }

    public String getCuentaEstado() { return cuentaEstado; }
    public void setCuentaEstado(String cuentaEstado) { this.cuentaEstado = cuentaEstado; }

    // ─────────────────────────────────────────────────────────────────────────
    //  Object
    // ─────────────────────────────────────────────────────────────────────────

    @Override
    public String toString() {
        return "Cuenta{cuentaId='" + cuentaId + "', cuentaSaldo=" + cuentaSaldo
                + ", cuentaEstado='" + cuentaEstado + "'}";
    }
}
