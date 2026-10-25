package ec.edu.monster.modelos;

/**
 * Modelo de la tabla USUARIO.
 *
 * Columnas BDD:
 *   USUARIO_ID           varchar(36) PK
 *   ROL_ID               varchar(36) FK → ROL
 *   USUARIO_NOMBRE       varchar(36)
 *   USUARIO_APELLIDO     varchar(36)
 *   USUARIO_TELEFONO     varchar(36)
 *   USUARIO_USUARIO      varchar(36) UNIQUE  — nombre de usuario (login)
 *   USUARIO_CONTRASEANIA varchar(36) — NOTA: debe ampliarse a varchar(64)
 *                                      para almacenar el hash SHA-256 (64 hex chars)
 *
 * Encriptación de contraseña:
 *   Algoritmo: SHA-256 (hexadecimal, minúsculas, 64 caracteres)
 *   IMPORTANTE: La columna USUARIO_CONTRASEANIA debe ser varchar(64).
 *   Ejecutar en MySQL:
 *     ALTER TABLE USUARIO MODIFY USUARIO_CONTRASEANIA varchar(64) NOT NULL;
 *
 * @author MONSTER GR07
 */
public class Usuario {

    private String usuarioId;
    private String rolId;
    private String usuarioNombre;
    private String usuarioApellido;
    private String usuarioTelefono;
    private String usuarioUsuario;
    private String usuarioContraseania; // hash SHA-256 hex (64 chars)

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores
    // ─────────────────────────────────────────────────────────────────────────

    public Usuario() {}

    public Usuario(String usuarioId, String rolId, String usuarioNombre,
                   String usuarioApellido, String usuarioTelefono,
                   String usuarioUsuario, String usuarioContraseania) {
        this.usuarioId           = usuarioId;
        this.rolId               = rolId;
        this.usuarioNombre       = usuarioNombre;
        this.usuarioApellido     = usuarioApellido;
        this.usuarioTelefono     = usuarioTelefono;
        this.usuarioUsuario      = usuarioUsuario;
        this.usuarioContraseania = usuarioContraseania;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Getters / Setters
    // ─────────────────────────────────────────────────────────────────────────

    public String getUsuarioId() { return usuarioId; }
    public void setUsuarioId(String usuarioId) { this.usuarioId = usuarioId; }

    public String getRolId() { return rolId; }
    public void setRolId(String rolId) { this.rolId = rolId; }

    public String getUsuarioNombre() { return usuarioNombre; }
    public void setUsuarioNombre(String usuarioNombre) { this.usuarioNombre = usuarioNombre; }

    public String getUsuarioApellido() { return usuarioApellido; }
    public void setUsuarioApellido(String usuarioApellido) { this.usuarioApellido = usuarioApellido; }

    public String getUsuarioTelefono() { return usuarioTelefono; }
    public void setUsuarioTelefono(String usuarioTelefono) { this.usuarioTelefono = usuarioTelefono; }

    public String getUsuarioUsuario() { return usuarioUsuario; }
    public void setUsuarioUsuario(String usuarioUsuario) { this.usuarioUsuario = usuarioUsuario; }

    public String getUsuarioContraseania() { return usuarioContraseania; }
    public void setUsuarioContraseania(String usuarioContraseania) { this.usuarioContraseania = usuarioContraseania; }

    // ─────────────────────────────────────────────────────────────────────────
    //  Object
    // ─────────────────────────────────────────────────────────────────────────

    @Override
    public String toString() {
        return "Usuario{usuarioId='" + usuarioId + "', usuarioUsuario='" + usuarioUsuario
                + "', rolId='" + rolId + "'}";
    }
}
