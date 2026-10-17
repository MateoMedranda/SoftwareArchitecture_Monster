package ec.edu.monster.vistas;

import ec.edu.monster.modelos.TipoConversion;
import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.GridLayout;
import java.util.EnumMap;
import java.util.Map;
import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTabbedPane;

/** Ventana principal de las conversiones. */
public class MainVista extends JFrame {

    private final Map<TipoConversion, PanelConversionVista> paneles =
            new EnumMap<>(TipoConversion.class);
    private final JButton cerrarSesionBtn = AppTheme.botonSecundario("Cerrar sesión");

    public MainVista() {
        this("");
    }

    public MainVista(String usuario) {
        configurarVentana(usuario);
    }

    private void configurarVentana(String usuario) {
        setTitle("MONSTER · Conversor de unidades");
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setMinimumSize(new Dimension(980, 660));

        AppTheme.FondoDegradado fondo = new AppTheme.FondoDegradado();
        fondo.setLayout(new GridBagLayout());
        AppTheme.PanelRedondeado tarjeta = new AppTheme.PanelRedondeado(26, Color.WHITE);
        tarjeta.setLayout(new BorderLayout(0, 18));
        tarjeta.setPreferredSize(new Dimension(900, 590));
        tarjeta.setBorder(BorderFactory.createEmptyBorder(18, 28, 22, 28));

        tarjeta.add(crearEncabezado(usuario), BorderLayout.NORTH);

        JTabbedPane pestanias = new JTabbedPane();
        pestanias.setFont(new Font("Segoe UI", Font.BOLD, 14));
        pestanias.setBorder(BorderFactory.createEmptyBorder());
        for (TipoConversion tipo : TipoConversion.values()) {
            PanelConversionVista panel = new PanelConversionVista(tipo);
            paneles.put(tipo, panel);
            pestanias.addTab(tipo.getTitulo(), panel);
        }
        tarjeta.add(pestanias, BorderLayout.CENTER);

        fondo.add(tarjeta, new GridBagConstraints());
        setContentPane(fondo);
        pack();
    }

    private JPanel crearEncabezado(String usuario) {
        JPanel encabezado = new JPanel(new BorderLayout(16, 0));
        encabezado.setOpaque(false);
        encabezado.setBorder(BorderFactory.createMatteBorder(0, 0, 1, 0, new Color(225, 233, 242)));

        JLabel logo = new JLabel(AppTheme.logo(58, 58));
        encabezado.add(logo, BorderLayout.WEST);

        JPanel textos = new JPanel(new GridLayout(2, 1, 0, 2));
        textos.setOpaque(false);
        textos.add(AppTheme.etiqueta("Conversor de unidades", 22, Font.BOLD, AppTheme.AZUL_OSCURO));
        String saludo = usuario == null || usuario.isBlank()
                ? "Selecciona una categoría y realiza tu conversión"
                : "Bienvenido, " + usuario;
        textos.add(AppTheme.etiqueta(saludo, 13, Font.PLAIN, AppTheme.TEXTO_SUAVE));
        encabezado.add(textos, BorderLayout.CENTER);

        JPanel acciones = new JPanel(new GridBagLayout());
        acciones.setOpaque(false);
        cerrarSesionBtn.setPreferredSize(new Dimension(148, 40));
        acciones.add(cerrarSesionBtn, new GridBagConstraints());
        encabezado.add(acciones, BorderLayout.EAST);
        return encabezado;
    }

    public PanelConversionVista getPanelConversion(TipoConversion tipo) {
        return paneles.get(tipo);
    }

    public JButton getBtnCerrarSesion() {
        return cerrarSesionBtn;
    }
}
