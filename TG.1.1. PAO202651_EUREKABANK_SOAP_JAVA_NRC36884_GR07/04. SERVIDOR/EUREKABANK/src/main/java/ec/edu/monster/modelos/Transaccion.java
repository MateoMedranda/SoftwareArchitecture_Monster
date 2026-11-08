package ec.edu.monster.modelos;

import java.math.BigDecimal;
import java.time.LocalDateTime;

/**
 * Modelo de la tabla TRANSACCION.
 *
 * Columnas BDD:
 *   TRANSACCION_ID               varchar(36)    PK
 *   CUENTA_ID                    varchar(36)    FK → CUENTA
 *   TRANSACCION_SALDO            decimal(10,2)  — monto de la transacción
 *   TRANSACCION_TIPO             varchar(36)    — p.ej. "DEPOSITO", "RETIRO", "TRANSFERENCIA"
 *   TRANSACCION_SALDO_ANTERIOR   decimal(12,2)  — saldo antes de la transacción
 *   TRANSACCION_SALDO_POSTERIOR  decimal(12,2)  — saldo después de la transacción
 *   TRANSACCION_CREADO_EN      datetime       — fecha y hora de la transacción
 *   TRANSACCION_CREADO_POR       varchar(36)    — USUARIO_ID que realizó la transacción
 *
 * @author MONSTER GR07
 */
public class Transaccion {

    private String         transaccionId;
    private String         cuentaId;
    private BigDecimal     transaccionSaldo;
    private String         transaccionTipo;
    private BigDecimal     transaccionSaldoAnterior;
    private BigDecimal     transaccionSaldoPosterior;
    private LocalDateTime  transaccionCreadoEn;
    private String         transaccionCreadoPor;

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores
    // ─────────────────────────────────────────────────────────────────────────

    public Transaccion() {}

    public Transaccion(String transaccionId, String cuentaId,
                       BigDecimal transaccionSaldo, String transaccionTipo,
                       BigDecimal transaccionSaldoAnterior, BigDecimal transaccionSaldoPosterior,
                       LocalDateTime transaccionCreadoEn, String transaccionCreadoPor) {
        this.transaccionId              = transaccionId;
        this.cuentaId                   = cuentaId;
        this.transaccionSaldo           = transaccionSaldo;
        this.transaccionTipo            = transaccionTipo;
        this.transaccionSaldoAnterior   = transaccionSaldoAnterior;
        this.transaccionSaldoPosterior  = transaccionSaldoPosterior;
        this.transaccionCreadoEn        = transaccionCreadoEn;
        this.transaccionCreadoPor       = transaccionCreadoPor;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Getters / Setters
    // ─────────────────────────────────────────────────────────────────────────

    public String getTransaccionId() { return transaccionId; }
    public void setTransaccionId(String transaccionId) { this.transaccionId = transaccionId; }

    public String getCuentaId() { return cuentaId; }
    public void setCuentaId(String cuentaId) { this.cuentaId = cuentaId; }

    public BigDecimal getTransaccionSaldo() { return transaccionSaldo; }
    public void setTransaccionSaldo(BigDecimal transaccionSaldo) { this.transaccionSaldo = transaccionSaldo; }

    public String getTransaccionTipo() { return transaccionTipo; }
    public void setTransaccionTipo(String transaccionTipo) { this.transaccionTipo = transaccionTipo; }

    public BigDecimal getTransaccionSaldoAnterior() { return transaccionSaldoAnterior; }
    public void setTransaccionSaldoAnterior(BigDecimal transaccionSaldoAnterior) {
        this.transaccionSaldoAnterior = transaccionSaldoAnterior;
    }

    public BigDecimal getTransaccionSaldoPosterior() { return transaccionSaldoPosterior; }
    public void setTransaccionSaldoPosterior(BigDecimal transaccionSaldoPosterior) {
        this.transaccionSaldoPosterior = transaccionSaldoPosterior;
    }

    public LocalDateTime getTransaccionCreadoEn() { return transaccionCreadoEn; }
    public void setTransaccionCreadoEn(LocalDateTime transaccionCreadoEn) {
        this.transaccionCreadoEn = transaccionCreadoEn;
    }

    public String getTransaccionCreadoPor() { return transaccionCreadoPor; }
    public void setTransaccionCreadoPor(String transaccionCreadoPor) {
        this.transaccionCreadoPor = transaccionCreadoPor;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Object
    // ─────────────────────────────────────────────────────────────────────────

    @Override
    public String toString() {
        return "Transaccion{transaccionId='" + transaccionId + "', cuentaId='" + cuentaId
                + "', tipo='" + transaccionTipo + "', saldo=" + transaccionSaldo + "}";
    }
}
