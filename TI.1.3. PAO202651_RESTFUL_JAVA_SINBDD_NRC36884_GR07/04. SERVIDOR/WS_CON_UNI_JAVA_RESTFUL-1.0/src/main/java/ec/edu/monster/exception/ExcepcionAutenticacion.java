package ec.edu.monster.exception;

public class ExcepcionAutenticacion extends RuntimeException {

    public ExcepcionAutenticacion(String mensaje) {
        super(mensaje);
    }

    public ExcepcionAutenticacion(String mensaje, Throwable causa) {
        super(mensaje, causa);
    }
}