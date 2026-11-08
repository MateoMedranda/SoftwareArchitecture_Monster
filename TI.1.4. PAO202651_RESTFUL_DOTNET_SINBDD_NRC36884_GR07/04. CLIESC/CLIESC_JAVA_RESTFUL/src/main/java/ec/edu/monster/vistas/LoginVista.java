package ec.edu.monster.vistas;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.GridLayout;
import java.awt.Insets;
import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JPasswordField;
import javax.swing.JTextField;
import javax.swing.SwingConstants;

/** Pantalla de acceso del cliente de escritorio. */
public class LoginVista extends JFrame {

    private final JTextField usuarioTxt = new JTextField();
    private final JPasswordField contraseniaTxt = new JPasswordField();
    private final JButton loginBtn = AppTheme.botonPrincipal("Ingresar");
    private final JButton salirBtn = AppTheme.botonSecundario("Salir");
    private final JLabel mensajeLb = AppTheme.etiqueta(" ", 13, Font.PLAIN, AppTheme.ERROR);

    public LoginVista() {
        configurarVentana();
    }

    private void configurarVentana() {
        setTitle("MONSTER · Conversor de unidades");
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setMinimumSize(new Dimension(940, 650));
        setResizable(false);

        AppTheme.FondoDegradado fondo = new AppTheme.FondoDegradado();
        fondo.setLayout(new GridBagLayout());
        AppTheme.PanelRedondeado tarjeta = new AppTheme.PanelRedondeado(26, Color.WHITE);
        tarjeta.setLayout(new BorderLayout());
        tarjeta.setPreferredSize(new Dimension(430, 560));
        tarjeta.setBorder(BorderFactory.createEmptyBorder(30, 42, 28, 42));

        JPanel contenido = new JPanel(new GridBagLayout());
        contenido.setOpaque(false);
        GridBagConstraints c = new GridBagConstraints();
        c.gridx = 0;
        c.fill = GridBagConstraints.HORIZONTAL;
        c.weightx = 1;

        JLabel logo = new JLabel(AppTheme.logo(118, 118));
        logo.setHorizontalAlignment(SwingConstants.CENTER);
        c.gridy = 0;
        c.insets = new Insets(0, 0, 5, 0);
        contenido.add(logo, c);

        JLabel marca = AppTheme.etiqueta("MONSTER", 12, Font.BOLD, AppTheme.AZUL_PRINCIPAL);
        marca.setHorizontalAlignment(SwingConstants.CENTER);
        c.gridy++;
        c.insets = new Insets(0, 0, 2, 0);
        contenido.add(marca, c);

        JLabel titulo = AppTheme.etiqueta("Conversor de unidades", 25, Font.BOLD, AppTheme.AZUL_OSCURO);
        titulo.setHorizontalAlignment(SwingConstants.CENTER);
        c.gridy++;
        c.insets = new Insets(0, 0, 5, 0);
        contenido.add(titulo, c);

        JLabel subtitulo = AppTheme.etiqueta("Ingresa para transformar tus medidas", 13, Font.PLAIN, AppTheme.TEXTO_SUAVE);
        subtitulo.setHorizontalAlignment(SwingConstants.CENTER);
        c.gridy++;
        c.insets = new Insets(0, 0, 25, 0);
        contenido.add(subtitulo, c);

        AppTheme.configurarCampo(usuarioTxt);
        usuarioTxt.setToolTipText("Ingrese su usuario");
        AppTheme.configurarCampo(contraseniaTxt);
        contraseniaTxt.setToolTipText("Ingrese su contraseña");

        c.gridy++;
        c.insets = new Insets(0, 0, 6, 0);
        contenido.add(AppTheme.etiqueta("Usuario", 13, Font.PLAIN, AppTheme.TEXTO_SUAVE), c);
        c.gridy++;
        c.insets = new Insets(0, 0, 16, 0);
        contenido.add(usuarioTxt, c);

        c.gridy++;
        c.insets = new Insets(0, 0, 6, 0);
        contenido.add(AppTheme.etiqueta("Contraseña", 13, Font.PLAIN, AppTheme.TEXTO_SUAVE), c);
        c.gridy++;
        c.insets = new Insets(0, 0, 22, 0);
        contenido.add(contraseniaTxt, c);

        loginBtn.setPreferredSize(new Dimension(0, 45));
        c.gridy++;
        c.insets = new Insets(0, 0, 11, 0);
        contenido.add(loginBtn, c);

        mensajeLb.setHorizontalAlignment(SwingConstants.CENTER);
        c.gridy++;
        c.insets = new Insets(0, 0, 9, 0);
        contenido.add(mensajeLb, c);

        JPanel acciones = new JPanel(new GridLayout(1, 1));
        acciones.setOpaque(false);
        acciones.add(salirBtn);
        c.gridy++;
        c.insets = new Insets(0, 95, 0, 95);
        contenido.add(acciones, c);

        tarjeta.add(contenido, BorderLayout.CENTER);
        fondo.add(tarjeta, new GridBagConstraints());
        setContentPane(fondo);
        pack();
        setLocationRelativeTo(null);
    }

    public String getUsuario() {
        return usuarioTxt.getText().trim();
    }

    public String getPassword() {
        return new String(contraseniaTxt.getPassword());
    }

    public JButton getBtnLogin() {
        return loginBtn;
    }

    public JButton getBtnSalir() {
        return salirBtn;
    }

    public void setMensaje(String mensaje) {
        mensajeLb.setText(mensaje == null || mensaje.isBlank() ? " " : mensaje);
    }

    public void setProcesando(boolean procesando) {
        loginBtn.setEnabled(!procesando);
        usuarioTxt.setEnabled(!procesando);
        contraseniaTxt.setEnabled(!procesando);
        loginBtn.setText(procesando ? "Verificando..." : "Ingresar");
    }
}
