package ec.edu.monster.exception;

public class ExcepcionTemperaturaInvalida extends IllegalArgumentException {

    public ExcepcionTemperaturaInvalida(String mensaje) {
        super(mensaje);
    }

    public ExcepcionTemperaturaInvalida(String mensaje, Throwable causa) {
        super(mensaje, causa);
    }
}