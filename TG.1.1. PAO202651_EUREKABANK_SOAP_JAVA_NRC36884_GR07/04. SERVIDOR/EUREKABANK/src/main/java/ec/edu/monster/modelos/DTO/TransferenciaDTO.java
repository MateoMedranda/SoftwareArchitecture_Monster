package ec.edu.monster.modelos.DTO;

import java.math.BigDecimal;
import java.time.LocalDateTime;

/** Resultado auditable de una transferencia entre dos cuentas. */
public class TransferenciaDTO {
    private String transferenciaId;
    private String cuentaOrigenId;
    private String cuentaDestinoId;
    private BigDecimal monto;
    private TransaccionDTO debito;
    private TransaccionDTO credito;
    private LocalDateTime creadoEn;

    public TransferenciaDTO() {}
    public String getTransferenciaId() { return transferenciaId; }
    public void setTransferenciaId(String v) { transferenciaId = v; }
    public String getCuentaOrigenId() { return cuentaOrigenId; }
    public void setCuentaOrigenId(String v) { cuentaOrigenId = v; }
    public String getCuentaDestinoId() { return cuentaDestinoId; }
    public void setCuentaDestinoId(String v) { cuentaDestinoId = v; }
    public BigDecimal getMonto() { return monto; }
    public void setMonto(BigDecimal v) { monto = v; }
    public TransaccionDTO getDebito() { return debito; }
    public void setDebito(TransaccionDTO v) { debito = v; }
    public TransaccionDTO getCredito() { return credito; }
    public void setCredito(TransaccionDTO v) { credito = v; }
    public LocalDateTime getCreadoEn() { return creadoEn; }
    public void setCreadoEn(LocalDateTime v) { creadoEn = v; }
}
