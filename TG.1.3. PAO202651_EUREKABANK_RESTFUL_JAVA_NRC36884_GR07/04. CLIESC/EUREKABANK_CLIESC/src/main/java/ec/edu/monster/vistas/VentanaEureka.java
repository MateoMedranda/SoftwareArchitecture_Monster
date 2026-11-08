package ec.edu.monster.vistas;

import ec.edu.monster.controladores.BancaControlador;
import ec.edu.monster.modelos.Modelos.*;
import java.awt.*;
import java.math.BigDecimal;
import java.net.URL;
import java.util.Map;
import java.util.List;
import javax.swing.*;
import javax.swing.border.EmptyBorder;
import javax.swing.table.DefaultTableModel;

/**
 * Interfaz Swing MVC: no realiza llamadas SOAP directamente.
 */
public final class VentanaEureka extends JFrame {

    private final BancaControlador controlador = new BancaControlador();
    private final CardLayout paginas = new CardLayout();
    private final CardLayout secciones = new CardLayout();
    private final JPanel raiz = new JPanel(paginas);
    private final JLabel saludo = new JLabel();
    private final JLabel estadoLogin = new JLabel(" ", SwingConstants.CENTER);
    private final JTextField usuario = new JTextField(24);
    private final JPasswordField contrasenia = new JPasswordField(24);
    private JButton botonIngresar;
    private JToggleButton operacionesNavegacion, cuentasNavegacion;
    private boolean actualizandoCuentas;
    private final DefaultTableModel modeloCuentas = new DefaultTableModel(
            new String[]{"Número de cuenta", "Saldo", "Estado", "Disponibilidad", "Sucursal"}, 0) {
        @Override
        public boolean isCellEditable(int fila, int columna) {
            return false;
        }
    };
    private final DefaultTableModel modeloMovimientos = new DefaultTableModel(
            new String[]{"ID", "Tipo", "Monto", "Fecha"}, 0) {
        @Override
        public boolean isCellEditable(int fila, int columna) {
            return false;
        }
    };

    public VentanaEureka() {
        super("EUREKABANK RESTFUL · Banca digital");
        TemaEureka.aplicar();
        setDefaultCloseOperation(EXIT_ON_CLOSE);
        setMinimumSize(new Dimension(1040, 700));
        raiz.add(crearLogin(), "login");
        raiz.add(crearAplicacion(), "app");
        setContentPane(raiz);
        pack();
        setLocationRelativeTo(null);
    }

    private JPanel crearLogin() {
        JPanel fondo = new JPanel(new GridBagLayout());
        fondo.setBackground(TemaEureka.FONDO);
        prepararCampoLogin(usuario);
        prepararCampoLogin(contrasenia);
        JPanel tarjeta = new JPanel(new GridBagLayout());
        tarjeta.setPreferredSize(new Dimension(430, 510));
        tarjeta.setBackground(TemaEureka.PANEL);
        tarjeta.setBorder(TemaEureka.borde());
        GridBagConstraints g = new GridBagConstraints();
        g.insets = new Insets(9, 14, 9, 14);
        g.fill = GridBagConstraints.HORIZONTAL;
        g.gridx = 0;
        g.weightx = 1;
        g.gridy = 0;
        tarjeta.add(logo(), g);
        g.gridy++;
        JLabel titulo = new JLabel("EUREKABANK RESTFUL", SwingConstants.CENTER);
        titulo.setFont(new Font("Segoe UI", Font.BOLD, 27));
        tarjeta.add(titulo, g);
        g.gridy++;
        JLabel subtitulo = new JLabel("Cliente de escritorio · API RESTful", SwingConstants.CENTER);
        subtitulo.setForeground(TemaEureka.MUTED);
        tarjeta.add(subtitulo, g);
        g.gridy++;
        tarjeta.add(grupoCampo("Usuario", usuario), g);
        g.gridy++;
        tarjeta.add(grupoCampo("Contraseña", contrasenia), g);
        g.gridy++;
        estadoLogin.setForeground(TemaEureka.ERROR);
        tarjeta.add(estadoLogin, g);
        g.gridy++;
        botonIngresar = TemaEureka.boton("Ingresar a mi banca");
        tarjeta.add(botonIngresar, g);
        getRootPane().setDefaultButton(botonIngresar);
        botonIngresar.addActionListener(e -> iniciarSesion());
        fondo.add(tarjeta);
        return fondo;
    }

    private JPanel crearAplicacion() {
        JPanel principal = new JPanel(new BorderLayout());
        principal.setBackground(TemaEureka.FONDO);
        JPanel barra = new JPanel(new BorderLayout());
        barra.setBackground(TemaEureka.PANEL);
        barra.setBorder(new EmptyBorder(13, 28, 13, 28));
        JLabel marca = new JLabel("EUREKABANK");
        marca.setFont(new Font("Segoe UI", Font.BOLD, 19));
        marca.setForeground(TemaEureka.TEXTO);
        barra.add(marca, BorderLayout.WEST);
        JPanel derecha = new JPanel(new FlowLayout(FlowLayout.RIGHT, 13, 0));
        derecha.setOpaque(false);
        saludo.setForeground(TemaEureka.MUTED);
        JButton salir = TemaEureka.boton("Cerrar sesión");
        salir.addActionListener(e -> cerrarSesion());
        derecha.add(saludo);
        derecha.add(salir);
        barra.add(derecha, BorderLayout.EAST);
        JPanel cabecera = new JPanel(new BorderLayout());
        cabecera.setBackground(TemaEureka.PANEL);
        cabecera.add(barra, BorderLayout.NORTH);
        JPanel navegacion = new JPanel(new FlowLayout(FlowLayout.LEFT, 3, 0));
        navegacion.setBackground(TemaEureka.PANEL);
        navegacion.setBorder(new EmptyBorder(0, 23, 0, 23));
        JPanel contenido = new JPanel(secciones);
        contenido.setBackground(TemaEureka.FONDO);
        ButtonGroup grupoNavegacion = new ButtonGroup();
        agregarSeccion(navegacion, grupoNavegacion, contenido, "Resumen", "resumen", crearResumen(), true);
        agregarSeccion(navegacion, grupoNavegacion, contenido, "Movimientos", "movimientos", crearMovimientos(), false);
        agregarSeccion(navegacion, grupoNavegacion, contenido, "Transferir", "transferir", crearTransferencia(), false);
        operacionesNavegacion = agregarSeccion(navegacion, grupoNavegacion, contenido, "Operaciones", "operaciones", crearOperaciones(), false);
        cuentasNavegacion = agregarSeccion(navegacion, grupoNavegacion, contenido, "Cuentas", "cuentas", crearAdministracion(), false);
        agregarSeccion(navegacion, grupoNavegacion, contenido, "Perfil", "perfil", crearPerfil(), false);
        cabecera.add(navegacion, BorderLayout.SOUTH);
        principal.add(cabecera, BorderLayout.NORTH);
        principal.add(contenido, BorderLayout.CENTER);
        return principal;
    }

    private JToggleButton agregarSeccion(JPanel navegacion, ButtonGroup grupo, JPanel contenido, String titulo,
            String clave, JPanel vista, boolean seleccionada) {
        JToggleButton boton = TemaEureka.navegacion(titulo);
        boton.addActionListener(e -> secciones.show(contenido, clave));
        grupo.add(boton);
        navegacion.add(boton);
        contenido.add(vista, clave);
        if (seleccionada) {
            boton.setSelected(true);
        }
        return boton;
    }

    private JPanel crearResumen() {
        JPanel p = new JPanel(new BorderLayout(16, 16));
        p.setBorder(new EmptyBorder(26, 26, 26, 26));
        JLabel titulo = new JLabel("Resumen de cuentas");
        titulo.setFont(new Font("Segoe UI", Font.BOLD, 23));
        p.add(titulo, BorderLayout.NORTH);
        JTable tabla = new JTable(modeloCuentas);
        tabla.setRowHeight(36);
        p.add(new JScrollPane(tabla), BorderLayout.CENTER);
        JButton actualizar = TemaEureka.boton("Actualizar saldos");
        actualizar.addActionListener(e -> refrescarCuentas());
        p.add(actualizar, BorderLayout.SOUTH);
        return p;
    }

    private JPanel crearMovimientos() {
        JPanel p = new JPanel(new BorderLayout(14, 14));
        p.setBorder(new EmptyBorder(26, 26, 26, 26));
        JPanel filtros = new JPanel(new FlowLayout(FlowLayout.LEFT, 10, 0));
        JTextField cuenta = TemaEureka.campo();
        JButton consultar = TemaEureka.boton("Consultar historial");
        filtros.add(grupoCampo("Número de cuenta", cuenta));
        filtros.add(consultar);
        p.add(filtros, BorderLayout.NORTH);
        p.add(new JScrollPane(new JTable(modeloMovimientos)), BorderLayout.CENTER);
        consultar.addActionListener(e -> ejecutarAsync(() -> {
            List<Movimiento> lista = controlador.movimientos(cuenta.getText().trim());
            SwingUtilities.invokeLater(() -> {
                modeloMovimientos.setRowCount(0);
                for (Movimiento m : lista) {
                    modeloMovimientos.addRow(
                            new Object[]{m.id(), m.tipo(), "$ " + m.monto(), m.fecha()});
                }
            });
        }, null));
        return p;
    }

    private JPanel crearTransferencia() {
        JPanel vista = formulario("Transferencia entre cuentas", "Usa números de cuenta de 10 dígitos.");
        JPanel p = contenidoFormulario(vista);
        JTextField origen = campo();
        JTextField destino = campo();
        JTextField monto = campo();
        JLabel estado = mensajeEstado();
        JButton transferir = TemaEureka.boton("Confirmar transferencia");
        p.add(grupoCampo("Número de cuenta origen", origen));
        p.add(Box.createVerticalStrut(12));
        p.add(grupoCampo("Número de cuenta destino", destino));
        p.add(Box.createVerticalStrut(12));
        p.add(grupoCampo("Monto", monto));
        p.add(Box.createVerticalStrut(14));
        p.add(estado);
        p.add(transferir);
        transferir.addActionListener(e -> {
            final BigDecimal valor;
            try {
                validarCuenta(origen.getText(), "Cuenta origen");
                validarCuenta(destino.getText(), "Cuenta destino");
                if (origen.getText().trim().equals(destino.getText().trim())) {
                    throw new IllegalArgumentException("Las cuentas de origen y destino deben ser diferentes.");
                }
                valor = validarMonto(monto.getText());
            } catch (IllegalArgumentException ex) {
                error(estado, ex.getMessage());
                return;
            }
            ejecutarAsync(() -> {
                controlador.transferir(origen.getText().trim(), destino.getText().trim(), valor);
                SwingUtilities.invokeLater(() -> {
                    origen.setText("");
                    destino.setText("");
                    monto.setText("");
                    exito(estado, "Transferencia completada y auditada.");
                });
            }, estado);
        });
        return vista;
    }

    private JPanel crearOperaciones() {
        JPanel vista = formulario("Operaciones de caja", "Selecciona el tipo de operación y confirma sus datos.");
        JPanel p = contenidoFormulario(vista);
        JComboBox<String> tipo = TemaEureka.selector(new String[]{"Selecciona una operación", "Depósito", "Retiro"});
        JTextField cuenta = campo();
        JTextField monto = campo();
        JLabel estado = mensajeEstado();
        JButton registrar = TemaEureka.boton("Registrar operación");
        p.add(grupoCampo("Tipo de operación", tipo));
        p.add(Box.createVerticalStrut(12));
        p.add(grupoCampo("Número de cuenta", cuenta));
        p.add(Box.createVerticalStrut(12));
        p.add(grupoCampo("Monto", monto));
        p.add(Box.createVerticalStrut(14));
        p.add(estado);
        p.add(registrar);
        registrar.addActionListener(e -> {
            String seleccionado = (String) tipo.getSelectedItem();
            if ("Selecciona una operación".equals(seleccionado)) {
                error(estado, "Selecciona Depósito o Retiro.");
                return;
            }
            String operacion = "Depósito".equals(seleccionado) ? "INGRESO" : "RETIRO";
            final BigDecimal valor;
            try {
                validarCuenta(cuenta.getText(), "Número de cuenta");
                valor = validarMonto(monto.getText());
            } catch (IllegalArgumentException ex) {
                error(estado, ex.getMessage());
                return;
            }
            ejecutarAsync(() -> {
                controlador.movimiento(operacion, cuenta.getText().trim(),
                        valor);
                SwingUtilities.invokeLater(() -> {
                    tipo.setSelectedIndex(0);
                    cuenta.setText("");
                    monto.setText("");
                    exito(estado, seleccionado + " registrado correctamente.");
                });
            }, estado);
        });
        p.add(Box.createVerticalStrut(18));
        JLabel reversaTitulo = new JLabel("Reversar movimiento");
        reversaTitulo.setFont(new Font("Segoe UI", Font.BOLD, 16));
        p.add(reversaTitulo);
        JTextField reversa = campo();
        JButton reversar = TemaEureka.boton("Registrar reversa");
        p.add(grupoCampo("ID de transacción", reversa));
        p.add(Box.createVerticalStrut(12));
        p.add(reversar);
        reversar.addActionListener(e -> {
            if (!reversa.getText().trim().matches("[0-9a-fA-F-]{36}")) {
                error(estado, "Ingresa un ID de transacción válido.");
                return;
            }
            ejecutarAsync(() -> {
                controlador.reversar(reversa.getText().trim());
                SwingUtilities.invokeLater(() -> {
                    reversa.setText("");
                    exito(estado, "Reversa registrada correctamente.");
                });
            }, estado);
        });
        return vista;
    }

    private JPanel crearPerfil() {
        JPanel p = new JPanel(new BorderLayout(12, 12));
        p.setBorder(new EmptyBorder(26, 26, 26, 26));
        JTextArea datos = new JTextArea();
        datos.setEditable(false);
        datos.setFont(new Font("Segoe UI", Font.PLAIN, 16));
        JButton ver = TemaEureka.boton("Ver mi perfil");
        ver.addActionListener(e -> ejecutarAsync(() -> {
            Perfil perfil = controlador.perfil();
            SwingUtilities.invokeLater(() -> datos.setText("PERFIL BANCARIO\n\n" + perfil.nombre()
                    + "\n@" + perfil.usuario() + "\n" + perfil.telefono()
                    + "\n\nCuentas asociadas: " + perfil.cuentas().size()));
        }, null));
        p.add(ver, BorderLayout.NORTH);
        p.add(new JScrollPane(datos), BorderLayout.CENTER);
        return p;
    }

    private JPanel crearAdministracion() {
        JPanel vista = formulario("Crear cuenta para un cliente",
                "Selecciona al titular; el sistema genera y asigna una cuenta única automáticamente.");
        JPanel p = contenidoFormulario(vista);
        JComboBox<Cliente> clientes = TemaEureka.selector(new Cliente[0]);
        clientes.setEnabled(false);
        JLabel estado = mensajeEstado();
        JButton cargar = TemaEureka.boton("Cargar clientes");
        JButton crear = TemaEureka.boton("Crear y asignar cuenta");
        crear.setEnabled(false);
        clientes.addActionListener(e -> {
            Cliente seleccionado = (Cliente) clientes.getSelectedItem();
            crear.setEnabled(seleccionado != null && !seleccionado.id().isBlank());
        });
        p.add(cargar);
        p.add(Box.createVerticalStrut(16));
        p.add(grupoCampo("Cliente titular", clientes));
        p.add(Box.createVerticalStrut(14));
        p.add(estado);
        p.add(crear);
        cargar.addActionListener(e -> ejecutarAsync(() -> {
            List<Cliente> lista = controlador.clientesParaCuenta();
            SwingUtilities.invokeLater(() -> {
                clientes.removeAllItems();
                clientes.addItem(new Cliente("", "", ""));
                for (Cliente cliente : lista) {
                    clientes.addItem(cliente);
                }
                clientes.setSelectedIndex(0);
                clientes.setEnabled(!lista.isEmpty());
                crear.setEnabled(false);
                estado.setText(lista.isEmpty() ? "No existen clientes disponibles." : "Selecciona el titular de la nueva cuenta.");
                estado.setForeground(TemaEureka.MUTED);
            });
        }, estado));
        crear.addActionListener(e -> {
            Cliente cliente = (Cliente) clientes.getSelectedItem();
            if (cliente == null) {
                error(estado, "Selecciona un cliente antes de crear la cuenta.");
                return;
            }
            ejecutarAsync(() -> {
                Cuenta cuenta = controlador.crearCuentaParaUsuario(cliente.id());
                SwingUtilities.invokeLater(() -> {
                    clientes.setSelectedIndex(0);
                    exito(estado, "Cuenta " + cuenta.numero() + " creada y asignada a " + cliente.nombre() + ".");
                });
            }, estado);
        });
        return vista;
    }

    private JPanel formulario(String titulo, String descripcion) {
        JPanel exterior = new JPanel(new GridBagLayout());
        exterior.setBackground(TemaEureka.FONDO);
        exterior.setBorder(new EmptyBorder(30, 30, 30, 30));
        JPanel p = new JPanel() {
            @Override
            public Dimension getPreferredSize() {
                Dimension natural = super.getPreferredSize();
                return new Dimension(620, natural.height);
            }
        };
        p.setLayout(new BoxLayout(p, BoxLayout.Y_AXIS));
        p.setBackground(TemaEureka.PANEL);
        p.setBorder(TemaEureka.borde());
        p.setMaximumSize(new Dimension(620, Integer.MAX_VALUE));
        JLabel encabezado = new JLabel(titulo);
        encabezado.setFont(new Font("Segoe UI", Font.BOLD, 24));
        JLabel detalle = new JLabel(descripcion);
        detalle.setForeground(TemaEureka.MUTED);
        p.add(encabezado);
        p.add(Box.createVerticalStrut(5));
        p.add(detalle);
        p.add(Box.createVerticalStrut(18));
        exterior.add(p);
        exterior.putClientProperty("contenidoFormulario", p);
        return exterior;
    }

    private JPanel contenidoFormulario(JPanel vista) {
        return (JPanel) vista.getClientProperty("contenidoFormulario");
    }

    private JTextField campo() {
        JTextField campo = TemaEureka.campo();
        campo.setAlignmentX(Component.LEFT_ALIGNMENT);
        return campo;
    }

    private JPanel grupoCampo(String titulo, JComponent control) {
        JPanel grupo = new JPanel();
        grupo.setLayout(new BoxLayout(grupo, BoxLayout.Y_AXIS));
        grupo.setOpaque(false);
        grupo.setAlignmentX(Component.LEFT_ALIGNMENT);
        JLabel etiqueta = new JLabel(titulo);
        etiqueta.setFont(new Font("Segoe UI", Font.BOLD, 13));
        etiqueta.setForeground(TemaEureka.TEXTO);
        etiqueta.setAlignmentX(Component.LEFT_ALIGNMENT);
        control.setAlignmentX(Component.LEFT_ALIGNMENT);
        grupo.add(etiqueta);
        grupo.add(Box.createVerticalStrut(6));
        grupo.add(control);
        return grupo;
    }

    private void prepararCampoLogin(JTextField campo) {
        campo.setFont(new Font("Segoe UI", Font.PLAIN, 14));
        campo.setForeground(TemaEureka.TEXTO);
        campo.setBackground(TemaEureka.PANEL);
        campo.setBorder(TemaEureka.bordeCampo());
        campo.setPreferredSize(new Dimension(360, 44));
    }

    private void validarCuenta(String valor, String etiqueta) {
        if (valor == null || !valor.trim().matches("\\d{10}")) {
            throw new IllegalArgumentException(etiqueta + " debe tener exactamente 10 dígitos.");
        }
    }

    private BigDecimal validarMonto(String valor) {
        try {
            BigDecimal monto = new BigDecimal(valor == null ? "" : valor.trim());
            if (monto.compareTo(BigDecimal.ZERO) <= 0) {
                throw new NumberFormatException();
            }
            return monto;
        } catch (NumberFormatException e) {
            throw new IllegalArgumentException("El monto debe ser un número mayor a cero.");
        }
    }

    private JLabel mensajeEstado() {
        JLabel etiqueta = new JLabel(" ");
        etiqueta.setAlignmentX(Component.LEFT_ALIGNMENT);
        etiqueta.setForeground(TemaEureka.ERROR);
        return etiqueta;
    }

    private void iniciarSesion() {
        estadoLogin.setText("Conectando...");
        ejecutarAsync(() -> {
            Sesion sesion = controlador.login(usuario.getText().trim(), new String(contrasenia.getPassword()));
            SwingUtilities.invokeLater(() -> {
                boolean esCliente = "CLIENTE".equalsIgnoreCase(sesion.rol());
                operacionesNavegacion.setVisible(!esCliente);
                cuentasNavegacion.setVisible(!esCliente);
                saludo.setText("Hola, " + sesion.nombre());
                estadoLogin.setText(" ");
                getRootPane().setDefaultButton(null);
                paginas.show(raiz, "app");
                refrescarCuentas();
            });
        }, estadoLogin);
    }

    private void cerrarSesion() {
        ejecutarAsync(() -> {
            try {
                controlador.cerrarSesion();
            } catch (Exception ignored) {
                controlador.invalidarSesion();
            }
            SwingUtilities.invokeLater(() -> volverAlLogin("Sesión cerrada correctamente."));
        }, null);
    }

    private void refrescarCuentas() {
        if (actualizandoCuentas || controlador.sesion() == null) {
            return;
        }
        actualizandoCuentas = true;
        ejecutarAsync(() -> {
            try {
                List<Cuenta> lista = controlador.cuentas();
                Map<String, String> disponibilidades = controlador.disponibilidades();
                SwingUtilities.invokeLater(() -> {
                    modeloCuentas.setRowCount(0);
                    for (Cuenta c : lista) {
                        modeloCuentas.addRow(
                                new Object[]{c.numero(), "$ " + c.saldo(), c.estado(),
                                    disponibilidades.getOrDefault(c.id(), "DISPONIBLE"), c.sucursal()});
                    }
                    actualizandoCuentas = false;
                });
            } catch (Exception e) {
                SwingUtilities.invokeLater(() -> actualizandoCuentas = false);
                throw e;
            }
        }, null);
    }

    private void ejecutarAsync(Tarea tarea, JLabel destinoError) {
        new Thread(() -> {
            try {
                tarea.ejecutar();
            } catch (Exception error) {
                SwingUtilities.invokeLater(() -> {
                    String mensaje = limpiar(error.getMessage());
                    if (sesionInvalida(mensaje)) {
                        controlador.invalidarSesion();
                        volverAlLogin("Tu sesión venció. Inicia sesión nuevamente.");
                    } else if (destinoError != null) {
                        this.error(destinoError, mensaje);
                    } else {
                        JOptionPane.showMessageDialog(this, mensaje, "EUREKABANK", JOptionPane.ERROR_MESSAGE);
                    }
                });
            }
        }, "eurekabank-restful").start();
    }

    private boolean sesionInvalida(String mensaje) {
        String valor = mensaje.toLowerCase();
        return valor.contains("token invalido") || valor.contains("token inválido")
                || valor.contains("token expirado") || valor.contains("token revocado");
    }

    private void volverAlLogin(String mensaje) {
        actualizandoCuentas = false;
        contrasenia.setText("");
        estadoLogin.setForeground(TemaEureka.ERROR);
        estadoLogin.setText(mensaje);
        getRootPane().setDefaultButton(botonIngresar);
        paginas.show(raiz, "login");
    }

    private void exito(JLabel destino, String mensaje) {
        destino.setText(" ");
        mostrarToast(mensaje);
        refrescarCuentas();
    }

    /**
     * Notificación breve no modal para confirmar operaciones exitosas.
     */
    private void mostrarToast(String mensaje) {
        JDialog toast = new JDialog(this);
        toast.setUndecorated(true);
        toast.setAlwaysOnTop(true);
        JPanel contenido = new JPanel(new FlowLayout(FlowLayout.LEFT, 12, 10));
        contenido.setBackground(new Color(12, 125, 104));
        contenido.setBorder(BorderFactory.createCompoundBorder(
                BorderFactory.createLineBorder(new Color(5, 92, 79)), new EmptyBorder(3, 8, 3, 10)));
        JLabel icono = new JLabel("✓");
        icono.setForeground(Color.WHITE);
        icono.setFont(new Font("Segoe UI", Font.BOLD, 18));
        JLabel texto = new JLabel(mensaje);
        texto.setForeground(Color.WHITE);
        texto.setFont(new Font("Segoe UI", Font.BOLD, 13));
        contenido.add(icono);
        contenido.add(texto);
        toast.setContentPane(contenido);
        toast.pack();
        Point ventana = getLocationOnScreen();
        toast.setLocation(ventana.x + getWidth() - toast.getWidth() - 28,
                ventana.y + getHeight() - toast.getHeight() - 42);
        toast.setVisible(true);
        Timer cerrar = new Timer(2800, e -> toast.dispose());
        cerrar.setRepeats(false);
        cerrar.start();
    }

    private void error(JLabel destino, String mensaje) {
        destino.setForeground(TemaEureka.ERROR);
        destino.setText(mensaje);
    }

    private JLabel logo() {
        URL recurso = getClass().getResource("/img/logo.png");
        if (recurso != null) {
            ImageIcon icono = new ImageIcon(recurso);
            return new JLabel(new ImageIcon(icono.getImage().getScaledInstance(76, 76, Image.SCALE_SMOOTH)),
                    SwingConstants.CENTER);
        }
        JLabel respaldo = new JLabel("◆", SwingConstants.CENTER);
        respaldo.setFont(new Font("Segoe UI", Font.BOLD, 64));
        respaldo.setForeground(TemaEureka.ACENTO);
        return respaldo;
    }

    private String limpiar(String valor) {
        return valor == null ? "No se pudo completar la operación."
                : valor.replaceAll("(?s)<[^>]*>", "").trim();
    }

    @FunctionalInterface
    private interface Tarea {

        void ejecutar() throws Exception;
    }
}
