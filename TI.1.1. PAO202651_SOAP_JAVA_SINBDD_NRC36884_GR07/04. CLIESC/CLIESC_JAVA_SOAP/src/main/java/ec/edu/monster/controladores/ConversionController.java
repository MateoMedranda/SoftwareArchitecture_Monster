package ec.edu.monster.controladores;

import ec.edu.monster.modelos.ResultadoConversion;
import ec.edu.monster.modelos.TipoConversion;
import ec.edu.monster.servicios.WSConversorCliente;
import ec.edu.monster.vistas.PanelConversionVista;
import java.util.concurrent.ExecutionException;
import javax.swing.SwingWorker;

public class ConversionController {

    private final PanelConversionVista vista;
    private final WSConversorCliente cliente;
    private final TipoConversion tipo;

    public ConversionController(PanelConversionVista vista, WSConversorCliente cliente,
            TipoConversion tipo) {
        this.vista = vista;
        this.cliente = cliente;
        this.tipo = tipo;
        vista.agregarAccionConvertir(evento -> convertir());
    }

    private void convertir() {
        String textoValor = vista.getValor();
        if (textoValor.isBlank()) {
            vista.mostrarError("Ingrese el valor que desea convertir.");
            return;
        }

        final double valor;
        try {
            valor = Double.parseDouble(textoValor.replace(',', '.'));
        } catch (NumberFormatException ex) {
            vista.mostrarError("El valor debe ser un número válido.");
            return;
        }

        if (!Double.isFinite(valor)) {
            vista.mostrarError("El valor debe ser un número finito.");
            return;
        }
        if (tipo == TipoConversion.MASA && valor < 0) {
            vista.mostrarError("La masa no puede ser negativa.");
            return;
        }

        String origen = vista.getUnidadOrigen();
        String destino = vista.getUnidadDestino();
        if (origen == null || destino == null) {
            vista.mostrarError("Seleccione las unidades de origen y destino.");
            return;
        }

        vista.limpiarMensaje();
        vista.setProcesando(true);
        new SwingWorker<ResultadoConversion, Void>() {
            @Override
            protected ResultadoConversion doInBackground() {
                double resultado = solicitarConversion(valor, origen, destino);
                return new ResultadoConversion(origen, destino, resultado, valor);
            }

            @Override
            protected void done() {
                vista.setProcesando(false);
                try {
                    vista.mostrarResultado(get());
                } catch (InterruptedException ex) {
                    Thread.currentThread().interrupt();
                    vista.mostrarError("La conversión fue interrumpida.");
                } catch (ExecutionException ex) {
                    vista.mostrarError(obtenerMensaje(ex.getCause()));
                }
            }
        }.execute();
    }

    private double solicitarConversion(double valor, String origen, String destino) {
        return switch (tipo) {
            case TEMPERATURA -> cliente.convertirTemperatura(valor, origen, destino);
            case MASA -> cliente.convertirMasa(valor, origen, destino);
            case LONGITUD -> cliente.convertirLongitud(valor, origen, destino);
        };
    }

    private String obtenerMensaje(Throwable error) {
        String mensaje = error == null ? null : error.getMessage();
        if (mensaje == null || mensaje.isBlank()) {
            return "No se pudo completar la conversión. Verifique la conexión con el servidor.";
        }
        return mensaje;
    }
}
