package ec.edu.monster.modelos;

import java.math.BigDecimal;
import java.util.List;

public final class Modelos {

    private Modelos() {
    }

    public record Sesion(String token, String usuarioId, String nombre, String mensaje, String rol) {

    }

    public record Cuenta(String id, String numero, BigDecimal saldo, String estado, String sucursal) {

    }

    public record Cliente(String id, String nombre, String usuario) {

    }

    public record Movimiento(String id, String cuentaId, BigDecimal monto, String tipo, String fecha) {

    }

    public record Resultado(String id, String mensaje) {

    }

    public record Perfil(String nombre, String usuario, String telefono, List<Cuenta> cuentas) {

    }
}
