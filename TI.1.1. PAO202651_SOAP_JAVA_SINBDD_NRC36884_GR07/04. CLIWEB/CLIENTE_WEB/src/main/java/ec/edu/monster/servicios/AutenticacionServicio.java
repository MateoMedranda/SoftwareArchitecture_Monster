package ec.edu.monster.servicios;

import ec.edu.monster.resources.WSConversorUnidades;
 import jakarta.enterprise.context.ApplicationScoped;
import java.io.Serializable;


@ApplicationScoped
public class AutenticacionServicio implements Serializable {

    private static final long serialVersionUID = 1L;

    /**
     * @return el token devuelto por el servidor.
     */
    public String login(String usuario, String contrasenia) {
        WSConversorUnidades port = ClienteSOAPFactory.crearPuerto(null);
        return port.login(usuario, contrasenia);
    }
}
