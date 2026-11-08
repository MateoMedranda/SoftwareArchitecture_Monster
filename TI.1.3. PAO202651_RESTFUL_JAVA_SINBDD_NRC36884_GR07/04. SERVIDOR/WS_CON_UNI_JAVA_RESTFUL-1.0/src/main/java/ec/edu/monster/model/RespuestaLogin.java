package ec.edu.monster.model;

import java.io.Serializable;
import org.eclipse.microprofile.openapi.annotations.enums.SchemaType;
import org.eclipse.microprofile.openapi.annotations.media.Schema;

@Schema(description = "Objeto devuelto tras login exitoso")
public class RespuestaLogin implements Serializable {

    @Schema(description = "Token de sesión UUID", type = SchemaType.STRING,
            example = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJNT05TVEVSIiwiaWF0IjoxNzkyMzYzOTQ3LCJleHAiOjE3OTIzNjc1NDd9.XDEFozqTZdyNDYXHbU3wF7KxHW2JjF33oKtd9DksvvDMThwI6FClKCQPhVh9QWOPADeL779DBbDkbVM8MYP7rw"
    )
    private String token;
    
    @Schema(description = "Duración del token en segundos", type = SchemaType.INTEGER, example = "3600")
    private long expiraEn;

    public RespuestaLogin() {}

    public RespuestaLogin(String token, long expiraEn) {
        this.token = token;
        this.expiraEn = expiraEn;
    }

    public String getToken() {
        return token;
    }

    public void setToken(String token) {
        this.token = token;
    }

    public long getExpiraEn() {
        return expiraEn;
    }

    public void setExpiraEn(long expiraEn) {
        this.expiraEn = expiraEn;
    }
}