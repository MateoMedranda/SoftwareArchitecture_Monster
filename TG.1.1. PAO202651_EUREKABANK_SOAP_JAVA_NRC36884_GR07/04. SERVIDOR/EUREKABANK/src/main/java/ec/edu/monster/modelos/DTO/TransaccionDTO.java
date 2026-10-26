package ec.edu.monster.modelos.DTO;

import java.math.BigDecimal;
import java.time.LocalDateTime;

public class TransaccionDTO {
    private String transaccionId;
    private String cuentaId;
    private BigDecimal transaccionSaldo;
    private String transaccionTipo;
    private BigDecimal transaccionSaldoAnterior;
    private BigDecimal transaccionSaldoPosterior;
    private LocalDateTime transaccionCreadoEn;
    private String transaccionCreadoPor;

    public TransaccionDTO() {}
    public String getTransaccionId() { return transaccionId; }
    public void setTransaccionId(String v) { this.transaccionId = v; }
    public String getCuentaId() { return cuentaId; }
    public void setCuentaId(String v) { this.cuentaId = v; }
    public BigDecimal getTransaccionSaldo() { return transaccionSaldo; }
    public void setTransaccionSaldo(BigDecimal v) { this.transaccionSaldo = v; }
    public String getTransaccionTipo() { return transaccionTipo; }
    public void setTransaccionTipo(String v) { this.transaccionTipo = v; }
    public BigDecimal getTransaccionSaldoAnterior() { return transaccionSaldoAnterior; }
    public void setTransaccionSaldoAnterior(BigDecimal v) { this.transaccionSaldoAnterior = v; }
    public BigDecimal getTransaccionSaldoPosterior() { return transaccionSaldoPosterior; }
    public void setTransaccionSaldoPosterior(BigDecimal v) { this.transaccionSaldoPosterior = v; }
    public LocalDateTime getTransaccionCreadoEn() { return transaccionCreadoEn; }
    public void setTransaccionCreadoEn(LocalDateTime v) { this.transaccionCreadoEn = v; }
    public String getTransaccionCreadoPor() { return transaccionCreadoPor; }
    public void setTransaccionCreadoPor(String v) { this.transaccionCreadoPor = v; }
}
