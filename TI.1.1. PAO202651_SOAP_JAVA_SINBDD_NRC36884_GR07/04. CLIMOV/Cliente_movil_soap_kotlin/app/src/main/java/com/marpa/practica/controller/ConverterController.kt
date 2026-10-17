package com.marpa.practica.controller

import android.R
import com.marpa.practica.model.UnitConverterModel
import com.marpa.practica.model.entities.UnitCategory
import com.marpa.practica.model.network.SessionExpiredException
import com.marpa.practica.model.network.SoapException
import com.marpa.practica.view.ConverterView
import java.math.BigDecimal
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.math.RoundingMode

/**
 * Controlador de UNA pestaña (una categoría). Guarda qué unidades hay seleccionadas,
 * valida la entrada, llama al modelo y le indica a la vista qué mostrar.
 */
class ConverterController(
    private val view: ConverterView,
    private val model: UnitConverterModel,
    private val category: UnitCategory,
    private val scope: CoroutineScope
) {
    private var fromIndex = 0
    private var toIndex = 1

    fun onViewCreated() {
        val units = model.unitsOf(category)
        view.showTitle("Convertidor de ${category.title}")
        view.showUnits(units, fromIndex, toIndex)
        refreshSymbols()
    }

    fun onFromSelected(index: Int) {
        fromIndex = index
        onSelectionChanged()
    }

    fun onToSelected(index: Int) {
        toIndex = index
        onSelectionChanged()
    }

    /** El usuario editó el valor: el resultado anterior ya no es válido. */
    fun onInputChanged() {
        view.clearResult()
        view.showInputError(null)
    }

    fun onConvertClicked(rawValue: String) {
        // Acepta "12,5" y "12.5" según el teclado del usuario.
        val value = rawValue.trim().replace(',', '.').toDoubleOrNull()
        if (value == null) {
            view.showInputError("Ingresa un número válido")
            return
        }
        view.showInputError(null)
        view.showLoading(true)
        scope.launch {
            try {
                val result = withContext(Dispatchers.IO) {
                    model.convert(category, value, fromIndex, toIndex)
                }
                view.showResult(format(result))
            }
            catch (e: SessionExpiredException) {
                view.clearResult()
                view.showSessionExpiredError()
            }
            catch (e: SoapException) {
                view.clearResult()
                view.showError(e.message ?: "Error de comunicación con el servidor")
            } finally {
                view.showLoading(false)
            }
        }
    }

    private fun onSelectionChanged() {
        refreshSymbols()
        view.clearResult()
    }

    private fun refreshSymbols() {
        val units = model.unitsOf(category)
        view.showSymbols(units[fromIndex].symbol, units[toIndex].symbol)
    }

    /** 10 cifras significativas, sin ceros sobrantes ni notación científica. */
    private fun format(value: Double): String =
        BigDecimal.valueOf(value)
            .setScale(3, RoundingMode.HALF_UP)
            .stripTrailingZeros()
            .toPlainString()
}
