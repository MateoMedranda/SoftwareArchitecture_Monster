package ec.edu.monster.security;

import io.jsonwebtoken.Claims;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.security.Keys;
import jakarta.annotation.PostConstruct;
import jakarta.enterprise.context.ApplicationScoped;
import jakarta.inject.Inject;
import org.eclipse.microprofile.config.inject.ConfigProperty;

import javax.crypto.SecretKey;
import java.nio.charset.StandardCharsets;
import java.util.Date;

@ApplicationScoped
public class ProveedorTokenJwt {

    @Inject
    @ConfigProperty(name = "jwt_secret", defaultValue = "dev-secret-key-for-monster-application-change-in-production")
    private String secreto;

    @Inject
    @ConfigProperty(name = "jwt_expiration", defaultValue = "3600000")
    private long expiracionMs;

    private SecretKey claveSecreta;

    public ProveedorTokenJwt() {}

    @PostConstruct
    protected void inicializar() {
        this.claveSecreta = Keys.hmacShaKeyFor(secreto.getBytes(StandardCharsets.UTF_8));
    }

    public String generarToken(String usuario) {
        Date ahora = new Date();
        Date expiracion = new Date(ahora.getTime() + expiracionMs);

        return Jwts.builder()
                .subject(usuario)
                .issuedAt(ahora)
                .expiration(expiracion)
                .signWith(claveSecreta)
                .compact();
    }

    public boolean validarToken(String token) {
        try {
            Jwts.parser()
                    .verifyWith(claveSecreta)
                    .build()
                    .parseSignedClaims(token);
            return true;
        } catch (Exception e) {
            return false;
        }
    }

    public String obtenerUsuario(String token) {
        Claims claims = Jwts.parser()
                .verifyWith(claveSecreta)
                .build()
                .parseSignedClaims(token)
                .getPayload();
        return claims.getSubject();
    }

    public long obtenerTiempoExpiracion() {
        return expiracionMs / 1000;
    }
}