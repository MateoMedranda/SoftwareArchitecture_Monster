package ec.edu.monster.servicios;

import ec.edu.monster.configuracion.ConfiguracionRest;
import ec.edu.monster.modelos.Modelos.*;
import java.io.*;
import java.math.*;
import java.net.*;
import java.nio.charset.*;
import java.util.*;
import org.json.*;

/**
 * Cliente REST/JSON para Android; debe invocarse fuera del hilo principal.
 */
public final class ClienteRestEureka {

    public Sesion login(String u, String p) throws Exception {
        JSONObject x = o(call("POST", "/auth/login", null, new JSONObject().put("usuario", u).put("contrasenia", p).toString()));
        return new Sesion(s(x, "token"), s(x, "usuarioId"), s(x, "usuarioNombre"), s(x, "rolNombre"));
    }

    public List<Cuenta> cuentas(String t) throws Exception {
        return cuentas(a(call("GET", "/cuentas", t, null)));
    }

    public Map<String, String> disponibilidades(String t) throws Exception {
        Map<String, String> r = new HashMap<>();
        JSONArray estados = a(call("GET", "/cuentas/disponibilidades", t, null));
        for (int i = 0; i < estados.length(); i++) {
            JSONObject x = estados.getJSONObject(i);
            r.put(s(x, "cuentaId"), s(x, "estado"));
        }
        return r;
    }

    public List<Cliente> clientesParaCuenta(String t) throws Exception {
        JSONArray a = a(call("GET", "/cuentas/clientes", t, null));
        List<Cliente> r = new ArrayList<>();
        for (int i = 0; i < a.length(); i++) {
            JSONObject x = a.getJSONObject(i);
            r.add(new Cliente(s(x, "usuarioId"), s(x, "nombreCompleto"), s(x, "usuario")));
        }
        return r;
    }

    public Cuenta crearCuentaParaUsuario(String t, String id) throws Exception {
        return cuenta(o(call("POST", "/cuentas/para-usuario/" + path(id), t, null)));
    }

    public Perfil perfil(String t, String id) throws Exception {
        JSONObject p = o(call("GET", "/usuarios/" + path(id) + "/perfil", t, null)), u = p.getJSONObject("usuario");
        return new Perfil(s(u, "usuarioNombre") + " " + s(u, "usuarioApellido"), s(u, "usuarioUsuario"), s(u, "usuarioTelefono"), cuentas(p.optJSONArray("cuentas")));
    }

    public List<Movimiento> movimientos(String t, String c) throws Exception {
        JSONArray a = a(call("GET", "/transacciones?cuentaId=" + path(c), t, null));
        List<Movimiento> r = new ArrayList<>();
        for (int i = 0; i < a.length(); i++) {
            JSONObject x = a.getJSONObject(i);
            r.add(new Movimiento(s(x, "transaccionId"), s(x, "transaccionTipo"), d(x, "transaccionSaldo"), s(x, "transaccionCreadoEn")));
        }
        return r;
    }

    public void transferir(String t, String o, String d, BigDecimal m) throws Exception {
        call("POST", "/transacciones/transferencias", t, new JSONObject().put("cuentaOrigenId", o).put("cuentaDestinoId", d).put("monto", m).toString());
    }

    public void movimiento(String t, String tipo, String c, BigDecimal m) throws Exception {
        call("POST", "/transacciones/movimientos", t, new JSONObject().put("tipo", tipo).put("cuentaId", c).put("monto", m).toString());
    }

    public void reversar(String t, String id) throws Exception {
        call("POST", "/transacciones/" + path(id) + "/reversa", t, null);
    }

    private static String call(String m, String p, String t, String b) throws Exception {
        HttpURLConnection c = (HttpURLConnection) new URL(ConfiguracionRest.baseUrl() + p).openConnection();
        c.setRequestMethod(m);
        c.setConnectTimeout(8000);
        c.setReadTimeout(30000);
        c.setRequestProperty("Accept", "application/json");
        if (t != null) {
            c.setRequestProperty("Authorization", "Bearer " + t);
        }
        if (b != null) {
            c.setDoOutput(true);
            c.setRequestProperty("Content-Type", "application/json");
            try (OutputStream z = c.getOutputStream()) {
                z.write(b.getBytes(StandardCharsets.UTF_8));
            }
        }
        int code = c.getResponseCode();
        InputStream in = code >= 400 ? c.getErrorStream() : c.getInputStream();
        String r = in == null ? "" : new String(in.readAllBytes(), StandardCharsets.UTF_8);
        if (code >= 400) {
            JSONObject e = o(r);
            throw new IllegalStateException(s(e, "codigo") + ": " + s(e, "mensaje"));
        }
        return r;
    }

    private static JSONObject o(String s) throws JSONException {
        return new JSONObject(s);
    }

    private static JSONArray a(String s) throws JSONException {
        return new JSONArray(s);
    }

    private static String s(JSONObject x, String k) {
        return x.optString(k, "");
    }

    private static BigDecimal d(JSONObject x, String k) {
        try {
            return new BigDecimal(x.optString(k, "0"));
        } catch (Exception e) {
            return BigDecimal.ZERO;
        }
    }

    private static Cuenta cuenta(JSONObject x) {
        String su = s(x, "sucursalNombre");
        return new Cuenta(s(x, "cuentaId"), s(x, "cuentaNumero"), d(x, "cuentaSaldo"), s(x, "cuentaEstado"), su.isEmpty() ? s(x, "sucursalId") : su);
    }

    private static List<Cuenta> cuentas(JSONArray a) throws JSONException {
        List<Cuenta> r = new ArrayList<>();
        if (a != null) {
            for (int i = 0; i < a.length(); i++) {
                r.add(cuenta(a.getJSONObject(i)));
            }
        }
        return r;
    }

    private static String path(String s) {
        return URLEncoder.encode(s, StandardCharsets.UTF_8);
    }
}
