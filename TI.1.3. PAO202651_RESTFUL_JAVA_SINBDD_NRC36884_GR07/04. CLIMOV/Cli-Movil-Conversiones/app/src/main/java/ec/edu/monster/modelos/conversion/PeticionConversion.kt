package ec.edu.monster.modelos.conversion

/** Cuerpo de POST /conversion/{longitud|temperatura|masa} */
data class PeticionConversion(
    val valor: Double,
    val unidadOrigen: String,
    val unidadDestino: String
)
