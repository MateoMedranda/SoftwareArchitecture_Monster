package com.marpa.practica.model

import com.marpa.practica.model.entities.Unit
import com.marpa.practica.model.entities.UnitCategory

object UnitCatalog {

    private fun defineUnit(name: String, symbol: String, soap_unit: String) =
        Unit(name, symbol, soap_unit)
    private val length = listOf(
        defineUnit("centímetro", "cm", "CENTIMETRO"),
        defineUnit("metro","m", "METRO"),
        defineUnit("pie","ft", "PIE"),
        defineUnit("yarda","yd", "YARDA"),
        defineUnit("milla","mi", "MILLA")
    )

    private val mass = listOf(
        defineUnit("kilogramo", "kg", "KILOGRAMO"),
        defineUnit("gramo", "g", "GRAMO"),
        defineUnit("tonelada", "ton", "TONELADA"),
        defineUnit("onza", "oz","ONZA"),
        defineUnit("miligramo","mg","MILIGRAMO")
    )

    private val temperature = listOf(
        defineUnit("Celsius", "°C", "CELSIUS"),
        defineUnit("Fahrenheit", "°F", "FAHRENHEIT"),
        defineUnit("Kelvin", "K", "KELVIN"),
        defineUnit("Rankine", "°R", "RANKINE")
    )

    fun unitsOf(category: UnitCategory): List<Unit> = when (category) {
        UnitCategory.LENGTH -> length
        UnitCategory.MASS -> mass
        UnitCategory.TEMPERATURE -> temperature
    }
}