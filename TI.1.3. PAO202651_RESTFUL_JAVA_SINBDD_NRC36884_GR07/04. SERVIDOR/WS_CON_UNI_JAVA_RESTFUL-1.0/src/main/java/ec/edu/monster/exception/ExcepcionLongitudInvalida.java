package ec.edu.monster.exception;

public class ExcepcionLongitudInvalida extends IllegalArgumentException {

    public ExcepcionLongitudInvalida(String mensaje) {
        super(mensaje);
    }

    public ExcepcionLongitudInvalida(String mensaje, Throwable causa) {
        super(mensaje, causa);
    }
}