package ec.edu.monster.vistas;

import ec.edu.monster.modelos.ResultadoConversion;
import ec.edu.monster.modelos.TipoConversion;
import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Font;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Insets;
import java.awt.event.ActionListener;
import java.text.DecimalFormat;
import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.SwingConstants;

public class PanelConversionVista extends JPanel {

    private static final DecimalFormat FORMATO_RESULTADO = new DecimalFormat("0.##########");

    private final JTextField txtValor = new JTextField(16);
    private final JComboBox<String> cmbOrigen;
    private final JComboBox<String> cmbDestino;
    private final JButton btnConvertir = new JButton("Convertir");
    private final JLabel lblResultado = new JLabel("Ingrese un valor y presione Convertir.");
    private final JLabel lblMensaje = new JLabel(" ");

    public PanelConversionVista(TipoConversion tipo) {
        cmbOrigen = new JComboBox<>(tipo.getUnidades().toArray(String[]::new));
        cmbDestino = new JComboBox<>(tipo.getUnidades().toArray(String[]::new));
        if (cmbDestino.getItemCount() > 1) {
            cmbDestino.setSelectedIndex(1);
        }
        configurarComponentes(tipo);
    }

    private void configurarComponentes(TipoConversion tipo) {
        setLayout(new BorderLayout(12, 12));
        setBorder(BorderFactory.createEmptyBorder(20, 28, 20, 28));

        JLabel titulo = new JLabel("Conversión de " + tipo.getTitulo());
        titulo.setFont(titulo.getFont().deriveFont(Font.BOLD, 19f));
        add(titulo, BorderLayout.NORTH);

        JPanel formulario = new JPanel(new GridBagLayout());
        GridBagConstraints restricciones = new GridBagConstraints();
        restricciones.insets = new Insets(6, 6, 6, 6);
        restricciones.fill = GridBagConstraints.HORIZONTAL;

        agregarFila(formulario, restricciones, 0, "Valor:", txtValor);
        agregarFila(formulario, restricciones, 1, "Unidad de origen:", cmbOrigen);
        agregarFila(formulario, restricciones, 2, "Unidad de destino:", cmbDestino);

        restricciones.gridx = 1;
        restricciones.gridy = 3;
        restricciones.anchor = GridBagConstraints.LINE_START;
        restricciones.fill = GridBagConstraints.NONE;
        formulario.add(btnConvertir, restricciones);

        add(formulario, BorderLayout.CENTER);

        JPanel estado = new JPanel(new GridBagLayout());
        GridBagConstraints estadoRestricciones = new GridBagConstraints();
        estadoRestricciones.gridx = 0;
        estadoRestricciones.gridy = 0;
        estadoRestricciones.anchor = GridBagConstraints.LINE_START;
        lblResultado.setFont(lblResultado.getFont().deriveFont(Font.BOLD));
        estado.add(lblResultado, estadoRestricciones);
        estadoRestricciones.gridy = 1;
        lblMensaje.setHorizontalAlignment(SwingConstants.LEFT);
        estado.add(lblMensaje, estadoRestricciones);
        add(estado, BorderLayout.SOUTH);
    }

    private void agregarFila(JPanel panel, GridBagConstraints restricciones, int fila,
            String textoEtiqueta, java.awt.Component componente) {
        restricciones.gridx = 0;
        restricciones.gridy = fila;
        restricciones.weightx = 0;
        restricciones.anchor = GridBagConstraints.LINE_END;
        panel.add(new JLabel(textoEtiqueta), restricciones);

        restricciones.gridx = 1;
        restricciones.weightx = 1;
        restricciones.anchor = GridBagConstraints.LINE_START;
        panel.add(componente, restricciones);
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
        lblResultado.setText(String.format("Resultado: %s %s",
                FORMATO_RESULTADO.format(resultado.getResultado()), resultado.getUnidadFin()));
        mostrarMensaje("Conversión realizada correctamente.", new Color(0, 128, 0));
    }

    public void mostrarError(String mensaje) {
        mostrarMensaje(mensaje, new Color(180, 0, 0));
    }

    public void limpiarMensaje() {
        lblMensaje.setText(" ");
    }

    public void setProcesando(boolean procesando) {
        btnConvertir.setEnabled(!procesando);
        txtValor.setEnabled(!procesando);
        cmbOrigen.setEnabled(!procesando);
        cmbDestino.setEnabled(!procesando);
        if (procesando) {
            mostrarMensaje("Convirtiendo...", Color.DARK_GRAY);
        }
    }

    private void mostrarMensaje(String mensaje, Color color) {
        lblMensaje.setForeground(color);
        lblMensaje.setText(mensaje);
    }
}
