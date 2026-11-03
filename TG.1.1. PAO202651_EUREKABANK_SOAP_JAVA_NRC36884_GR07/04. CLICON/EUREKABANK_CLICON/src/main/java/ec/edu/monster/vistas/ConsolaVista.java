package ec.edu.monster.vistas;

import ec.edu.monster.controladores.BancaControlador;
import ec.edu.monster.modelos.Modelos.*;
import java.math.BigDecimal;
import java.util.*;

/**
 * Vista de terminal: no contiene llamadas SOAP.
 */
public final class ConsolaVista {

    private final Scanner in = new Scanner(System.in);
    private final BancaControlador c = new BancaControlador();

    public void ejecutar() {
        cabecera();
        while (true) {
            try {
                login();
                if (menu()) {
                    return;
                }
            } catch (Exception e) {
                error(limpiar(e.getMessage()));
            }
        }
    }

    private void login() throws Exception {
        System.out.print("\n  Usuario: ");
        String u = in.nextLine().trim();
        System.out.print("  Contraseña: ");
        String p = in.nextLine();
        if (u.isEmpty() || p.trim().isEmpty()) {
            throw new IllegalArgumentException("Usuario y contraseña son obligatorios.");
        }
        Sesion s = c.iniciar(u, p);
        mensaje("Bienvenido/a, " + s.nombre() + ".");
    }

    private boolean menu() {
        while (true) {
            boolean esCliente = "CLIENTE".equalsIgnoreCase(c.sesion().rol());
            String opciones = esCliente ? "\n+------------------------ EUREKABANK -------------------------+\n|  1. Mis cuentas       2. Movimientos     3. Transferir      |\n|  6. Mi perfil                                             |\n|  0. Salir                                                    |\n+--------------------------------------------------------------+" : "\n+------------------------ EUREKABANK -------------------------+\n|  1. Mis cuentas       2. Movimientos     3. Transferir      |\n|  4. Ingreso/retiro    5. Reversar        6. Mi perfil       |\n|  7. Crear cuenta (ADMIN/CAJERO)                              |\n|  0. Salir                                                    |\n+--------------------------------------------------------------+";
            System.out.println(opciones);
            System.out.print("  Elige una opción: ");
            try {
                switch (in.nextLine().trim()) {
                    case "1" ->
                        cuentas();
                    case "2" ->
                        movimientos();
                    case "3" ->
                        transferir();
                    case "4" -> {
                        if (esCliente) {
                            error("Opción no disponible para CLIENTE.");
                        } else {
                            movimiento();
                        }
                    }
                    case "5" -> {
                        if (esCliente) {
                            error("Opción no disponible para CLIENTE.");
                        } else {
                            reversa();
                        }
                    }
                    case "6" ->
                        perfil();
                    case "7" -> {
                        if (esCliente) {
                            error("Opción no disponible para CLIENTE.");
                        } else {
                            crearCuenta();
                        }
                    }
                    case "0" -> {
                        mensaje("Sesión finalizada. Hasta pronto.");
                        return true;
                    }
                    default ->
                        error("Opción no válida.");
                }
            } catch (Exception e) {
                String detalle = limpiar(e.getMessage());
                error(detalle);
                if (esErrorSesion(detalle)) {
                    return false;
                }
            }
        }
    }

    private void cuentas() throws Exception {
        List<Cuenta> cs = c.cuentas();
        Map<String, String> estados = c.disponibilidades();
        System.out.println("\n  CUENTAS DISPONIBLES");
        for (Cuenta x : cs) {
            System.out.printf("  • %s  |  $ %,.2f  |  %s  |  %s  |  %s%n", x.numero(), x.saldo(), x.estado(), estados.getOrDefault(x.id(), "DISPONIBLE"), x.sucursal());
        }
    }

    private void movimientos() throws Exception {
        System.out.print("  Número de cuenta: ");
        for (Movimiento x : c.movimientos(cuentaValida(in.nextLine(), "Número de cuenta"))) {
            System.out.printf("  %s | %-18s | $ %,.2f | %s%n", x.id(), x.tipo(), x.monto(), x.fecha());
        }
    }

    private void transferir() throws Exception {
        System.out.print("  Número de cuenta origen: ");
        String o = cuentaValida(in.nextLine(), "Cuenta origen");
        System.out.print("  Número de cuenta destino: ");
        String d = cuentaValida(in.nextLine(), "Cuenta destino");
        if (o.equals(d)) {
            throw new IllegalArgumentException("Las cuentas deben ser diferentes.");
        }
        System.out.print("  Monto: ");
        mensaje(c.transferir(o, d, montoValido(in.nextLine())).mensaje());
    }

    private void movimiento() throws Exception {
        System.out.print("  Tipo [INGRESO/RETIRO]: ");
        String t = in.nextLine().trim().toUpperCase();
        if (!t.equals("INGRESO") && !t.equals("RETIRO")) {
            throw new IllegalArgumentException("Tipo inválido: use INGRESO o RETIRO.");
        }
        System.out.print("  Número de cuenta: ");
        String id = cuentaValida(in.nextLine(), "Número de cuenta");
        System.out.print("  Monto: ");
        mensaje(c.registrar(t, id, montoValido(in.nextLine())).mensaje());
    }

    private void reversa() throws Exception {
        System.out.print("  ID de transacción: ");
        mensaje(c.reversar(in.nextLine()).mensaje());
    }

    private void perfil() throws Exception {
        Perfil p = c.perfil();
        System.out.println("\n  PERFIL\n  " + p.nombre() + "\n  @" + p.usuario() + "  ·  " + p.telefono());
    }

    private void crearCuenta() throws Exception {
        List<Cliente> clientes = c.clientesParaCuenta();
        if (clientes.isEmpty()) {
            error("No existen clientes disponibles.");
            return;
        }
        System.out.println("\n  SELECCIONA EL CLIENTE TITULAR");
        for (int i = 0; i < clientes.size(); i++) {
            Cliente x = clientes.get(i);
            System.out.printf("  %d. %s (@%s)%n", i + 1, x.nombre(), x.usuario());
        }
        System.out.print("  Cliente: ");
        int opcion = Integer.parseInt(in.nextLine().trim());
        if (opcion < 1 || opcion > clientes.size()) {
            error("Cliente no válido.");
            return;
        }
        Cliente cliente = clientes.get(opcion - 1);
        Cuenta cuenta = c.crearCuentaParaUsuario(cliente.id());
        mensaje("Cuenta " + cuenta.numero() + " creada y asignada a " + cliente.nombre() + ".");
    }

    private void cabecera() {
        System.out.println("\n+--------------------------------------------------------------+\n|              E U R E K A B A N K                             |\n|                    BANCA DIGITAL                              |\n+--------------------------------------------------------------+");
    }

    private boolean esErrorSesion(String mensaje) {
        String valor = mensaje.toLowerCase();
        return valor.contains("token inválido") || valor.contains("token invalido") || valor.contains("token expirado") || valor.contains("token revocado") || valor.contains("sesión expirada") || valor.contains("sesion expirada");
    }

    private String cuentaValida(String valor, String etiqueta) {
        String cuenta = valor == null ? "" : valor.trim();
        if (!cuenta.matches("\\d{10}")) {
            throw new IllegalArgumentException(etiqueta + " debe tener 10 dígitos.");
        }
        return cuenta;
    }

    private BigDecimal montoValido(String valor) {
        try {
            BigDecimal monto = new BigDecimal(valor == null ? "" : valor.trim());
            if (monto.compareTo(BigDecimal.ZERO) <= 0) {
                throw new NumberFormatException();
            }
            return monto;
        } catch (NumberFormatException e) {
            throw new IllegalArgumentException("El monto debe ser mayor a cero.");
        }
    }

    private void mensaje(String m) {
        System.out.println("\n  [OK] " + m);
    }

    private void error(String m) {
        System.out.println("\n  [ERROR] " + m);
    }

    private String limpiar(String s) {
        return s == null ? "Error de comunicación." : s.replaceAll("(?s)<[^>]*>", "").trim();
    }
}
