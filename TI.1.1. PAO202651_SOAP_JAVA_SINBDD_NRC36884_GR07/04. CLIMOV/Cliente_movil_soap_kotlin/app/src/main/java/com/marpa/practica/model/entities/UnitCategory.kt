package com.marpa.practica.model.entities

enum class UnitCategory (val title: String, val soapOperation: String) {
    LENGTH("Longitud", "convertirLongitud"),
    MASS("Masa", "convertirMasa"),
    TEMPERATURE("Temperatura", "convertirTemperatura")
}