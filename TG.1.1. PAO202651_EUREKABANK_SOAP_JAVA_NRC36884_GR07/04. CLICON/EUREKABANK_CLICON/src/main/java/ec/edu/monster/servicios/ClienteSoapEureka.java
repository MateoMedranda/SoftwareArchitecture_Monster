package ec.edu.monster.servicios;

import ec.edu.monster.configuracion.ConfiguracionSoap;
import ec.edu.monster.modelos.Modelos.*;
import java.io.*;
import java.math.BigDecimal;
import java.net.HttpURLConnection;
import java.net.URL;
import java.nio.charset.StandardCharsets;
import java.util.*;
import javax.xml.XMLConstants;
import javax.xml.parsers.DocumentBuilderFactory;
import org.w3c.dom.*;

/**
 * Adaptador SOAP: la vista y sus controladores no conocen XML ni HTTP.
 */
public final class ClienteSoapEureka {

    private static final String NS = "http://servicios.monster.edu.ec/";

    public Sesion login(String usuario, String contrasenia) throws Exception {
        Document d = llamar("LoginServicio/LoginControlador", "login", null,
                "<usuario>" + esc(usuario) + "</usuario><contrasenia>" + esc(contrasenia) + "</contrasenia>");
        comprobarError(d);
        Element r = primero(d, "respuestaLogin");
        return new Sesion(texto(r, "token"), texto(r, "usuarioId"), texto(r, "usuarioNombre"), texto(r, "mensaje"), texto(r, "rolNombre"));
    }

    public List<Cuenta> cuentas(String token) throws Exception {
        Document d = llamar("CuentaServicio/CuentaControlador", "listarCuentas", token, "");
        comprobarError(d);
        List<Cuenta> salida = new ArrayList<>();
        for (Element e : elementos(d, "return")) {
            salida.add(new Cuenta(texto(e, "cuentaId"), texto(e, "cuentaNumero"), decimal(texto(e, "cuentaSaldo")), texto(e, "cuentaEstado"), sucursal(e)));
        }
        return salida;
    }

    public Map<String, String> disponibilidades(String token) throws Exception {
        Document d = llamar("CuentaServicio/CuentaControlador", "listarDisponibilidades", token, "");
        comprobarError(d);
        Map<String, String> salida = new HashMap<>();
        for (Element e : elementos(d, "disponibilidad")) {
            salida.put(texto(e, "cuentaId"), texto(e, "estado"));
        }
        return salida;
    }

    public List<Cliente> clientesParaCuenta(String token) throws Exception {
        Document d = llamar("CuentaServicio/CuentaControlador", "listarClientesParaCuenta", token, "");
        comprobarError(d);
        List<Cliente> salida = new ArrayList<>();
        for (Element e : elementos(d, "cliente")) {
            salida.add(new Cliente(texto(e, "usuarioId"),
                    texto(e, "nombreCompleto"), texto(e, "usuario")));
        }
        return salida;
    }

    public Cuenta crearCuentaParaUsuario(String token, String usuarioId) throws Exception {
        Document d = llamar("CuentaServicio/CuentaControlador", "crearCuentaParaUsuario", token,
                "<usuarioId>" + esc(usuarioId) + "</usuarioId>");
        comprobarError(d);
        Element e = primero(d, "cuenta");
        return new Cuenta(texto(e, "cuentaId"), texto(e, "cuentaNumero"),
                decimal(texto(e, "cuentaSaldo")), texto(e, "cuentaEstado"), sucursal(e));
    }

    public List<Movimiento> movimientos(String token, String cuentaId) throws Exception {
        Document d = llamar("TransaccionServicio/TransaccionControlador", "listarPorCuenta", token, "<cuentaId>" + esc(cuentaId) + "</cuentaId>");
        comprobarError(d);
        List<Movimiento> salida = new ArrayList<>();
        for (Element e : elementos(d, "transacciones")) {
            salida.add(new Movimiento(texto(e, "transaccionId"), texto(e, "cuentaId"), decimal(texto(e, "transaccionSaldo")), texto(e, "transaccionTipo"), texto(e, "transaccionCreadoEn")));
        }
        return salida;
    }

    public Resultado transferencia(String token, String origen, String destino, BigDecimal monto) throws Exception {
        Document d = llamar("TransaccionServicio/TransaccionControlador", "realizarTransferencia", token, "<cuentaOrigenId>" + esc(origen) + "</cuentaOrigenId><cuentaDestinoId>" + esc(destino) + "</cuentaDestinoId><monto>" + monto.toPlainString() + "</monto>");
        comprobarError(d);
        Element e = primero(d, "transferencia");
        return new Resultado(texto(e, "transferenciaId"), "Transferencia completada correctamente.");
    }

    public Resultado movimiento(String token, String tipo, String cuenta, BigDecimal monto) throws Exception {
        Document d = llamar("TransaccionServicio/TransaccionControlador", "generarTransaccion", token, "<tipo>" + esc(tipo) + "</tipo><cuentaId>" + esc(cuenta) + "</cuentaId><monto>" + monto.toPlainString() + "</monto>");
        comprobarError(d);
        return new Resultado(texto(primero(d, "transaccion"), "transaccionId"), "Movimiento registrado de forma auditable.");
    }

    public Resultado reversar(String token, String id) throws Exception {
        Document d = llamar("TransaccionServicio/TransaccionControlador", "reversarTransaccion", token, "<transaccionId>" + esc(id) + "</transaccionId>");
        comprobarError(d);
        return new Resultado(texto(primero(d, "reversa"), "transaccionId"), "Reversa registrada correctamente.");
    }

    public Perfil perfil(String token, String usuarioId) throws Exception {
        Document d = llamar("UsuarioServicio/UsuarioControlador", "obtenerPerfilBancario", token, "<usuarioId>" + esc(usuarioId) + "</usuarioId>");
        comprobarError(d);
        Element p = primero(d, "perfil"), u = primero(p, "usuario");
        List<Cuenta> cs = new ArrayList<>();
        for (Element e : elementos(p, "cuentas")) {
            cs.add(new Cuenta(texto(e, "cuentaId"), texto(e, "cuentaNumero"), decimal(texto(e, "cuentaSaldo")), texto(e, "cuentaEstado"), sucursal(e)));
        }
        return new Perfil(texto(u, "usuarioNombre") + " " + texto(u, "usuarioApellido"), texto(u, "usuarioUsuario"), texto(u, "usuarioTelefono"), cs);
    }

    private Document llamar(String servicio, String operacion, String token, String cuerpo) throws Exception {
        String header = token == null ? "" : "<ser:token>" + esc(token) + "</ser:token>";
        String xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ser=\"" + NS + "\"><soap:Header>" + header + "</soap:Header><soap:Body><ser:" + operacion + ">" + cuerpo + "</ser:" + operacion + "></soap:Body></soap:Envelope>";
        HttpURLConnection c = (HttpURLConnection) new URL(ConfiguracionSoap.baseUrl() + "/" + servicio).openConnection();
        c.setConnectTimeout(8000);
        c.setReadTimeout(20000);
        c.setRequestMethod("POST");
        c.setDoOutput(true);
        c.setRequestProperty("Content-Type", "text/xml; charset=utf-8");
        c.setRequestProperty("SOAPAction", "");
        try (OutputStream out = c.getOutputStream()) {
            out.write(xml.getBytes(StandardCharsets.UTF_8));
        }
        InputStream in = c.getResponseCode() >= 400 ? c.getErrorStream() : c.getInputStream();
        if (in == null) {
            throw new IOException("El servidor no devolvió respuesta.");
        }
        DocumentBuilderFactory f = DocumentBuilderFactory.newInstance();
        f.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
        f.setAttribute(XMLConstants.ACCESS_EXTERNAL_DTD, "");
        f.setAttribute(XMLConstants.ACCESS_EXTERNAL_SCHEMA, "");
        f.setNamespaceAware(true);
        return f.newDocumentBuilder().parse(in);
    }

    private static void comprobarError(Document d) throws Exception {
        NodeList f = d.getElementsByTagNameNS("*", "faultstring");
        if (f.getLength() > 0) {
            throw new IllegalStateException(f.item(0).getTextContent());
        }
    }

    private static Element primero(Node n, String tag) {
        NodeList l = (n instanceof Document) ? ((Document) n).getElementsByTagNameNS("*", tag) : ((Element) n).getElementsByTagNameNS("*", tag);
        return l.getLength() == 0 ? null : (Element) l.item(0);
    }

    private static List<Element> elementos(Node n, String tag) {
        NodeList l = (n instanceof Document) ? ((Document) n).getElementsByTagNameNS("*", tag) : ((Element) n).getElementsByTagNameNS("*", tag);
        List<Element> r = new ArrayList<>();
        for (int i = 0; i < l.getLength(); i++) {
            r.add((Element) l.item(i));
        }
        return r;
    }

    private static String texto(Element e, String tag) {
        if (e == null) {
            return "";
        }
        NodeList l = e.getElementsByTagNameNS("*", tag);
        return l.getLength() == 0 ? "" : l.item(0).getTextContent();
    }

    private static String sucursal(Element e) {
        String nombre = texto(e, "sucursalNombre");
        return nombre.isBlank() ? texto(e, "sucursalId") : nombre;
    }

    private static BigDecimal decimal(String v) {
        try {
            return new BigDecimal(v);
        } catch (Exception e) {
            return BigDecimal.ZERO;
        }
    }

    private static String esc(String v) {
        return v == null ? "" : v.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace("\"", "&quot;");
    }
}
