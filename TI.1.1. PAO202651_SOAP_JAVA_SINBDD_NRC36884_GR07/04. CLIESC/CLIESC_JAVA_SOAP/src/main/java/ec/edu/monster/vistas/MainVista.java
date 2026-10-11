package ec.edu.monster.vistas;

import ec.edu.monster.modelos.TipoConversion;
import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.Font;
import java.util.EnumMap;
import java.util.Map;
import javax.swing.BorderFactory;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTabbedPane;

/** Ventana principal de la aplicación cliente de conversiones. */
public class MainVista extends JFrame {

    private final Map<TipoConversion, PanelConversionVista> paneles =
            new EnumMap<>(TipoConversion.class);

    public MainVista() {
        configurarVentana();
    }

    private void configurarVentana() {
        setTitle("Conversor de Unidades");
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setMinimumSize(new Dimension(620, 340));

        JPanel contenido = new JPanel(new BorderLayout(0, 10));
        contenido.setBorder(BorderFactory.createEmptyBorder(14, 16, 16, 16));

        JLabel encabezado = new JLabel("Conversor de unidades");
        encabezado.setFont(encabezado.getFont().deriveFont(Font.BOLD, 24f));
        contenido.add(encabezado, BorderLayout.NORTH);

        JTabbedPane pestanias = new JTabbedPane();
        for (TipoConversion tipo : TipoConversion.values()) {
            PanelConversionVista panel = new PanelConversionVista(tipo);
            paneles.put(tipo, panel);
            pestanias.addTab(tipo.getTitulo(), panel);
        }
        contenido.add(pestanias, BorderLayout.CENTER);

        setContentPane(contenido);
        pack();
    }

    public PanelConversionVista getPanelConversion(TipoConversion tipo) {
        return paneles.get(tipo);
    }
}
