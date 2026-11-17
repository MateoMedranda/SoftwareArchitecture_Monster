package ec.edu.monster.servicios;

import ec.edu.monster.configuracion.ConfiguracionRest;
import ec.edu.monster.modelos.Modelos.*;
import java.io.*;
import java.math.BigDecimal;
import java.net.*;
import java.nio.charset.StandardCharsets;
import java.util.*;

/**
 * Cliente HTTP/JSON del API REST. Las vistas no dependen del protocolo.
 */
public final class ClienteRestEureka {

    public Sesion login(String usuario, String contrasenia) throws Exception {
        Map<String, Object> r = objeto(llamar("POST", "/auth/login", null, json(Map.of("usuario", usuario, "contrasenia", contrasenia))));
        return new Sesion(texto(r, "token"), texto(r, "usuarioId"), texto(r, "usuarioNombre"), texto(r, "mensaje"), texto(r, "rolNombre"));
    }

    public List<Cuenta> cuentas(String token) throws Exception {
        return cuentas(arreglo(llamar("GET", "/cuentas", token, null)));
    }

    public Map<String, String> disponibilidades(String token) throws Exception {
        Map<String, String> r = new HashMap<>();
        for (Object x : arreglo(llamar("GET", "/cuentas/disponibilidades", token, null))) {
            Map<String, Object> o = objeto(x);
            r.put(texto(o, "cuentaId"), texto(o, "estado"));
        }
        return r;
    }

    public List<Cliente> clientesParaCuenta(String token) throws Exception {
        List<Cliente> r = new ArrayList<>();
        for (Object x : arreglo(llamar("GET", "/cuentas/clientes", token, null))) {
            Map<String, Object> o = objeto(x);
            r.add(new Cliente(texto(o, "usuarioId"), texto(o, "nombreCompleto"), texto(o, "usuario")));
        }
        return r;
    }

    public Cuenta crearCuentaParaUsuario(String token, String usuarioId) throws Exception {
        return cuenta(objeto(llamar("POST", "/cuentas/para-usuario/" + url(usuarioId), token, null)));
    }

    public List<Movimiento> movimientos(String token, String cuentaId) throws Exception {
        List<Movimiento> r = new ArrayList<>();
        for (Object x : arreglo(llamar("GET", "/transacciones?cuentaId=" + url(cuentaId), token, null))) {
            Map<String, Object> o = objeto(x);
            r.add(new Movimiento(texto(o, "transaccionId"), texto(o, "cuentaId"), numero(o, "transaccionSaldo"), texto(o, "transaccionTipo"), texto(o, "transaccionCreadoEn")));
        }
        return r;
    }

    public Resultado transferencia(String token, String origen, String destino, BigDecimal monto) throws Exception {
        Map<String, Object> r = objeto(llamar("POST", "/transacciones/transferencias", token, json(Map.of("cuentaOrigenId", origen, "cuentaDestinoId", destino, "monto", monto))));
        return new Resultado(texto(r, "transferenciaId"), "Transferencia completada correctamente.");
    }

    public Resultado movimiento(String token, String tipo, String cuenta, String monto) throws Exception {
        return movimiento(token, tipo, cuenta, new BigDecimal(monto));
    }

    public Resultado movimiento(String token, String tipo, String cuenta, BigDecimal monto) throws Exception {
        Map<String, Object> r = objeto(llamar("POST", "/transacciones/movimientos", token, json(Map.of("tipo", tipo, "cuentaId", cuenta, "monto", monto))));
        return new Resultado(texto(r, "transaccionId"), "Movimiento registrado de forma auditable.");
    }

    public Resultado reversar(String token, String id) throws Exception {
        Map<String, Object> r = objeto(llamar("POST", "/transacciones/" + url(id) + "/reversa", token, null));
        return new Resultado(texto(r, "transaccionId"), "Reversa registrada correctamente.");
    }

    public Perfil perfil(String token, String usuarioId) throws Exception {
        Map<String, Object> p = objeto(llamar("GET", "/usuarios/" + url(usuarioId) + "/perfil", token, null)), u = objeto(p.get("usuario"));
        return new Perfil(texto(u, "usuarioNombre") + " " + texto(u, "usuarioApellido"), texto(u, "usuarioUsuario"), texto(u, "usuarioTelefono"), cuentas((List<?>) p.getOrDefault("cuentas", List.of())));
    }

    private String llamar(String metodo, String ruta, String token, String cuerpo) throws Exception {
        HttpURLConnection c = (HttpURLConnection) new URL(ConfiguracionRest.baseUrl() + ruta).openConnection();
        c.setConnectTimeout(8000);
        c.setReadTimeout(30000);
        c.setRequestMethod(metodo);
        c.setRequestProperty("Accept", "application/json");
        if (token != null) {
            c.setRequestProperty("Authorization", "Bearer " + token);
        }
        if (cuerpo != null) {
            c.setDoOutput(true);
            c.setRequestProperty("Content-Type", "application/json; charset=utf-8");
            try (OutputStream o = c.getOutputStream()) {
                o.write(cuerpo.getBytes(StandardCharsets.UTF_8));
            }
        }
        int estado = c.getResponseCode();
        InputStream in = estado >= 400 ? c.getErrorStream() : c.getInputStream();
        String respuesta = in == null ? "" : new String(in.readAllBytes(), StandardCharsets.UTF_8);
        if (estado >= 400) {
            try {
                Map<String, Object> e = objeto(respuesta);
                throw new IllegalStateException(texto(e, "codigo") + ": " + texto(e, "mensaje"));
            } catch (IllegalArgumentException ignorada) {
                throw new IOException("HTTP " + estado + ": " + respuesta);
            }
        }
        return respuesta;
    }

    private static List<Cuenta> cuentas(List<?> origen) {
        List<Cuenta> r = new ArrayList<>();
        for (Object x : origen) {
            r.add(cuenta(objeto(x)));
        }
        return r;
    }

    private static Cuenta cuenta(Map<String, Object> o) {
        String sucursal = texto(o, "sucursalNombre");
        return new Cuenta(texto(o, "cuentaId"), texto(o, "cuentaNumero"), numero(o, "cuentaSaldo"), texto(o, "cuentaEstado"), sucursal.isBlank() ? texto(o, "sucursalId") : sucursal);
    }

    private static String texto(Map<String, Object> o, String campo) {
        Object v = o.get(campo);
        return v == null ? "" : String.valueOf(v);
    }

    private static BigDecimal numero(Map<String, Object> o, String campo) {
        try {
            return new BigDecimal(texto(o, campo));
        } catch (Exception e) {
            return BigDecimal.ZERO;
        }
    }

    @SuppressWarnings("unchecked")
    private static Map<String, Object> objeto(Object x) {
        if (x instanceof String) {
            x = Json.leer((String) x);
        }
        if (!(x instanceof Map)) {
            throw new IllegalArgumentException("Respuesta JSON inválida.");
        }
        return (Map<String, Object>) x;
    }

    @SuppressWarnings("unchecked")
    private static List<?> arreglo(String x) {
        Object r = Json.leer(x);
        if (!(r instanceof List)) {
            throw new IllegalArgumentException("Se esperaba un arreglo JSON.");
        }
        return (List<?>) r;
    }

    private static String url(String v) throws UnsupportedEncodingException {
        return URLEncoder.encode(v, StandardCharsets.UTF_8);
    }

    private static String json(Map<String, ?> valores) {
        StringBuilder s = new StringBuilder("{");
        for (var e : valores.entrySet()) {
            if (s.length() > 1) {
                s.append(',');
            }
            s.append('"').append(e.getKey()).append("\":");
            Object v = e.getValue();
            if (v instanceof Number || v instanceof Boolean) {
                s.append(v);
            } else {
                s.append('"').append(String.valueOf(v).replace("\\", "\\\\").replace("\"", "\\\"")).append('"');
            }
        }
        return s.append('}').toString();
    }

    /**
     * Parser JSON pequeño y sin dependencias externas para los DTO del
     * proyecto.
     */
    private static final class Json {

        private final String s;
        private int i;

        private Json(String s) {
            this.s = s;
        }

        static Object leer(String s) {
            Json p = new Json(s);
            Object v = p.valor();
            p.blancos();
            if (p.i != s.length()) {
                throw new IllegalArgumentException("JSON inválido");
            }
            return v;
        }

        private Object valor() {
            blancos();
            if (i >= s.length()) {
                throw new IllegalArgumentException("JSON vacío");
            }
            char c = s.charAt(i);
            if (c == '{') {
                return objeto();
            }
            if (c == '[') {
                return lista();
            }
            if (c == '\"') {
                return cadena();
            }
            if (s.startsWith("true", i)) {
                i += 4;
                return true;
            }
            if (s.startsWith("false", i)) {
                i += 5;
                return false;
            }
            if (s.startsWith("null", i)) {
                i += 4;
                return null;
            }
            int a = i;
            while (i < s.length() && "-+.0123456789eE".indexOf(s.charAt(i)) >= 0) {
                i++;
            }
            try {
                return new BigDecimal(s.substring(a, i));
            } catch (Exception e) {
                throw new IllegalArgumentException("Número JSON inválido");
            }
        }

        private Map<String, Object> objeto() {
            Map<String, Object> r = new LinkedHashMap<>();
            esperar('{');
            blancos();
            if (ver('}')) {
                i++;
                return r;
            }
            while (true) {
                String k = cadena();
                esperar(':');
                r.put(k, valor());
                blancos();
                if (ver('}')) {
                    i++;
                    return r;
                }
                esperar(',');
            }
        }

        private List<Object> lista() {
            List<Object> r = new ArrayList<>();
            esperar('[');
            blancos();
            if (ver(']')) {
                i++;
                return r;
            }
            while (true) {
                r.add(valor());
                blancos();
                if (ver(']')) {
                    i++;
                    return r;
                }
                esperar(',');
            }
        }

        private String cadena() {
            esperar('\"');
            StringBuilder r = new StringBuilder();
            while (i < s.length()) {
                char c = s.charAt(i++);
                if (c == '\"') {
                    return r.toString();
                }
                if (c != '\\') {
                    r.append(c);
                    continue;
                }
                if (i >= s.length()) {
                    break;
                }
                char e = s.charAt(i++);
                switch (e) {
                    case '\"':
                    case '\\':
                    case '/':
                        r.append(e);
                        break;
                    case 'b':
                        r.append('\b');
                        break;
                    case 'f':
                        r.append('\f');
                        break;
                    case 'n':
                        r.append('\n');
                        break;
                    case 'r':
                        r.append('\r');
                        break;
                    case 't':
                        r.append('\t');
                        break;
                    case 'u':
                        r.append((char) Integer.parseInt(s.substring(i, i += 4), 16));
                        break;
                    default:
                        throw new IllegalArgumentException("Escape JSON inválido");
                }
            }
            throw new IllegalArgumentException("Cadena JSON incompleta");
        }

        private void esperar(char c) {
            blancos();
            if (!ver(c)) {
                throw new IllegalArgumentException("JSON inválido");
            }
            i++;
        }

        private boolean ver(char c) {
            return i < s.length() && s.charAt(i) == c;
        }

        private void blancos() {
            while (i < s.length() && Character.isWhitespace(s.charAt(i))) {
                i++;
            }
        }
    }
}
