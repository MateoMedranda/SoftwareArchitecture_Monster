package ec.edu.monster.modelos.Respuestas;

public class TransaccionExcepcion extends Exception {

    private RespuestaError faultInfo;

    public TransaccionExcepcion() {
        super();
    }

    public TransaccionExcepcion(String message, RespuestaError faultInfo) {
        super(message);
        this.faultInfo = faultInfo;
    }

    public TransaccionExcepcion(String message, RespuestaError faultInfo, Throwable cause) {
        super(message, cause);
        this.faultInfo = faultInfo;
    }

    public RespuestaError getFaultInfo() {
        return faultInfo;
    }
}
