package ec.edu.monster.modelos.DTO;

import java.math.BigDecimal;

public class CuentaDTO {
    private String cuentaId;
    private String cuentaNumero;
    private String sucursalId;
    private String sucursalNombre;
    private BigDecimal cuentaSaldo;
    private String cuentaEstado;

    public CuentaDTO() {}
    public String getCuentaId() { return cuentaId; }
    public void setCuentaId(String v) { this.cuentaId = v; }
    public String getCuentaNumero() { return cuentaNumero; }
    public void setCuentaNumero(String v) { this.cuentaNumero = v; }
    public String getSucursalId() { return sucursalId; }
    public void setSucursalId(String v) { this.sucursalId = v; }
    public String getSucursalNombre() { return sucursalNombre; }
    public void setSucursalNombre(String v) { this.sucursalNombre = v; }
    public BigDecimal getCuentaSaldo() { return cuentaSaldo; }
    public void setCuentaSaldo(BigDecimal v) { this.cuentaSaldo = v; }
    public String getCuentaEstado() { return cuentaEstado; }
    public void setCuentaEstado(String v) { this.cuentaEstado = v; }
}
