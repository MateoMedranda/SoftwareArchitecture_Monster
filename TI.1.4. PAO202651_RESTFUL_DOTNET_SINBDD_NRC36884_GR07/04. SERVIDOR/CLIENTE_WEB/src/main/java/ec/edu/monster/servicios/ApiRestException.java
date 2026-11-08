package ec.edu.monster.servicios;

public class ApiRestException extends RuntimeException {

    public ApiRestException(String mensaje) {
        super(mensaje);
    }

    public ApiRestException(String mensaje, Throwable causa) {
        super(mensaje, causa);
    }
}
