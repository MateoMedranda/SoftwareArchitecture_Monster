package ec.edu.monster.servicios;

import ec.edu.monster.modelos.RespuestaError;
import ec.edu.monster.modelos.RespuestaLogin;
import ec.edu.monster.configuracion.TokenGenerador;
import ec.edu.monster.conexion.ConexionBDD;
import io.jsonwebtoken.Claims;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import jakarta.jws.WebResult;
import jakarta.jws.WebService;
import jakarta.jws.soap.SOAPBinding;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Optional;
import java.util.logging.Logger;

/**
 * Servicio SOAP de autenticación de EUREKABANK.
 *
 * Expone tres operaciones:
 *
 *   1. login(usuario, contrasenia)
 *      → Autentica al usuario contra la BDD (SHA-256).
 *        Genera un JWT y lo registra en la tabla TOKEN.
 *        Retorna RespuestaLogin con el token.
 *
 *   2. logout(token)
 *      → Revoca el token (JTI → INACTIVO en tabla TOKEN).
 *        Operación protegida: requiere token válido.
 *
 *   3. verificarToken(token)
 *      → Valida que el token sea correcto y esté activo.
 *        Retorna los datos del usuario si es válido.
 *
 * URL del WSDL (Payara):
 *   https://localhost:8181/EUREKABANK/LoginServicio?wsdl
 *
 * Encriptación de contraseña:
 *   Algoritmo: SHA-256 (hexadecimal, minúsculas).
 *   La columna USUARIO_CONTRASEANIA debe contener el hash SHA-256
 *   de la contraseña del usuario (64 caracteres hex).
 *
 *   ⚠ IMPORTANTE — Actualizar columna antes de desplegar:
 *   ALTER TABLE USUARIO MODIFY USUARIO_CONTRASEANIA varchar(64) NOT NULL;
 *
 *   Para obtener el hash SHA-256 de una contraseña (ejemplo en MySQL):
 *   SELECT LOWER(SHA2('MONSTER9', 256));
 *
 * @author MONSTER GR07
 */
@Stateless
@WebService(
    serviceName     = "LoginServicio",
    portName        = "LoginServicioPort",
    targetNamespace = "http://servicios.monster.edu.ec/"
)
@SOAPBinding(style = SOAPBinding.Style.DOCUMENT)
public class LoginServicio {

    private static final Logger LOG = Logger.getLogger(LoginServicio.class.getName());

    @EJB
    private ConexionBDD conexionBDD;

    @EJB
    private TokenGenerador tokenGenerador;

    // ─────────────────────────────────────────────────────────────────────────
    //  Operación 1: login
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Autentica a un usuario con su nombre de usuario y contraseña.
     *
     * @param usuario    Nombre de usuario (USUARIO_USUARIO en BDD)
     * @param contrasenia Contraseña en texto plano (se hashea con SHA-256 antes de comparar)
     * @return RespuestaLogin con token JWT y datos del usuario
     * @throws LoginExcepcion con RespuestaError en caso de credenciales inválidas u otros errores
     */
    @WebMethod(operationName = "login")
    @WebResult(name = "respuestaLogin")
    public RespuestaLogin login(
            @WebParam(name = "usuario")     String usuario,
            @WebParam(name = "contrasenia") String contrasenia
    ) throws LoginExcepcion {

        // ── Validar parámetros de entrada ──
        if (usuario == null || usuario.trim().isEmpty()) {
            throw new LoginExcepcion(
                "Campo usuario obligatorio",
                new RespuestaError(
                    "CAMPO_REQUERIDO",
                    "El campo 'usuario' es obligatorio y no puede estar vacío.",
                    "usuario"
                )
            );
        }
        if (contrasenia == null || contrasenia.trim().isEmpty()) {
            throw new LoginExcepcion(
                "Campo contrasenia obligatorio",
                new RespuestaError(
                    "CAMPO_REQUERIDO",
                    "El campo 'contrasenia' es obligatorio y no puede estar vacío.",
                    "contrasenia"
                )
            );
        }

        // ── Hashear la contraseña ingresada con SHA-256 ──
        String contraseniaHash = calcularSha256(contrasenia.trim());

        // ── Buscar usuario en la BDD ──
        String sql = "SELECT USUARIO_ID, ROL_ID, USUARIO_NOMBRE, USUARIO_APELLIDO, "
                   + "       USUARIO_CONTRASEANIA "
                   + "FROM   USUARIO "
                   + "WHERE  USUARIO_USUARIO = ?";

        try (Connection conn = conexionBDD.getConexion();
             PreparedStatement ps = conn.prepareStatement(sql)) {

            ps.setString(1, usuario.trim());
            ResultSet rs = ps.executeQuery();

            if (!rs.next()) {
                // El usuario no existe — no revelar si es usuario o contraseña
                LOG.warning("[LoginServicio] Login fallido — usuario no encontrado: " + usuario);
                throw new LoginExcepcion(
                    "Usuario no encontrado en el sistema",
                    new RespuestaError(
                        "USUARIO_NO_ENCONTRADO",
                        "El nombre de usuario ingresado no está registrado en el sistema.",
                        null
                    )
                );
            }

            String contraseniaAlmacenada = rs.getString("USUARIO_CONTRASEANIA");

            // ── Comparar hashes (case-insensitive para flexibilidad) ──
            if (!contraseniaHash.equalsIgnoreCase(contraseniaAlmacenada)) {
                LOG.warning("[LoginServicio] Login fallido — contraseña incorrecta para: " + usuario);
                throw new LoginExcepcion(
                    "Credenciales incorrectas",
                    new RespuestaError(
                        "CREDENCIALES_INVALIDAS",
                        "La contraseña ingresada es incorrecta. Por favor, verifique sus datos.",
                        null
                    )
                );
            }

            // ── Autenticación exitosa — generar JWT ──
            String usuarioId = rs.getString("USUARIO_ID");
            String rolId     = rs.getString("ROL_ID");
            String nombre    = rs.getString("USUARIO_NOMBRE");
            String apellido  = rs.getString("USUARIO_APELLIDO");

            String jwtToken = tokenGenerador.generarToken(usuarioId, rolId);

            LOG.info("[LoginServicio] Login exitoso para usuario: " + usuario + " | ID: " + usuarioId);

            return new RespuestaLogin(
                true,
                jwtToken,
                usuarioId,
                rolId,
                nombre + " " + apellido,
                "Autenticación exitosa. Bienvenido, " + nombre + ". El token expira en 8 horas."
            );

        } catch (LoginExcepcion e) {
            throw e; // relanzar excepciones propias sin modificar
        } catch (SQLException e) {
            LOG.severe("[LoginServicio] Error de base de datos en login: " + e.getMessage());
            throw new LoginExcepcion(
                "Error al acceder a la base de datos",
                new RespuestaError(
                    "ERROR_BASE_DATOS",
                    "No se pudo procesar su solicitud por un error interno. Intente nuevamente.",
                    e.getMessage()
                ),
                e
            );
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Operación 2: logout (protegida — requiere token válido)
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Cierra la sesión del usuario revocando el token JWT.
     * El token queda registrado como INACTIVO en la tabla TOKEN.
     *
     * @param token Token JWT obtenido al hacer login
     * @return true si el logout fue exitoso
     * @throws LoginExcepcion si el token es inválido, expirado o ya fue revocado
     */
    @WebMethod(operationName = "logout")
    @WebResult(name = "exitoso")
    public boolean logout(
            @WebParam(name = "token") String token
    ) throws LoginExcepcion {

        // Verificar que el token sea válido antes de revocar
        verificarTokenOLanzar(token, "logout");

        boolean revocado = tokenGenerador.revocarToken(token);
        if (!revocado) {
            throw new LoginExcepcion(
                "No se pudo cerrar la sesión",
                new RespuestaError(
                    "LOGOUT_FALLIDO",
                    "No se pudo cerrar la sesión. Es posible que el token ya haya sido revocado.",
                    null
                )
            );
        }

        LOG.info("[LoginServicio] Logout exitoso.");
        return true;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Operación 3: verificarToken (protegida — requiere token válido)
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Verifica si un token JWT es válido y retorna la información del usuario.
     * Útil para que los clientes comprueben el estado del token antes de operar.
     *
     * @param token Token JWT a verificar
     * @return RespuestaLogin con los datos del usuario si el token es válido
     * @throws LoginExcepcion si el token es inválido o expirado
     */
    @WebMethod(operationName = "verificarToken")
    @WebResult(name = "respuestaToken")
    public RespuestaLogin verificarToken(
            @WebParam(name = "token") String token
    ) throws LoginExcepcion {

        Claims claims = verificarTokenOLanzar(token, "verificarToken");

        RespuestaLogin respuesta = new RespuestaLogin();
        respuesta.setExitoso(true);
        respuesta.setToken(token);
        respuesta.setUsuarioId(claims.getSubject());
        respuesta.setRolId(claims.get("rol", String.class));
        respuesta.setMensaje("Token válido y activo.");
        return respuesta;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Métodos privados
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Valida el token o lanza LoginExcepcion con el mensaje adecuado.
     * Centraliza la validación para todas las operaciones protegidas.
     *
     * @param token     Token JWT a validar
     * @param operacion Nombre de la operación que requiere el token (para logs)
     * @return Claims del token si es válido
     * @throws LoginExcepcion si el token es nulo, inválido o expirado
     */
    private Claims verificarTokenOLanzar(String token, String operacion) throws LoginExcepcion {
        if (token == null || token.trim().isEmpty()) {
            throw new LoginExcepcion(
                "Token requerido en operacion: " + operacion,
                new RespuestaError(
                    "TOKEN_REQUERIDO",
                    "Se requiere un token de autenticación para acceder a esta operación.",
                    "Operacion: " + operacion
                )
            );
        }

        Optional<Claims> optClaims = tokenGenerador.validarToken(token.trim());
        if (optClaims.isEmpty()) {
            throw new LoginExcepcion(
                "Token invalido o expirado en operacion: " + operacion,
                new RespuestaError(
                    "TOKEN_INVALIDO",
                    "El token proporcionado es inválido, ha expirado o fue revocado. " +
                    "Por favor, inicie sesión nuevamente.",
                    null
                )
            );
        }

        return optClaims.get();
    }

    /**
     * Calcula el hash SHA-256 de un texto y lo retorna como string hexadecimal (64 chars).
     *
     * @param texto Texto a hashear (contraseña en plano)
     * @return Hash SHA-256 en hexadecimal minúsculas (64 caracteres)
     * @throws LoginExcepcion si el algoritmo SHA-256 no está disponible (nunca debería ocurrir)
     */
    private String calcularSha256(String texto) throws LoginExcepcion {
        try {
            MessageDigest digest = MessageDigest.getInstance("SHA-256");
            byte[] hashBytes = digest.digest(texto.getBytes(StandardCharsets.UTF_8));

            StringBuilder sb = new StringBuilder();
            for (byte b : hashBytes) {
                sb.append(String.format("%02x", b));
            }
            return sb.toString(); // 64 caracteres hex

        } catch (NoSuchAlgorithmException e) {
            // SHA-256 siempre está disponible en Java — este catch es preventivo
            throw new LoginExcepcion(
                "Error interno de cifrado",
                new RespuestaError(
                    "ERROR_CIFRADO",
                    "Error interno al procesar las credenciales. Contacte al administrador.",
                    e.getMessage()
                ),
                e
            );
        }
    }
}
