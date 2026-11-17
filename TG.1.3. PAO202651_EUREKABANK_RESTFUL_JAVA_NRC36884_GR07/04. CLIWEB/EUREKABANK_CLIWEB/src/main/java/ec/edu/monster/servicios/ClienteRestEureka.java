package ec.edu.monster.servicios;

import ec.edu.monster.configuracion.ConfiguracionRest;
import ec.edu.monster.modelos.*;
import jakarta.json.*;
import java.io.*;
import java.math.*;
import java.net.*;
import java.nio.charset.*;
import java.util.*;

/**
 * Infraestructura HTTP/JSON para los beans JSF.
 */
public final class ClienteRestEureka {

    public record Login(String token, String usuarioId, String nombre, String rol) {}

    public Login login(String u, String p) throws Exception {
        JsonObject x = obj(call("POST", "/auth/login", null, "{\"usuario\":" + q(u) + ",\"contrasenia\":" + q(p) + "}"));
        return new Login(s(x, "token"), s(x, "usuarioId"), s(x, "usuarioNombre"), s(x, "rolNombre"));
    }

    public List<CuentaVista> cuentas(String t) throws Exception {
        List<CuentaVista> r = new ArrayList<>();
        for (JsonValue v : arr(call("GET", "/cuentas", t, null))) {
            r.add(cuenta(v.asJsonObject()));
        }
        return r;
    }

    public Map<String, String> disponibilidades(String t) throws Exception {
        Map<String, String> r = new HashMap<>();
        for (JsonValue v : arr(call("GET", "/cuentas/disponibilidades", t, null))) {
            JsonObject x = v.asJsonObject();
            r.put(s(x, "cuentaId"), s(x, "estado"));
        }
        return r;
    }

    public List<ClienteCuentaVista> clientesParaCuenta(String t) throws Exception {
        List<ClienteCuentaVista> r = new ArrayList<>();
        for (JsonValue v : arr(call("GET", "/cuentas/clientes", t, null))) {
            JsonObject x = v.asJsonObject();
            r.add(new ClienteCuentaVista(s(x, "usuarioId"), s(x, "nombreCompleto"), s(x, "usuario")));
        }
        return r;
    }

    public CuentaVista crearCuentaParaUsuario(String t, String id) throws Exception {
        return cuenta(obj(call("POST", "/cuentas/para-usuario/" + path(id), t, null)));
    }

    public PerfilVista perfil(String t, String id) throws Exception {
        JsonObject u = obj(call("GET", "/usuarios/" + path(id) + "/perfil", t, null)).getJsonObject("usuario");
        return new PerfilVista(s(u, "usuarioNombre") + " " + s(u, "usuarioApellido"), s(u, "usuarioUsuario"), s(u, "usuarioTelefono"));
    }

    public List<MovimientoVista> movimientos(String t, String c) throws Exception {
        List<MovimientoVista> r = new ArrayList<>();
        for (JsonValue v : arr(call("GET", "/transacciones?cuentaId=" + path(c), t, null))) {
            JsonObject x = v.asJsonObject();
            r.add(new MovimientoVista(s(x, "transaccionId"), s(x, "transaccionTipo"), d(x, "transaccionSaldo"), s(x, "transaccionCreadoEn"), s(x, "transaccionCreadoPor")));
        }
        return r;
    }

    public void transferir(String t, String o, String d, BigDecimal m) throws Exception {
        call("POST", "/transacciones/transferencias", t, "{\"cuentaOrigenId\":" + q(o) + ",\"cuentaDestinoId\":" + q(d) + ",\"monto\":" + m + "}");
    }

    public void movimiento(String t, String tipo, String c, BigDecimal m) throws Exception {
        call("POST", "/transacciones/movimientos", t, "{\"tipo\":" + q(tipo) + ",\"cuentaId\":" + q(c) + ",\"monto\":" + m + "}");
    }

    public void reversar(String t, String id) throws Exception {
        call("POST", "/transacciones/" + path(id) + "/reversa", t, null);
    }

    private static String call(String m, String p, String token, String body) throws Exception {
        HttpURLConnection c = (HttpURLConnection) new URL(ConfiguracionRest.baseUrl() + p).openConnection();
        c.setRequestMethod(m);
        c.setConnectTimeout(8000);
        c.setReadTimeout(30000);
        c.setRequestProperty("Accept", "application/json");
        if (token != null) {
            c.setRequestProperty("Authorization", "Bearer " + token);
        }
        if (body != null) {
            c.setDoOutput(true);
            c.setRequestProperty("Content-Type", "application/json");
            try (OutputStream o = c.getOutputStream()) {
                o.write(body.getBytes(StandardCharsets.UTF_8));
            }
        }
        int code = c.getResponseCode();
        InputStream i = code >= 400 ? c.getErrorStream() : c.getInputStream();
        String r = i == null ? "" : new String(i.readAllBytes(), StandardCharsets.UTF_8);
        if (code >= 400) {
            JsonObject e = obj(r);
            throw new IllegalStateException(s(e, "codigo") + ": " + s(e, "mensaje"));
        }
        return r;
    }

    private static JsonObject obj(String x) {
        try (JsonReader r = Json.createReader(new StringReader(x))) {
            return r.readObject();
        }
    }

    private static JsonArray arr(String x) {
        try (JsonReader r = Json.createReader(new StringReader(x))) {
            return r.readArray();
        }
    }

    private static String s(JsonObject x, String k) {
        return x.getString(k, "");
    }

    private static BigDecimal d(JsonObject x, String k) {
        return x.getJsonNumber(k) == null ? BigDecimal.ZERO : x.getJsonNumber(k).bigDecimalValue();
    }

    private static CuentaVista cuenta(JsonObject x) {
        String su = s(x, "sucursalNombre");
        return new CuentaVista(s(x, "cuentaId"), s(x, "cuentaNumero"), d(x, "cuentaSaldo"), s(x, "cuentaEstado"), su.isBlank() ? s(x, "sucursalId") : su);
    }

    private static String path(String s) {
        return URLEncoder.encode(s, StandardCharsets.UTF_8);
    }

    private static String q(String s) {
        return "\"" + s.replace("\\", "\\\\").replace("\"", "\\\"") + "\"";
    }
}
