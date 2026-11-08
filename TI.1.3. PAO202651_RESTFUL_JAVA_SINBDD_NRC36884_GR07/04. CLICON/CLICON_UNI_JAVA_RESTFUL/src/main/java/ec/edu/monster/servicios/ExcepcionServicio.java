package ec.edu.monster.servicios;

import ec.edu.monster.modelos.RespuestaError;

/**
 * Error detectado al consumir el servicio RESTful: falla de red/configuración
 * sin respuesta utilizable, o respuesta de error (4xx/5xx) del servicio.
 * Transporta el código HTTP real y el cuerpo estructurado cuando existen.
 */
public class ExcepcionServicio extends RuntimeException {

    private final Integer codigoHttp;
    private final transient RespuestaError respuestaError;

    private ExcepcionServicio(String mensaje, Integer codigoHttp, RespuestaError respuestaError, Throwable causa) {
        super(mensaje, causa);
        this.codigoHttp = codigoHttp;
        this.respuestaError = respuestaError;
    }

    /** Crea la excepción a partir de una respuesta de error del servicio (4xx/5xx). */
    public static ExcepcionServicio deRespuesta(int codigoHttp, RespuestaError error, String mensajeGenerico) {
        String mensaje = error == null ? mensajeGenerico : error.formatear();
        return new ExcepcionServicio(mensaje, codigoHttp, error, null);
    }

    /** Falla de red, de timeout o de conexión: no hubo respuesta del servicio. */
    public static ExcepcionServicio deRed(String mensaje, Throwable causa) {
        return new ExcepcionServicio(mensaje, null, null, causa);
    }

    /** Respuesta recibida que no corresponde a lo esperado por la especificación. */
    public static ExcepcionServicio inesperada(String mensaje, Throwable causa) {
        return new ExcepcionServicio(mensaje, null, null, causa);
    }

    /** Código HTTP real devuelto por el servicio, o {@code null} si no hubo respuesta. */
    public Integer codigoHttp() { return codigoHttp; }

    /** Cuerpo estructurado del error, o {@code null} si no estaba disponible. */
    public RespuestaError respuestaError() { return respuestaError; }

    /** Indica si el servicio rechazó la petición por token ausente o inválido (401). */
    public boolean sesionRechazada() { return codigoHttp != null && codigoHttp == 401; }
}
