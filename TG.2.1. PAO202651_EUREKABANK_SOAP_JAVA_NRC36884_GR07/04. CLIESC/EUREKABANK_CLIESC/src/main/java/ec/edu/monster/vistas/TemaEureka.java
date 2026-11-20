package ec.edu.monster.vistas;

import java.awt.*;
import javax.swing.*;
import javax.swing.border.*;
import javax.swing.plaf.basic.BasicComboBoxUI;

/** Paleta y componentes visuales compartidos con la marca web de EUREKABANK. */
public final class TemaEureka {
    public static final Color FONDO = new Color(244, 248, 252);
    public static final Color PANEL = Color.WHITE;
    public static final Color ACENTO = new Color(35, 104, 155);
    public static final Color TURQUESA = new Color(22, 167, 184);
    public static final Color TEXTO = new Color(18, 58, 90);
    public static final Color MUTED = new Color(91, 113, 135);
    public static final Color BORDE = new Color(207, 222, 235);
    public static final Color ERROR = new Color(184, 65, 82);
    private static final Font FUENTE = new Font("Segoe UI", Font.PLAIN, 14);

    private TemaEureka() { }

    public static void aplicar() {
        UIManager.put("Panel.background", FONDO);
        UIManager.put("Label.foreground", TEXTO);
        UIManager.put("Button.font", new Font("Segoe UI", Font.BOLD, 13));
        UIManager.put("Table.font", FUENTE);
        UIManager.put("Table.foreground", TEXTO);
        UIManager.put("Table.background", PANEL);
        UIManager.put("Table.selectionBackground", new Color(218, 241, 246));
        UIManager.put("Table.selectionForeground", TEXTO);
        UIManager.put("TableHeader.background", new Color(232, 243, 250));
        UIManager.put("TableHeader.foreground", TEXTO);
    }

    public static Border borde() {
        return new CompoundBorder(new LineBorder(BORDE, 1, true), new EmptyBorder(26, 28, 26, 28));
    }

    public static Border bordeCampo() {
        return new CompoundBorder(new LineBorder(BORDE, 1, true), new EmptyBorder(0, 13, 0, 13));
    }

    public static JButton boton(String texto) {
        JButton boton = new JButton(texto) {
            @Override protected void paintComponent(Graphics g) {
                Graphics2D dibujo = (Graphics2D) g.create();
                dibujo.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
                Color color = getModel().isPressed() ? ACENTO.darker()
                        : getModel().isRollover() ? new Color(29, 124, 174) : ACENTO;
                dibujo.setColor(color);
                dibujo.fillRoundRect(0, 0, getWidth(), getHeight(), 10, 10);
                dibujo.dispose();
                super.paintComponent(g);
            }
        };
        boton.setForeground(Color.WHITE);
        boton.setBackground(ACENTO);
        boton.setBorder(new EmptyBorder(11, 20, 11, 20));
        boton.setContentAreaFilled(false);
        boton.setFocusPainted(false);
        boton.setOpaque(false);
        boton.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
        boton.setAlignmentX(Component.LEFT_ALIGNMENT);
        return boton;
    }

    public static JTextField campo() {
        JTextField campo = new JTextField();
        campo.setFont(FUENTE);
        campo.setForeground(TEXTO);
        campo.setBackground(PANEL);
        campo.setBorder(bordeCampo());
        campo.setMaximumSize(new Dimension(Integer.MAX_VALUE, 44));
        campo.setPreferredSize(new Dimension(360, 44));
        return campo;
    }

    public static <T> JComboBox<T> selector(T[] opciones) {
        JComboBox<T> selector = new JComboBox<>(opciones);
        selector.setFont(FUENTE);
        selector.setForeground(TEXTO);
        selector.setBackground(PANEL);
        selector.setBorder(bordeCampo());
        selector.setMaximumSize(new Dimension(Integer.MAX_VALUE, 44));
        selector.setPreferredSize(new Dimension(360, 44));
        selector.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
        selector.setUI(new BasicComboBoxUI() {
            @Override protected JButton createArrowButton() {
                JButton flecha = new JButton("▼");
                flecha.setFont(new Font("Segoe UI", Font.BOLD, 19));
                flecha.setForeground(ACENTO);
                flecha.setBorder(new EmptyBorder(0, 4, 3, 11));
                flecha.setContentAreaFilled(false);
                flecha.setFocusPainted(false);
                flecha.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
                return flecha;
            }
        });
        selector.setRenderer(new DefaultListCellRenderer() {
            @Override public Component getListCellRendererComponent(JList<?> lista, Object valor,
                    int indice, boolean seleccionado, boolean enfocado) {
                JLabel etiqueta = (JLabel) super.getListCellRendererComponent(lista, valor, indice,
                        seleccionado, enfocado);
                etiqueta.setFont(FUENTE);
                etiqueta.setBorder(new EmptyBorder(9, 12, 9, 12));
                etiqueta.setOpaque(true);
                etiqueta.setBackground(seleccionado ? new Color(222, 246, 248) : PANEL);
                boolean guia = valor == null || String.valueOf(valor).startsWith("Selecciona el titular");
                etiqueta.setForeground(seleccionado ? TEXTO : (guia ? MUTED : TEXTO));
                return etiqueta;
            }
        });
        return selector;
    }

    public static JToggleButton navegacion(String texto) {
        JToggleButton boton = new JToggleButton(texto);
        boton.setFont(new Font("Segoe UI", Font.BOLD, 13));
        boton.setForeground(MUTED);
        boton.setBackground(PANEL);
        boton.setBorder(bordeNavegacion(false));
        boton.setContentAreaFilled(false);
        boton.setFocusPainted(false);
        boton.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
        boton.addItemListener(e -> {
            boolean activo = boton.isSelected();
            boton.setForeground(activo ? ACENTO : MUTED);
            boton.setBorder(bordeNavegacion(activo));
        });
        return boton;
    }

    private static Border bordeNavegacion(boolean activo) {
        return new CompoundBorder(new MatteBorder(0, 0, 3, 0, activo ? TURQUESA : PANEL),
                new EmptyBorder(12, 17, 9, 17));
    }
}
