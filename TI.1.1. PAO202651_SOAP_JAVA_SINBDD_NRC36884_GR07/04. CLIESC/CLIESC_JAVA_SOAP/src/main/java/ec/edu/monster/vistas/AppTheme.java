package ec.edu.monster.vistas;

import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Cursor;
import java.awt.Font;
import java.awt.GradientPaint;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Image;
import java.awt.RenderingHints;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.net.URL;
import javax.swing.BorderFactory;
import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.UIManager;
import javax.swing.border.Border;

/** Paleta y componentes visuales compartidos por el cliente de escritorio. */
public final class AppTheme {

    public static final Color AZUL_PRINCIPAL = new Color(34, 147, 235);
    public static final Color AZUL_OSCURO = new Color(23, 55, 92);
    public static final Color AZUL_SUAVE = new Color(235, 245, 255);
    public static final Color FONDO_INICIO = new Color(220, 236, 253);
    public static final Color FONDO_FIN = new Color(190, 218, 246);
    public static final Color TEXTO = new Color(31, 45, 61);
    public static final Color TEXTO_SUAVE = new Color(94, 112, 132);
    public static final Color BORDE = new Color(207, 220, 234);
    public static final Color EXITO = new Color(25, 135, 84);
    public static final Color ERROR = new Color(195, 53, 65);

    private AppTheme() { }

    public static void instalar() {
        try {
            UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
        } catch (Exception ignored) {
            // La aplicación puede funcionar con el look and feel por defecto.
        }
        UIManager.put("TabbedPane.selected", Color.WHITE);
        UIManager.put("TabbedPane.background", Color.WHITE);
        UIManager.put("TabbedPane.foreground", TEXTO_SUAVE);
        UIManager.put("TabbedPane.focus", new Color(0, 0, 0, 0));
        UIManager.put("ComboBox.selectionBackground", AZUL_PRINCIPAL);
        UIManager.put("ComboBox.selectionForeground", Color.WHITE);
    }

    public static JLabel etiqueta(String texto, float tamano, int estilo, Color color) {
        JLabel etiqueta = new JLabel(texto);
        etiqueta.setFont(new Font("Segoe UI", estilo, Math.round(tamano)));
        etiqueta.setForeground(color);
        return etiqueta;
    }

    public static void configurarCampo(JTextField campo) {
        campo.setFont(new Font("Segoe UI", Font.PLAIN, 15));
        campo.setForeground(TEXTO);
        campo.setBackground(Color.WHITE);
        campo.setCaretColor(AZUL_PRINCIPAL);
        campo.setBorder(bordeCampo());
        campo.setMargin(new java.awt.Insets(8, 11, 8, 11));
        campo.setPreferredSize(new java.awt.Dimension(280, 43));
    }

    public static void configurarCombo(JComboBox<?> combo) {
        combo.setFont(new Font("Segoe UI", Font.PLAIN, 15));
        combo.setForeground(TEXTO);
        combo.setBackground(Color.WHITE);
        combo.setBorder(bordeCampo());
        combo.setPreferredSize(new java.awt.Dimension(280, 43));
    }

    private static Border bordeCampo() {
        return BorderFactory.createCompoundBorder(
                BorderFactory.createLineBorder(BORDE, 1, true),
                BorderFactory.createEmptyBorder(1, 3, 1, 3));
    }

    public static JButton botonPrincipal(String texto) {
        return new RoundedButton(texto, AZUL_PRINCIPAL, Color.WHITE, false);
    }

    public static JButton botonSecundario(String texto) {
        return new RoundedButton(texto, Color.WHITE, AZUL_PRINCIPAL, true);
    }

    public static ImageIcon logo(int ancho, int alto) {
        URL recurso = AppTheme.class.getResource("/img/logo.png");
        if (recurso == null) {
            return null;
        }
        Image imagen = new ImageIcon(recurso).getImage()
                .getScaledInstance(ancho, alto, Image.SCALE_SMOOTH);
        return new ImageIcon(imagen);
    }

    static class PanelRedondeado extends JPanel {
        private final int radio;
        private final Color color;

        PanelRedondeado(int radio, Color color) {
            this.radio = radio;
            this.color = color;
            setOpaque(false);
        }

        @Override
        protected void paintComponent(Graphics graphics) {
            Graphics2D g2 = (Graphics2D) graphics.create();
            g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING,
                    RenderingHints.VALUE_ANTIALIAS_ON);
            g2.setColor(color);
            g2.fillRoundRect(0, 0, getWidth(), getHeight(), radio, radio);
            g2.dispose();
            super.paintComponent(graphics);
        }
    }

    static class FondoDegradado extends JPanel {
        FondoDegradado() {
            setOpaque(false);
        }

        @Override
        protected void paintComponent(Graphics graphics) {
            Graphics2D g2 = (Graphics2D) graphics.create();
            g2.setRenderingHint(RenderingHints.KEY_RENDERING,
                    RenderingHints.VALUE_RENDER_QUALITY);
            g2.setPaint(new GradientPaint(0, 0, FONDO_INICIO,
                    getWidth(), getHeight(), FONDO_FIN));
            g2.fillRect(0, 0, getWidth(), getHeight());
            g2.setColor(new Color(255, 255, 255, 42));
            g2.fillOval(-110, -100, 320, 320);
            g2.fillOval(getWidth() - 200, getHeight() - 175, 310, 310);
            g2.setColor(new Color(34, 147, 235, 28));
            g2.fillOval(getWidth() - 250, -75, 250, 250);
            g2.dispose();
            super.paintComponent(graphics);
        }
    }

    private static class RoundedButton extends JButton {
        private final Color fondo;
        private final boolean contorno;

        RoundedButton(String textoBoton, Color fondo, Color texto, boolean contorno) {
            super(textoBoton);
            this.fondo = fondo;
            this.contorno = contorno;
            setFont(new Font("Segoe UI", Font.BOLD, 15));
            setForeground(texto);
            setFocusPainted(false);
            setBorderPainted(false);
            setContentAreaFilled(false);
            setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
            setPreferredSize(new java.awt.Dimension(160, 43));
            addMouseListener(new MouseAdapter() {
                @Override public void mouseEntered(MouseEvent evento) { repaint(); }
                @Override public void mouseExited(MouseEvent evento) { repaint(); }
            });
        }

        @Override
        protected void paintComponent(Graphics graphics) {
            Graphics2D g2 = (Graphics2D) graphics.create();
            g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING,
                    RenderingHints.VALUE_ANTIALIAS_ON);
            Color colorActual = fondo;
            if (!isEnabled()) {
                colorActual = new Color(170, 196, 219);
            } else if (getModel().isPressed()) {
                colorActual = fondo.darker();
            } else if (getModel().isRollover()) {
                colorActual = contorno ? AZUL_SUAVE : fondo.brighter();
            }
            g2.setColor(colorActual);
            g2.fillRoundRect(0, 0, getWidth(), getHeight(), 12, 12);
            if (contorno) {
                g2.setColor(AZUL_PRINCIPAL);
                g2.setStroke(new BasicStroke(1.2f));
                g2.drawRoundRect(0, 0, getWidth() - 1, getHeight() - 1, 12, 12);
            }
            g2.dispose();
            super.paintComponent(graphics);
        }
    }
}
