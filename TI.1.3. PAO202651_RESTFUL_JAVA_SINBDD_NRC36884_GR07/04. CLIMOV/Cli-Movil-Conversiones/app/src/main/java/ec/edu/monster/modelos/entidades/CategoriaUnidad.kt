package ec.edu.monster.modelos.entidades

/**
 * Categorias de conversion (pestanas del conversor).
 * Las claves de unidad coinciden con los enums del servidor
 * (UnidadLongitud, UnidadTemperatura y UnidadMasa).
 */
enum class CategoriaUnidad(val titulo: String, val unidades: List<Unidad>) {

    LONGITUD(
        "Longitud",
        listOf(
            Unidad("METRO", "Metro", "m"),
            Unidad("CENTIMETRO", "Centímetro", "cm"),
            Unidad("PIE", "Pie", "ft"),
            Unidad("YARDA", "Yarda", "yd"),
            Unidad("MILLA", "Milla", "mi")
        )
    ),

    TEMPERATURA(
        "Temperatura",
        listOf(
            Unidad("CELSIUS", "Celsius", "°C"),
            Unidad("FAHRENHEIT", "Fahrenheit", "°F"),
            Unidad("KELVIN", "Kelvin", "K"),
            Unidad("RANKINE", "Rankine", "°R")
        )
    ),

    MASA(
        "Masa",
        listOf(
            Unidad("GRAMO", "Gramo", "g"),
            Unidad("KILOGRAMO", "Kilogramo", "kg"),
            Unidad("MILIGRAMO", "Miligramo", "mg"),
            Unidad("TONELADA", "Tonelada", "t"),
            Unidad("ONZA", "Onza", "oz")
        )
    )
}
