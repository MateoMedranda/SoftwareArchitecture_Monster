package ec.edu.monster.controladores;

import ec.edu.monster.modelos.ResultadoConversion;
import ec.edu.monster.modelos.TipoConversion;
import ec.edu.monster.modelos.excepciones.ExcepcionSesionRechazada;
import ec.edu.monster.servicios.ExcepcionServicio;
import ec.edu.monster.servicios.WSConversorCliente;
import ec.edu.monster.vistas.ConsolaVista;

/** Orquesta la conversión entre la vista de consola y el servicio SOAP. */
public class ConversionController {
    private final ConsolaVista vista;
    private final WSConversorCliente cliente;

    public ConversionController(ConsolaVista vista, WSConversorCliente cliente) {
        this.vista = vista;
        this.cliente = cliente;
    }

    public void convertir(TipoConversion tipo) {
        vista.mostrarMensaje("\n--- Conversión de " + tipo.titulo() + " ---");
        double valor = vista.solicitarValor();
        if (tipo != TipoConversion.TEMPERATURA && valor < 0) {
            vista.mostrarError("No se deben ingresar valores negativos");
            return;
        }
        String origen = vista.seleccionarUnidad("Unidad de origen:", tipo.unidades());
        String destino = vista.seleccionarUnidad("Unidad de destino:", tipo.unidades());
        try {
            double resultado = solicitarConversion(tipo, valor, origen, destino);
            vista.mostrarConversion(tipo, new ResultadoConversion(valor, origen, destino, resultado));
        } catch (ExcepcionServicio ex) {
            if (ex.sesionRechazada()) {
                //Reenvía el error a capa superior
                throw new ExcepcionSesionRechazada(ex.respuestaError().getMensaje());
            } else if (ex.respuestaError() != null) {
                vista.mostrarError(ex.respuestaError());
            } else {
                vista.mostrarError(ex.getMessage());
            }
        } catch (RuntimeException ex) {
            vista.mostrarError(mensajeSeguro(ex));
        }
    }

    private double solicitarConversion(TipoConversion tipo, double valor, String origen, String destino) {
        return switch (tipo) {
            case TEMPERATURA -> cliente.convertirTemperatura(valor, origen, destino);
            case MASA -> cliente.convertirMasa(valor, origen, destino);
            case LONGITUD -> cliente.convertirLongitud(valor, origen, destino);
        };
    }

    private String mensajeSeguro(RuntimeException ex) {
        String mensaje = ex.getMessage();
        return mensaje == null || mensaje.isBlank()
                ? "No se pudo completar la conversión. Verifique la conexión o su sesión."
                : mensaje;
    }
    
    
}
