package ec.edu.monster.vistas;

import ec.edu.monster.modelos.ResultadoConversion;
import ec.edu.monster.modelos.TipoConversion;
import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Insets;
import java.awt.Window;
import java.awt.event.ActionListener;
import java.text.DecimalFormat;
import java.util.Locale;
import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.SwingConstants;
import javax.swing.SwingUtilities;

/** Formulario de conversión con un flujo compacto y resultado de ancho completo. */
public class PanelConversionVista extends JPanel {

    private static final DecimalFormat FORMATO_RESULTADO = new DecimalFormat("0.##########");

    private final JTextField txtValor = new JTextField();
    private final JComboBox<String> cmbOrigen;
    private final JComboBox<String> cmbDestino;
    private final JButton btnConvertir = AppTheme.botonPrincipal("Convertir");
    private final JLabel lblResultado = AppTheme.etiqueta("—", 30, Font.BOLD, AppTheme.AZUL_OSCURO);
    private final JLabel lblMensaje = AppTheme.etiqueta("Completa los campos para realizar una conversión.", 13, Font.PLAIN, AppTheme.TEXTO_SUAVE);

    public PanelConversionVista(TipoConversion tipo) {
        cmbOrigen = new JComboBox<>(tipo.getUnidades().toArray(String[]::new));
        cmbDestino = new JComboBox<>(tipo.getUnidades().toArray(String[]::new));
        if (cmbDestino.getItemCount() > 1) {
            cmbDestino.setSelectedIndex(1);
        }
        configurarComponentes(tipo);
    }

    private void configurarComponentes(TipoConversion tipo) {
        setOpaque(false);
        setLayout(new BorderLayout(0, 14));
        setBorder(BorderFactory.createEmptyBorder(22, 14, 10, 14));

        JPanel introduccion = new JPanel(new BorderLayout(0, 3));
        introduccion.setOpaque(false);
        introduccion.add(AppTheme.etiqueta("Conversión de " + tipo.getTitulo(), 20, Font.BOLD, AppTheme.AZUL_OSCURO), BorderLayout.NORTH);
        introduccion.add(AppTheme.etiqueta("Ingresa un valor, elige las unidades y obtén el resultado al instante.", 13, Font.PLAIN, AppTheme.TEXTO_SUAVE), BorderLayout.SOUTH);
        add(introduccion, BorderLayout.NORTH);

        JPanel contenido = new JPanel(new BorderLayout(0, 14));
        contenido.setOpaque(false);
        contenido.add(crearFormulario(), BorderLayout.NORTH);
        contenido.add(crearResultado(), BorderLayout.CENTER);
        add(contenido, BorderLayout.CENTER);
    }

    private JPanel crearFormulario() {
        AppTheme.PanelRedondeado tarjeta = new AppTheme.PanelRedondeado(18, Color.WHITE);
        tarjeta.setLayout(new GridBagLayout());
        tarjeta.setBorder(BorderFactory.createCompoundBorder(
                BorderFactory.createLineBorder(new Color(226, 235, 244), 1, true),
                BorderFactory.createEmptyBorder(20, 24, 20, 24)));

        AppTheme.configurarCampo(txtValor);
        txtValor.setToolTipText("Por ejemplo: 25.5");
        AppTheme.configurarCombo(cmbOrigen);
        AppTheme.configurarCombo(cmbDestino);

        GridBagConstraints c = new GridBagConstraints();
        c.anchor = GridBagConstraints.LINE_START;
        c.fill = GridBagConstraints.HORIZONTAL;

        c.gridx = 0;
        c.gridy = 0;
        c.gridwidth = 3;
        c.weightx = 1;
        c.insets = new Insets(0, 0, 6, 0);
        tarjeta.add(AppTheme.etiqueta("Valor a convertir", 13, Font.PLAIN, AppTheme.TEXTO_SUAVE), c);

        c.gridy++;
        c.insets = new Insets(0, 0, 3, 0);
        tarjeta.add(txtValor, c);

        c.gridy++;
        c.insets = new Insets(0, 0, 18, 0);
        tarjeta.add(AppTheme.etiqueta("Ejemplo: 25.5", 11, Font.PLAIN, new Color(125, 143, 162)), c);

        c.gridy++;
        c.gridx = 0;
        c.gridwidth = 1;
        c.weightx = .46;
        c.insets = new Insets(0, 0, 6, 12);
        tarjeta.add(AppTheme.etiqueta("Desde", 13, Font.PLAIN, AppTheme.TEXTO_SUAVE), c);

        c.gridx = 2;
        c.weightx = .46;
        c.insets = new Insets(0, 12, 6, 0);
        tarjeta.add(AppTheme.etiqueta("Hacia", 13, Font.PLAIN, AppTheme.TEXTO_SUAVE), c);

        c.gridy++;
        c.gridx = 0;
        c.weightx = .46;
        c.insets = new Insets(0, 0, 0, 12);
        tarjeta.add(cmbOrigen, c);

        JLabel flecha = AppTheme.etiqueta("→", 24, Font.BOLD, AppTheme.AZUL_PRINCIPAL);
        flecha.setHorizontalAlignment(SwingConstants.CENTER);
        c.gridx = 1;
        c.weightx = .08;
        c.fill = GridBagConstraints.NONE;
        c.insets = new Insets(0, 0, 0, 0);
        tarjeta.add(flecha, c);

        c.gridx = 2;
        c.weightx = .46;
        c.fill = GridBagConstraints.HORIZONTAL;
        c.insets = new Insets(0, 12, 0, 0);
        tarjeta.add(cmbDestino, c);

        c.gridy++;
        c.gridx = 0;
        c.gridwidth = 3;
        c.weightx = 1;
        c.anchor = GridBagConstraints.LINE_END;
        c.fill = GridBagConstraints.NONE;
        c.insets = new Insets(20, 0, 0, 0);
        btnConvertir.setPreferredSize(new Dimension(180, 44));
        tarjeta.add(btnConvertir, c);
        return tarjeta;
    }

    private JPanel crearResultado() {
        AppTheme.PanelRedondeado resultado = new AppTheme.PanelRedondeado(18, AppTheme.AZUL_SUAVE);
        resultado.setLayout(new GridBagLayout());
        resultado.setBorder(BorderFactory.createEmptyBorder(22, 28, 22, 28));
        GridBagConstraints c = new GridBagConstraints();
        c.gridx = 0;
        c.weightx = 1;
        c.fill = GridBagConstraints.HORIZONTAL;
        c.anchor = GridBagConstraints.LINE_START;

        c.gridy = 0;
        resultado.add(AppTheme.etiqueta("RESULTADO DE LA CONVERSIÓN", 11, Font.BOLD, AppTheme.AZUL_PRINCIPAL), c);
        c.gridy++;
        c.insets = new Insets(8, 0, 7, 0);
        lblResultado.setHorizontalAlignment(SwingConstants.LEFT);
        resultado.add(lblResultado, c);
        c.gridy++;
        c.insets = new Insets(0, 0, 0, 0);
        resultado.add(lblMensaje, c);
        return resultado;
    }

    public String getValor() {
        return txtValor.getText().trim();
    }

    public String getUnidadOrigen() {
        return (String) cmbOrigen.getSelectedItem();
    }

    public String getUnidadDestino() {
        return (String) cmbDestino.getSelectedItem();
    }

    public void agregarAccionConvertir(ActionListener accion) {
        btnConvertir.addActionListener(accion);
        txtValor.addActionListener(accion);
    }

    public void mostrarResultado(ResultadoConversion resultado) {
        String unidad = nombreLegible(resultado.getUnidadFin());
        lblResultado.setText(FORMATO_RESULTADO.format(resultado.getResultado()) + " " + unidad);
        mostrarMensaje("Conversión realizada correctamente.", AppTheme.EXITO);
    }

    private String nombreLegible(String unidad) {
        if (unidad == null || unidad.isBlank()) {
            return "";
        }
        String texto = unidad.toLowerCase(Locale.ROOT).replace('_', ' ');
        return Character.toUpperCase(texto.charAt(0)) + texto.substring(1);
    }

    public void mostrarError(String mensaje) {
        lblResultado.setText("—");
        mostrarMensaje(mensaje, AppTheme.ERROR);
    }

    public int mostrarAlertaCierreSesion() {
        String[] opciones = {"Volver a iniciar sesión", "Salir"};
        return JOptionPane.showOptionDialog(this, "Tu sesión expiró. Inicia sesión de nuevo para continuar.",
                "Sesión finalizada", JOptionPane.DEFAULT_OPTION, JOptionPane.WARNING_MESSAGE,
                null, opciones, opciones[0]);
    }

    public void limpiarMensaje() {
        mostrarMensaje("Convirtiendo tus unidades...", AppTheme.TEXTO_SUAVE);
    }

    public void setProcesando(boolean procesando) {
        btnConvertir.setEnabled(!procesando);
        txtValor.setEnabled(!procesando);
        cmbOrigen.setEnabled(!procesando);
        cmbDestino.setEnabled(!procesando);
        btnConvertir.setText(procesando ? "Convirtiendo..." : "Convertir");
    }

    private void mostrarMensaje(String mensaje, Color color) {
        lblMensaje.setForeground(color);
        lblMensaje.setText(mensaje);
    }

    public void cerrarVentanaPadre() {
        Window ventanaPadre = SwingUtilities.getWindowAncestor(this);
        if (ventanaPadre != null) {
            ventanaPadre.dispose();
        }
    }
}
