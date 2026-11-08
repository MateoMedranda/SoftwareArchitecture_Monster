package ec.edu.monster.controladores;

import ec.edu.monster.modelos.Modelos.*;
import ec.edu.monster.servicios.ClienteRestEureka;
import java.math.BigDecimal;
import java.util.List;

/**
 * Controlador MVC móvil: conserva sesión y orquesta los casos de uso.
 */
public final class BancaControlador {

    private final ClienteRestEureka rest = new ClienteRestEureka();
    private Sesion sesion;

    public Sesion login(String u, String p) throws Exception {
        return sesion = rest.login(u, p);
    }

    public Sesion sesion() {
        return sesion;
    }

    public List<Cuenta> cuentas() throws Exception {
        return rest.cuentas(sesion.token);
    }

    public java.util.Map<String, String> disponibilidades() throws Exception {
        return rest.disponibilidades(sesion.token);
    }

    public List<Cliente> clientesParaCuenta() throws Exception {
        return rest.clientesParaCuenta(sesion.token);
    }

    public Cuenta crearCuentaParaUsuario(String usuarioId) throws Exception {
        return rest.crearCuentaParaUsuario(sesion.token, usuarioId);
    }

    public Perfil perfil() throws Exception {
        return rest.perfil(sesion.token, sesion.usuarioId);
    }

    public List<Movimiento> movimientos(String id) throws Exception {
        return rest.movimientos(sesion.token, id);
    }

    public void transferir(String o, String d, BigDecimal m) throws Exception {
        rest.transferir(sesion.token, o, d, m);
    }

    public void movimiento(String tipo, String cuenta, BigDecimal monto) throws Exception {
        rest.movimiento(sesion.token, tipo, cuenta, monto);
    }

    public void reversar(String transaccionId) throws Exception {
        rest.reversar(sesion.token, transaccionId);
    }

    public void salir() {
        sesion = null;
    }
}
