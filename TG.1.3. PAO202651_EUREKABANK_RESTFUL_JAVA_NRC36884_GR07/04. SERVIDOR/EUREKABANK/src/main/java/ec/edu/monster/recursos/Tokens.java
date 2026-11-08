package ec.edu.monster.recursos;

/**
 * Centraliza la extracción del JWT del encabezado HTTP estándar.
 */
final class Tokens {

    private Tokens() {
    }

    static String extraer(String authorization) {
        if (authorization == null) {
            return null;
        }
        return authorization.regionMatches(true, 0, "Bearer ", 0, 7)
                ? authorization.substring(7).trim() : authorization.trim();
    }
}
