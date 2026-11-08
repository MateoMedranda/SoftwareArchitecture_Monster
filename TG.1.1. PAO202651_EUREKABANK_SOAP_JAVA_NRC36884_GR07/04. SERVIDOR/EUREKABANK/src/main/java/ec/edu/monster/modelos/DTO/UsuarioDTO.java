package ec.edu.monster.modelos.DTO;

public class UsuarioDTO {
    private String usuarioId;
    private String rolId;
    private String usuarioNombre;
    private String usuarioApellido;
    private String usuarioTelefono;
    private String usuarioUsuario;
    private String usuarioContraseania;

    public UsuarioDTO() {}
    public String getUsuarioId() { return usuarioId; }
    public void setUsuarioId(String v) { this.usuarioId = v; }
    public String getRolId() { return rolId; }
    public void setRolId(String v) { this.rolId = v; }
    public String getUsuarioNombre() { return usuarioNombre; }
    public void setUsuarioNombre(String v) { this.usuarioNombre = v; }
    public String getUsuarioApellido() { return usuarioApellido; }
    public void setUsuarioApellido(String v) { this.usuarioApellido = v; }
    public String getUsuarioTelefono() { return usuarioTelefono; }
    public void setUsuarioTelefono(String v) { this.usuarioTelefono = v; }
    public String getUsuarioUsuario() { return usuarioUsuario; }
    public void setUsuarioUsuario(String v) { this.usuarioUsuario = v; }
    public String getUsuarioContraseania() { return usuarioContraseania; }
    public void setUsuarioContraseania(String v) { this.usuarioContraseania = v; }
}
