package ec.edu.monster.view

import ec.edu.monster.model.entities.Unit

interface ConverterView {
    fun showTitle(title: String)
    fun showUnits(units : List<Unit>, fromIndex: Int, toIndex: Int)
    fun showSymbols(from: String, to: String)
    fun showResult(text: String)
    fun clearResult()
    /** Pasa null para quitar el error. */
    fun showInputError(message: String?)
    /** Error de comunicación o del servicio. */
    fun showError(message: String)
    fun showLoading(show: Boolean)

    fun showSessionExpiredError()
}