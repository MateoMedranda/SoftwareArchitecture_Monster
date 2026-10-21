package ec.edu.monster.model;

import java.io.Serializable;
import java.time.LocalDateTime;
import org.eclipse.microprofile.openapi.annotations.enums.SchemaType;
import org.eclipse.microprofile.openapi.annotations.media.Schema;

@Schema(description = "Estructura devuelta cuando ocurre un error en la solicitud")
public class RespuestaError implements Serializable {

    @Schema(description = "Código HTTP del error", type = SchemaType.INTEGER, example = "400")
    private int codigo;
    @Schema(description = "Descripción breve del error", type = SchemaType.STRING, example = "Longitud Inválida")
    private String mensaje;
    @Schema(description = "Detalle de la causa del error", type = SchemaType.STRING, example = "No se permite ingresar valores negativos")
    private String detalles;
    @Schema(description = "Fecha y hora en que se generó el error", type = SchemaType.STRING, format = "date-time", example = "2026-10-18T19:22:42")
    private LocalDateTime fechaHora;

    public RespuestaError() {
        this.fechaHora = LocalDateTime.now();
    }

    public RespuestaError(int codigo, String mensaje, String detalles) {
        this();
        this.codigo = codigo;
        this.mensaje = mensaje;
        this.detalles = detalles;
    }

    public int getCodigo() {
        return codigo;
    }

    public void setCodigo(int codigo) {
        this.codigo = codigo;
    }

    public String getMensaje() {
        return mensaje;
    }

    public void setMensaje(String mensaje) {
        this.mensaje = mensaje;
    }

    public String getDetalles() {
        return detalles;
    }

    public void setDetalles(String detalles) {
        this.detalles = detalles;
    }

    public LocalDateTime getFechaHora() {
        return fechaHora;
    }

    public void setFechaHora(LocalDateTime fechaHora) {
        this.fechaHora = fechaHora;
    }
}