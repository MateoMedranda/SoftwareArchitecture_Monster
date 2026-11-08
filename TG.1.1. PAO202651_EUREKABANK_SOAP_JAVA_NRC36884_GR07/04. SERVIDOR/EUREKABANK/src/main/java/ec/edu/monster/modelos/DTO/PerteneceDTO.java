package ec.edu.monster.modelos.DTO;

public class PerteneceDTO {
    private String cuentaId;
    private String usuarioId;

    public PerteneceDTO() {}
    public String getCuentaId() { return cuentaId; }
    public void setCuentaId(String v) { this.cuentaId = v; }
    public String getUsuarioId() { return usuarioId; }
    public void setUsuarioId(String v) { this.usuarioId = v; }
}
