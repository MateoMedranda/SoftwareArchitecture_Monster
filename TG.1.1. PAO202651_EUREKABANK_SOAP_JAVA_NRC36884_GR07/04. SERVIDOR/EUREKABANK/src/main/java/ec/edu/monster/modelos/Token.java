package ec.edu.monster.modelos;

/**
 * Modelo de la tabla TOKEN.
 *
 * Columnas BDD:
 *   TOKEN_ID     varchar(36)  PK
 *   TOKEN_TOKEN  varchar(124) — almacena el JTI (UUID) del JWT correspondiente
 *   TOKEN_ESTADO varchar(24)  — ACTIVO | INACTIVO
 *   USUARIO_ID   varchar(36)  FK → USUARIO
 *
 * Nota: TOKEN_TOKEN guarda el JTI del JWT (UUID de 36 chars),
 * no el token JWT completo, para encajar en varchar(124).
 *
 * @author MONSTER GR07
 */
public class Token {

    private String tokenId;
    private String tokenToken;   // JTI del JWT
    private String tokenEstado;  // "ACTIVO" | "INACTIVO"
    private String usuarioId;

    // ─────────────────────────────────────────────────────────────────────────
    //  Constructores
    // ─────────────────────────────────────────────────────────────────────────

    public Token() {}

    public Token(String tokenId, String tokenToken, String tokenEstado, String usuarioId) {
        this.tokenId     = tokenId;
        this.tokenToken  = tokenToken;
        this.tokenEstado = tokenEstado;
        this.usuarioId   = usuarioId;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Getters / Setters
    // ─────────────────────────────────────────────────────────────────────────

    public String getTokenId() { return tokenId; }
    public void setTokenId(String tokenId) { this.tokenId = tokenId; }

    public String getTokenToken() { return tokenToken; }
    public void setTokenToken(String tokenToken) { this.tokenToken = tokenToken; }

    public String getTokenEstado() { return tokenEstado; }
    public void setTokenEstado(String tokenEstado) { this.tokenEstado = tokenEstado; }

    public String getUsuarioId() { return usuarioId; }
    public void setUsuarioId(String usuarioId) { this.usuarioId = usuarioId; }

    // ─────────────────────────────────────────────────────────────────────────
    //  Object
    // ─────────────────────────────────────────────────────────────────────────

    @Override
    public String toString() {
        return "Token{tokenId='" + tokenId + "', tokenEstado='" + tokenEstado
                + "', usuarioId='" + usuarioId + "'}";
    }
}
