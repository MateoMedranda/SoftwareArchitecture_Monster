package ec.edu.monster.modelos.entidades

/**
 * Unidad de medida.
 * @param clave nombre exacto que espera el servidor (enum UnidadLongitud / UnidadTemperatura)
 * @param nombre nombre para mostrar en la interfaz
 * @param simbolo simbolo corto (m, cm, °C...)
 */
data class Unidad(
    val clave: String,
    val nombre: String,
    val simbolo: String
)
