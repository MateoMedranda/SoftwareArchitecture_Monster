package ec.edu.monster.modelos.conversion

/** Respuesta de error del servidor (RespuestaError.java) */
data class RespuestaErrorServidor(
    val codigo: Int = 0,
    val mensaje: String? = null,
    val detalles: String? = null,
    val fechaHora: String? = null
)
