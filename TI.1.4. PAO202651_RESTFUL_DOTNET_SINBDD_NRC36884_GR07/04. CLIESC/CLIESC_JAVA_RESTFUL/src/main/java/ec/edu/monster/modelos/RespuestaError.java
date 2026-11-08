package ec.edu.monster.modelos;

/**
 * Error estructurado que devuelve el servicio RESTful (esquema RespuestaError
 * de la especificación OpenAPI): código, mensaje, detalles y fecha/hora.
 */
public class RespuestaError {
    private Integer codigo;
    private String mensaje;
    private String detalles;
    private String fechaHora;

    public Integer getCodigo() { return codigo; }
    public void setCodigo(Integer codigo) { this.codigo = codigo; }

    public String getMensaje() { return mensaje; }
    public void setMensaje(String mensaje) { this.mensaje = mensaje; }

    public String getDetalles() { return detalles; }
    public void setDetalles(String detalles) { this.detalles = detalles; }

    public String getFechaHora() { return fechaHora; }
    public void setFechaHora(String fechaHora) { this.fechaHora = fechaHora; }

    /**
     * Representación legible para la consola, por ejemplo:
     * [400] Longitud Inválida: No se permite ingresar valores negativos (2026-10-18T19:22:42).
     * Se omiten los campos que no vengan informados.
     */
    public String formatear() {
        StringBuilder texto = new StringBuilder();
        if (codigo != null) {
            texto.append('[').append(codigo).append("] ");
        }
        texto.append(conPorDefecto(mensaje, "Error devuelto por el servicio RESTful"));
        if (detalles != null && !detalles.isBlank()) {
            texto.append(": ").append(detalles.trim());
        }
        if (fechaHora != null && !fechaHora.isBlank()) {
            texto.append(" (").append(fechaHora.trim()).append(')');
        }
        return texto.toString();
    }

    private static String conPorDefecto(String valor, String porDefecto) {
        return valor == null || valor.isBlank() ? porDefecto : valor.trim();
    }
}
