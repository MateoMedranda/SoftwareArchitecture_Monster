package ec.edu.monster.controladores;

import ec.edu.monster.configuracion.TokenGenerador;
import ec.edu.monster.servicios.LoginExcepcion;
import ec.edu.monster.modelos.RespuestaError;
import io.jsonwebtoken.Claims;
import java.util.Optional;

/**
 * Filtro/helper de validación de tokens JWT para servicios SOAP protegidos.
 *
 * Esta clase provee métodos estáticos reutilizables para verificar
 * la autenticidad y permisos de un token JWT antes de ejecutar
 * la lógica de negocio de cualquier operación SOAP protegida.
 *
 * Patrón de uso en un servicio SOAP protegido:
 * <pre>
 *   {@literal @}EJB
 *   private TokenGenerador tokenGenerador;
 *
 *   {@literal @}WebMethod
 *   public MiRespuesta miOperacion(String token, ...) throws LoginExcepcion {
 *       // Verificar token — lanza LoginExcepcion si no es válido
 *       Claims claims = TokenFiltro.verificar(token, tokenGenerador);
 *
 *       // Opcional: verificar rol específico
 *       // Claims claims = TokenFiltro.verificarConRol(token, "ADMIN", tokenGenerador);
 *
 *       // Obtener ID del usuario del token
 *       String usuarioId = claims.getSubject();
 *       String rolId     = claims.get("rol", String.class);
 *
 *       // ... lógica de negocio protegida ...
 *   }
 * </pre>
 *
 * @author MONSTER GR07
 */
public final class TokenFiltro {

    private TokenFiltro() {
        // Clase utilitaria — no instanciar
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  API pública
    // ─────────────────────────────────────────────────────────────────────────

    /**
     * Verifica que el token JWT sea válido y esté activo en la BDD.
     *
     * @param token          Token JWT recibido en la petición SOAP
     * @param tokenGenerador Instancia del EJB TokenGenerador (inyectada por el servicio)
     * @return Claims del token si es válido
     * @throws LoginExcepcion con código TOKEN_REQUERIDO si el token es nulo/vacío
     * @throws LoginExcepcion con código TOKEN_INVALIDO si el token es inválido o expirado
     */
    public static Claims verificar(String token, TokenGenerador tokenGenerador)
            throws LoginExcepcion {

        if (token == null || token.trim().isEmpty()) {
            throw new LoginExcepcion(
                "Token de autenticacion no proporcionado",
                new RespuestaError(
                    "TOKEN_REQUERIDO",
                    "Se requiere un token de autenticación para acceder a esta operación. " +
                    "Por favor, inicie sesión primero.",
                    null
                )
            );
        }

        Optional<Claims> optClaims = tokenGenerador.validarToken(token.trim());

        if (optClaims.isEmpty()) {
            throw new LoginExcepcion(
                "Token invalido, expirado o revocado",
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
     * Verifica el token y además comprueba que el usuario tenga el rol requerido.
     *
     * @param token          Token JWT recibido en la petición SOAP
     * @param rolRequerido   Nombre/ID del rol que debe tener el usuario
     * @param tokenGenerador Instancia del EJB TokenGenerador
     * @return Claims del token si es válido y tiene el rol requerido
     * @throws LoginExcepcion con código TOKEN_REQUERIDO si el token es nulo/vacío
     * @throws LoginExcepcion con código TOKEN_INVALIDO si el token es inválido o expirado
     * @throws LoginExcepcion con código ACCESO_DENEGADO si el token no tiene el rol requerido
     */
    public static Claims verificarConRol(String token, String rolRequerido,
                                          TokenGenerador tokenGenerador)
            throws LoginExcepcion {

        Claims claims = verificar(token, tokenGenerador);

        if (rolRequerido != null && !rolRequerido.isBlank()) {
            String rolDelToken = claims.get("rol", String.class);
            if (!rolRequerido.equals(rolDelToken)) {
                throw new LoginExcepcion(
                    "Acceso denegado — rol insuficiente",
                    new RespuestaError(
                        "ACCESO_DENEGADO",
                        "No tiene los permisos necesarios para realizar esta operación.",
                        "Rol requerido: " + rolRequerido
                    )
                );
            }
        }

        return claims;
    }

    /**
     * Extrae el ID del usuario desde los Claims del token.
     * Método de conveniencia para no llamar directamente a claims.getSubject().
     *
     * @param claims Claims obtenidos de verificar() o verificarConRol()
     * @return ID del usuario (USUARIO_ID de la BDD)
     */
    public static String obtenerUsuarioId(Claims claims) {
        return claims.getSubject();
    }

    /**
     * Extrae el ID del rol desde los Claims del token.
     *
     * @param claims Claims obtenidos de verificar() o verificarConRol()
     * @return ID del rol (ROL_ID de la BDD)
     */
    public static String obtenerRolId(Claims claims) {
        return claims.get("rol", String.class);
    }
}
