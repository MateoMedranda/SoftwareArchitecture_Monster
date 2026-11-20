package ec.edu.monster.controladores;

import ec.edu.monster.modelos.Modelos.*;
import ec.edu.monster.servicios.ClienteSoapEureka;
import java.math.BigDecimal;
import java.util.List;

/**
 * Controlador MVC: concentra estado de sesión y casos de uso.
 */
public final class BancaControlador {

    private final ClienteSoapEureka s = new ClienteSoapEureka();
    private Sesion sesion;

    public Sesion login(String u, String c) throws Exception {
        return sesion = s.login(u, c);
    }

    public Sesion sesion() {
        return sesion;
    }

    public List<Cuenta> cuentas() throws Exception {
        return s.cuentas(sesion.token());
    }

    public java.util.Map<String, String> disponibilidades() throws Exception {
        return s.disponibilidades(sesion.token());
    }

    public List<Cliente> clientesParaCuenta() throws Exception {
        return s.clientesParaCuenta(sesion.token());
    }

    public Cuenta crearCuentaParaUsuario(String usuarioId) throws Exception {
        return s.crearCuentaParaUsuario(sesion.token(), usuarioId);
    }

    public List<Movimiento> movimientos(String c) throws Exception {
        return s.movimientos(sesion.token(), c);
    }

    public void transferir(String o, String d, BigDecimal m) throws Exception {
        s.transferir(sesion.token(), o, d, m);
    }

    public void movimiento(String t, String c, BigDecimal m) throws Exception {
        s.movimiento(sesion.token(), t, c, m);
    }

    public void reversar(String id) throws Exception {
        s.reversar(sesion.token(), id);
    }

    public Perfil perfil() throws Exception {
        return s.perfil(sesion.token(), sesion.usuarioId());
    }

    public void cerrarSesion() throws Exception {
        if (sesion != null) {
            s.logout(sesion.token());
        }
        sesion = null;
    }

    public void invalidarSesion() {
        sesion = null;
    }
}
