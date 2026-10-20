package ec.edu.monster.modelos.conversion

/** Respuesta de POST /conversion/{longitud|temperatura|masa} */
data class RespuestaConversion(
    val valorOriginal: Double,
    val valorConvertido: Double,
    val unidadOrigen: String,
    val unidadDestino: String
)
