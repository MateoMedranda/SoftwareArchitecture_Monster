package ec.edu.monster.servicios;

import ec.edu.monster.modelos.Modelos.*;
import java.math.BigDecimal;
import java.util.*;

/**
 * Adaptador REST del cliente de escritorio.
 */
public final class ClienteRestEureka {

    private final ClienteRestBase http = new ClienteRestBase();

    public Sesion login(String u, String p) throws Exception {
        var x = http.object(http.call("POST", "/auth/login", null, "{\"usuario\":" + http.quote(u) + ",\"contrasenia\":" + http.quote(p) + "}"));
        return new Sesion(http.text(x, "token"), http.text(x, "usuarioId"), http.text(x, "usuarioNombre"), http.text(x, "rolNombre"));
    }

    public List<Cuenta> cuentas(String t) throws Exception {
        return cuentas(http.array(http.call("GET", "/cuentas", t, null)));
    }

    public Map<String, String> disponibilidades(String t) throws Exception {
        Map<String, String> r = new HashMap<>();
        for (var x : http.array(http.call("GET", "/cuentas/disponibilidades", t, null))) {
            r.put(http.text(x, "cuentaId"), http.text(x, "estado"));
        }
        return r;
    }

    public List<Cliente> clientesParaCuenta(String t) throws Exception {
        List<Cliente> r = new ArrayList<>();
        for (var x : http.array(http.call("GET", "/cuentas/clientes", t, null))) {
            r.add(new Cliente(http.text(x, "usuarioId"), http.text(x, "nombreCompleto"), http.text(x, "usuario")));
        }
        return r;
    }

    public Cuenta crearCuentaParaUsuario(String t, String u) throws Exception {
        return cuenta(http.object(http.call("POST", "/cuentas/para-usuario/" + http.path(u), t, null)));
    }

    public List<Movimiento> movimientos(String t, String c) throws Exception {
        List<Movimiento> r = new ArrayList<>();
        for (var x : http.array(http.call("GET", "/transacciones?cuentaId=" + http.path(c), t, null))) {
            r.add(new Movimiento(http.text(x, "transaccionId"), http.text(x, "cuentaId"), http.decimal(x, "transaccionSaldo"), http.text(x, "transaccionTipo"), http.text(x, "transaccionCreadoEn")));
        }
        return r;
    }

    public void transferir(String t, String o, String d, BigDecimal m) throws Exception {
        http.call("POST", "/transacciones/transferencias", t, "{\"cuentaOrigenId\":" + http.quote(o) + ",\"cuentaDestinoId\":" + http.quote(d) + ",\"monto\":" + m + "}");
    }

    public void movimiento(String t, String tipo, String c, BigDecimal m) throws Exception {
        http.call("POST", "/transacciones/movimientos", t, "{\"tipo\":" + http.quote(tipo) + ",\"cuentaId\":" + http.quote(c) + ",\"monto\":" + m + "}");
    }

    public void reversar(String t, String id) throws Exception {
        http.call("POST", "/transacciones/" + http.path(id) + "/reversa", t, null);
    }

    public Perfil perfil(String t, String id) throws Exception {
        var p = http.object(http.call("GET", "/usuarios/" + http.path(id) + "/perfil", t, null));
        var u = http.object(p.get("usuario"));
        return new Perfil(http.text(u, "usuarioNombre") + " " + http.text(u, "usuarioApellido"), http.text(u, "usuarioUsuario"), http.text(u, "usuarioTelefono"), cuentas(http.array(p.get("cuentas"))));
    }

    public void logout(String t) throws Exception {
        http.call("POST", "/auth/logout", t, null);
    }

    private Cuenta cuenta(Map<String, Object> x) {
        String s = http.text(x, "sucursalNombre");
        return new Cuenta(http.text(x, "cuentaId"), http.text(x, "cuentaNumero"), http.decimal(x, "cuentaSaldo"), http.text(x, "cuentaEstado"), s.isBlank() ? http.text(x, "sucursalId") : s);
    }

    private List<Cuenta> cuentas(List<Map<String, Object>> v) {
        List<Cuenta> r = new ArrayList<>();
        for (var x : v) {
            r.add(cuenta(x));
        }
        return r;
    }
}
