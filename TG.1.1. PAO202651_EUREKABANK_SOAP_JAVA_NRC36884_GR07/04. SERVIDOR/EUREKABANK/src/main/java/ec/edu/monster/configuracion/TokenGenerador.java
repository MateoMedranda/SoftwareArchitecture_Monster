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
 *   - TOKEN_CREADO_EN registra la fecha/hora de emisión del token.
 *   - La validación verifica: firma válida + no expirado + JTI en estado ACTIVO
 *     + antigüedad menor o igual a 30 minutos según TOKEN_CREADO_EN.
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

    /** Expiracion del token: 30 minutos */
    private static final long EXPIRACION_MS = 30L * 60 * 1000;

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
     * @throws SQLException si no se puede registrar el JTI en la tabla TOKEN
     */
    public String generarToken(String usuarioId, String rolId) throws SQLException {
        String jti = UUID.randomUUID().toString();
        Date ahora = new Date();
        Date expiracion = new Date(ahora.getTime() + EXPIRACION_MS);

        String jwt = Jwts.builder()
                .id(jti)
                .subject(usuarioId)
                .claim("rol", rolId)
                .issuedAt(ahora)
                .expiration(expiracion)
                .signWith(obtenerClave())
                .compact();

        registrarOActualizarJtiEnBDD(jti, usuarioId);
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
            // Verificar antiguedad (30 min) contra BDD
            if (!esTokenVigente(jti)) {
                LOG.warning("[TokenGenerador] Token expirado por antiguedad (>30 min) — JTI: " + jti);
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

    private void registrarOActualizarJtiEnBDD(String jti, String usuarioId) throws SQLException {
        String select = "SELECT TOKEN_ID FROM TOKEN WHERE USUARIO_ID = ? LIMIT 1";
        String update = "UPDATE TOKEN SET TOKEN_TOKEN = ?, TOKEN_ESTADO = 'ACTIVO', TOKEN_CREADO_EN = NOW() WHERE USUARIO_ID = ?";
        String insert = "INSERT INTO TOKEN (TOKEN_ID, TOKEN_TOKEN, TOKEN_ESTADO, USUARIO_ID, TOKEN_CREADO_EN) VALUES (?, ?, 'ACTIVO', ?, NOW())";
        try (Connection conn = conexionBDD.getConexion()) {
            try (PreparedStatement ps = conn.prepareStatement(select)) {
                ps.setString(1, usuarioId);
                try (ResultSet rs = ps.executeQuery()) {
                    if (rs.next()) {
                        try (PreparedStatement ups = conn.prepareStatement(update)) {
                            ups.setString(1, jti);
                            ups.setString(2, usuarioId);
                            ups.executeUpdate();
                        }
                        return;
                    }
                }
            }
            try (PreparedStatement ips = conn.prepareStatement(insert)) {
                ips.setString(1, UUID.randomUUID().toString());
                ips.setString(2, jti);
                ips.setString(3, usuarioId);
                ips.executeUpdate();
            }
        } catch (SQLException e) {
            LOG.severe("[TokenGenerador] Error al registrar/actualizar JTI en BDD: " + e.getMessage());
            throw e;
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

    private boolean esTokenVigente(String jti) {
        // La antigüedad se calcula con la hora del servidor de BDD para evitar
        // desfases entre la zona horaria de MySQL y la de Payara.
        String sql = "SELECT TIMESTAMPDIFF(SECOND, TOKEN_CREADO_EN, NOW()) AS EDAD "
                   + "FROM TOKEN WHERE TOKEN_TOKEN = ?";
        try (Connection conn = conexionBDD.getConexion();
             PreparedStatement ps = conn.prepareStatement(sql)) {
            ps.setString(1, jti);
            ResultSet rs = ps.executeQuery();
            if (rs.next()) {
                long edadSegundos = rs.getLong("EDAD");
                return edadSegundos <= (EXPIRACION_MS / 1000);
            }
        } catch (SQLException e) {
            LOG.severe("[TokenGenerador] Error al verificar antiguedad: " + e.getMessage());
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
