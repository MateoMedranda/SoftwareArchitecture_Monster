package ec.edu.monster.seguridad;

public final class AdministradorTokensCliente {

    private static volatile String tokenActual;

    private AdministradorTokensCliente() {
    }

    public static void guardarToken(String token) {
        tokenActual = token;
    }

    public static String obtenerToken() {
        return tokenActual;
    }

    public static void limpiar() {
        tokenActual = null;
    }
}
