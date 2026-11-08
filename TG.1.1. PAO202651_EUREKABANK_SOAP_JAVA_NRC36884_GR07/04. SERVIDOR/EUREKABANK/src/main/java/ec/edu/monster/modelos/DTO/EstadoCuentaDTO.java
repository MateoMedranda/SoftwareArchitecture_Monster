package ec.edu.monster.modelos.DTO;

/** Estado temporal de disponibilidad de una cuenta para clientes SOAP. */
public class EstadoCuentaDTO {
    private String cuentaId;
    private String estado;
    private long version;
    private boolean bloqueada;

    public EstadoCuentaDTO() {}
    public String getCuentaId() { return cuentaId; }
    public void setCuentaId(String v) { cuentaId = v; }
    public String getEstado() { return estado; }
    public void setEstado(String v) { estado = v; }
    public long getVersion() { return version; }
    public void setVersion(long v) { version = v; }
    public boolean isBloqueada() { return bloqueada; }
    public void setBloqueada(boolean v) { bloqueada = v; }
}
