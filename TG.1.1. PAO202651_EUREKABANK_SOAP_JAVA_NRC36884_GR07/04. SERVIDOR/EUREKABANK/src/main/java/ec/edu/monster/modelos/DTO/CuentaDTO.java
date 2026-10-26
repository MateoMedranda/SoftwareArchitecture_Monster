package ec.edu.monster.modelos.DTO;

import java.math.BigDecimal;

public class CuentaDTO {
    private String cuentaId;
    private String sucursalId;
    private BigDecimal cuentaSaldo;
    private String cuentaEstado;

    public CuentaDTO() {}
    public String getCuentaId() { return cuentaId; }
    public void setCuentaId(String v) { this.cuentaId = v; }
    public String getSucursalId() { return sucursalId; }
    public void setSucursalId(String v) { this.sucursalId = v; }
    public BigDecimal getCuentaSaldo() { return cuentaSaldo; }
    public void setCuentaSaldo(BigDecimal v) { this.cuentaSaldo = v; }
    public String getCuentaEstado() { return cuentaEstado; }
    public void setCuentaEstado(String v) { this.cuentaEstado = v; }
}
