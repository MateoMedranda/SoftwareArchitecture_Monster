package ec.edu.monster.servicios;

import ec.edu.monster.resources.WSConversorUnidades;
import jakarta.enterprise.context.ApplicationScoped;
import java.io.Serializable;

/**
 * Servicio de conversion de unidades. Cada llamada adjunta el token de sesion
 * en el header SOAP mediante {@link ClienteSOAPFactory}.
 *
 */
@ApplicationScoped
public class ConversorServicio implements Serializable {

    private static final long serialVersionUID = 1L;

    public double convertirLongitud(String token, double valor, String de, String a) {
        WSConversorUnidades port = ClienteSOAPFactory.crearPuerto(token);
        return port.convertirLongitud(valor, de, a);
    }

    public double convertirMasa(String token, double valor, String de, String a) {
        WSConversorUnidades port = ClienteSOAPFactory.crearPuerto(token);
        return port.convertirMasa(valor, de, a);
    }

    public double convertirTemperatura(String token, double valor, String de, String a) {
        WSConversorUnidades port = ClienteSOAPFactory.crearPuerto(token);
        return port.convertirTemperatura(valor, de, a);
    }

    public String cambiarContrasenia(String token, String actual, String nueva) {
        WSConversorUnidades port = ClienteSOAPFactory.crearPuerto(token);
        return port.cambiarContrasenia(actual, nueva);
    }
}
