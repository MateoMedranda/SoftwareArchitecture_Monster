package ec.edu.monster.configuracion;

import ec.edu.monster.conexion.ConexionBDD;
import io.jsonwebtoken.Claims;
import io.jsonwebtoken.ExpiredJwtException;
import io.jsonwebtoken.JwtException;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.security.Keys;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Arrays;
import java.util.Date;
import java.util.Optional;
import java.util.UUID;
import java.util.logging.Logger;
import javax.crypto.SecretKey;

/**
 * Servicio para generación, validación y revocación de tokens JWT (HS256).
 *
 * Estrategia de tokens:
 *   - Se genera un JWT firmado con HMAC-SHA256.
 *   - El JWT incluye un claim "jti" (JWT ID) que es un UUID único.
 *   - El JTI se almacena en la tabla TOKEN de la BDD (TOKEN_TOKEN = jti).
 *   - La validación verifica: firma válida + no expirado + JTI en estado ACTIVO.
 *   - La revocación cambia el estado del JTI a INACTIVO (logout / blacklist).
 *
 * Configuración (via .development.env):
 *   JWT_SECRETO  — clave secreta para firmar/verificar (mínimo 32 caracteres)
 *
 * Nota: TOKEN_TOKEN almacena el JTI (UUID, 36 chars) que encaja en varchar(124).
 *
 * @author MONSTER GR07
 */
@Stateless
public class TokenGenerador {

    private static final Logger LOG = Logger.getLogger(TokenGenerador.class.getName());

    /** Expiración del token: 8 horas */
    private static final long EXPIRACION_MS = 8L * 60 * 60 * 1000;

    /**
     * Secreto por defecto (CAMBIAR en producción via JWT_SECRETO en .development.env).
     * Debe tener al menos 32 caracteres para HS256.
     */
    private static final String SECRETO_DEFECTO =
            "EUREKABANK_JWT_SECRETO_SEGURO_2026_MONSTER_NRC36884_GR07!!";

    @EJB
    private ConexionBDD conexionBDD;

    // ─────────────────────────────────────────────────────────────────────────
    //  API pública
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Genera un token JWT para el usuario autenticado y lo registra en la BDD.
     *
     * @param usuarioId ID del usuario (subject del JWT)
     * @param rolId     ID del rol del usuario (claim personalizado)
     * @return JWT compacto: header.payload.signature
     */
    public String generarToken(String usuarioId, String rolId) {
        String jti = UUID.randomUUID().toString();
        Date ahora = new Date();
        Date expiracion = new Date(ahora.getTime() + EXPIRACION_MS);

        String jwt = Jwts.builder()
                .id(jti)                         // jti claim — identificador único del token
                .subject(usuarioId)              // sub claim — ID del usuario
                .claim("rol", rolId)             // claim personalizado — ID del rol
                .issuedAt(ahora)                 // iat claim — fecha de emisión
                .expiration(expiracion)          // exp claim — fecha de expiración (8 h)
                .signWith(obtenerClave())         // firma HMAC-SHA256
                .compact();

        registrarJtiEnBDD(jti, usuarioId);
        LOG.info("[TokenGenerador] Token generado para usuario: " + usuarioId + " | JTI: " + jti);
        return jwt;
    }

    /**
     * Valida un token JWT: verifica firma, expiración y estado activo en BDD.
     *
     * @param token JWT recibido
     * @return Optional con los Claims si el token es completamente válido,
     *         Optional.empty() si es inválido, expirado o revocado
     */
    public Optional<Claims> validarToken(String token) {
        if (token == null || token.isBlank()) {
            return Optional.empty();
        }
        try {
            Claims claims = Jwts.parser()
                    .verifyWith(obtenerClave())
                    .build()
                    .parseSignedClaims(token)
                    .getPayload();

            // Verificar que el JTI no haya sido revocado
            String jti = claims.getId();
            if (!esJtiActivo(jti)) {
                LOG.warning("[TokenGenerador] Token revocado o no registrado — JTI: " + jti);
                return Optional.empty();
            }
            return Optional.of(claims);

        } catch (ExpiredJwtException e) {
            LOG.warning("[TokenGenerador] Token expirado.");
            return Optional.empty();
        } catch (JwtException e) {
            LOG.warning("[TokenGenerador] Token JWT invalido: " + e.getMessage());
            return Optional.empty();
        }
    }

    /**
     * Revoca un token cambiando su JTI a INACTIVO en la BDD (logout).
     *
     * @param token JWT a revocar
     * @return true si se revocó con éxito, false en caso contrario
     */
    public boolean revocarToken(String token) {
        if (token == null || token.isBlank()) {
            return false;
        }
        try {
            // Parsear sin verificar firma para obtener el JTI incluso si ya expiró
            Claims claims = Jwts.parser()
                    .verifyWith(obtenerClave())
                    .build()
                    .parseSignedClaims(token)
                    .getPayload();

            String jti = claims.getId();
            boolean revocado = marcarJtiInactivo(jti);
            LOG.info("[TokenGenerador] Token revocado — JTI: " + jti + " | Exito: " + revocado);
            return revocado;

        } catch (JwtException e) {
            LOG.warning("[TokenGenerador] No se puede revocar token invalido: " + e.getMessage());
            return false;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Métodos privados
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Construye la SecretKey para HMAC-SHA256.
     * Lee JWT_SECRETO del .development.env; si no existe, usa el secreto por defecto.
     * La clave se completa a 32 bytes si es más corta.
     */
    private SecretKey obtenerClave() {
        String secreto = DotEnvLoader.obtener("JWT_SECRETO", SECRETO_DEFECTO);
        byte[] bytes = secreto.getBytes(StandardCharsets.UTF_8);
        // HMAC-SHA256 requiere al menos 256 bits (32 bytes)
        if (bytes.length < 32) {
            bytes = Arrays.copyOf(bytes, 32);
        }
        return Keys.hmacShaKeyFor(bytes);
    }

    private void registrarJtiEnBDD(String jti, String usuarioId) {
        String sql = "INSERT INTO TOKEN (TOKEN_ID, TOKEN_TOKEN, TOKEN_ESTADO, USUARIO_ID) "
                   + "VALUES (?, ?, 'ACTIVO', ?)";
        try (Connection conn = conexionBDD.getConexion();
             PreparedStatement ps = conn.prepareStatement(sql)) {
            ps.setString(1, UUID.randomUUID().toString());
            ps.setString(2, jti);   // JTI = UUID (36 chars) — cabe en varchar(124)
            ps.setString(3, usuarioId);
            ps.executeUpdate();
        } catch (SQLException e) {
            LOG.severe("[TokenGenerador] Error al registrar JTI en BDD: " + e.getMessage());
        }
    }

    private boolean esJtiActivo(String jti) {
        String sql = "SELECT TOKEN_ESTADO FROM TOKEN WHERE TOKEN_TOKEN = ?";
        try (Connection conn = conexionBDD.getConexion();
             PreparedStatement ps = conn.prepareStatement(sql)) {
            ps.setString(1, jti);
            ResultSet rs = ps.executeQuery();
            if (rs.next()) {
                return "ACTIVO".equalsIgnoreCase(rs.getString("TOKEN_ESTADO"));
            }
        } catch (SQLException e) {
            LOG.severe("[TokenGenerador] Error al verificar JTI: " + e.getMessage());
        }
        return false;
    }

    private boolean marcarJtiInactivo(String jti) {
        String sql = "UPDATE TOKEN SET TOKEN_ESTADO = 'INACTIVO' WHERE TOKEN_TOKEN = ?";
        try (Connection conn = conexionBDD.getConexion();
             PreparedStatement ps = conn.prepareStatement(sql)) {
            ps.setString(1, jti);
            return ps.executeUpdate() > 0;
        } catch (SQLException e) {
            LOG.severe("[TokenGenerador] Error al revocar JTI: " + e.getMessage());
            return false;
        }
    }
}
