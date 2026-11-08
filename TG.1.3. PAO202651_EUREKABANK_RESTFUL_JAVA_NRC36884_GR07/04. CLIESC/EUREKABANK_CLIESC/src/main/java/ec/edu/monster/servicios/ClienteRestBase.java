package ec.edu.monster.servicios;

import ec.edu.monster.configuracion.ConfiguracionRest;
import java.io.*;
import java.math.BigDecimal;
import java.net.*;
import java.nio.charset.StandardCharsets;
import java.util.*;

/** Transporte HTTP/JSON compartido por el cliente de escritorio. */
final class ClienteRestBase {
    String base() { return ConfiguracionRest.baseUrl(); }
    String call(String method, String path, String token, String body) throws Exception {
        HttpURLConnection connection = (HttpURLConnection) new URL(base() + path).openConnection();
        connection.setRequestMethod(method); connection.setConnectTimeout(8000); connection.setReadTimeout(30000);
        connection.setRequestProperty("Accept", "application/json");
        if (token != null) connection.setRequestProperty("Authorization", "Bearer " + token);
        if (body != null) { connection.setDoOutput(true); connection.setRequestProperty("Content-Type", "application/json"); try (OutputStream output = connection.getOutputStream()) { output.write(body.getBytes(StandardCharsets.UTF_8)); } }
        int status = connection.getResponseCode(); InputStream input = status >= 400 ? connection.getErrorStream() : connection.getInputStream();
        String response = input == null ? "" : new String(input.readAllBytes(), StandardCharsets.UTF_8);
        if (status >= 400) { Map<String,Object> error = object(response); throw new IllegalStateException(text(error, "codigo") + ": " + text(error, "mensaje")); }
        return response;
    }
    String path(String value) { return URLEncoder.encode(value, StandardCharsets.UTF_8); }
    String quote(String value) { return "\"" + value.replace("\\", "\\\\").replace("\"", "\\\"") + "\""; }
    String text(Map<String,Object> value, String key) { Object result = value.get(key); return result == null ? "" : String.valueOf(result); }
    BigDecimal decimal(Map<String,Object> value, String key) { try { return new BigDecimal(text(value, key)); } catch (Exception e) { return BigDecimal.ZERO; } }
    @SuppressWarnings("unchecked") Map<String,Object> object(Object value) { if (value instanceof String) value = Json.parse((String) value); if (!(value instanceof Map)) throw new IllegalArgumentException("Respuesta JSON inválida."); return (Map<String,Object>) value; }
    @SuppressWarnings("unchecked") List<Map<String,Object>> array(Object value) { if (value == null) return List.of(); if (value instanceof String) value = Json.parse((String) value); if (!(value instanceof List)) throw new IllegalArgumentException("Se esperaba un arreglo JSON."); return (List<Map<String,Object>>) value; }

    private static final class Json {
        private final String source; private int index;
        private Json(String source) { this.source = source; }
        static Object parse(String source) { Json parser = new Json(source); Object result = parser.value(); parser.space(); if (parser.index != source.length()) throw new IllegalArgumentException("JSON inválido"); return result; }
        private Object value() { space(); if (index >= source.length()) throw new IllegalArgumentException("JSON vacío"); char character = source.charAt(index); if (character == '{') return object(); if (character == '[') return array(); if (character == '"') return string(); if (source.startsWith("null", index)) { index += 4; return null; } if (source.startsWith("true", index)) { index += 4; return true; } if (source.startsWith("false", index)) { index += 5; return false; } int start = index; while (index < source.length() && "-+.0123456789eE".indexOf(source.charAt(index)) >= 0) index++; return new BigDecimal(source.substring(start, index)); }
        private Map<String,Object> object() { Map<String,Object> result = new LinkedHashMap<>(); expect('{'); if (is('}')) { index++; return result; } do { String key = string(); expect(':'); result.put(key, value()); } while (optional(',')); expect('}'); return result; }
        private List<Object> array() { List<Object> result = new ArrayList<>(); expect('['); if (is(']')) { index++; return result; } do { result.add(value()); } while (optional(',')); expect(']'); return result; }
        private String string() { expect('"'); StringBuilder result = new StringBuilder(); while (source.charAt(index) != '"') { char character = source.charAt(index++); if (character == '\\') { char escaped = source.charAt(index++); if (escaped == 'u') result.append((char) Integer.parseInt(source.substring(index, index += 4), 16)); else result.append(escaped == 'n' ? '\n' : escaped == 'r' ? '\r' : escaped == 't' ? '\t' : escaped); } else result.append(character); } index++; return result.toString(); }
        private void space() { while (index < source.length() && Character.isWhitespace(source.charAt(index))) index++; }
        private boolean is(char character) { space(); return index < source.length() && source.charAt(index) == character; }
        private void expect(char character) { if (!is(character)) throw new IllegalArgumentException("JSON inválido"); index++; }
        private boolean optional(char character) { if (!is(character)) return false; index++; return true; }
    }
}
