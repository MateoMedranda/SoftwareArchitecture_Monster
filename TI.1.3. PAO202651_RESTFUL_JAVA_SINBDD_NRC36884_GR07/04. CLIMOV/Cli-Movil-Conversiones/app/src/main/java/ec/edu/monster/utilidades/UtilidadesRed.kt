package ec.edu.monster.utilidades

import ec.edu.monster.modelos.conversion.RespuestaErrorServidor
import com.google.gson.Gson
import retrofit2.HttpException

/** Traduce las respuestas de error del servidor REST a mensajes legibles. */
object UtilidadesRed {

    fun mensajeDeError(excepcion: HttpException): String {
        return try {
            val cuerpo = excepcion.response()?.errorBody()?.string()
            val error = Gson().fromJson(cuerpo, RespuestaErrorServidor::class.java)
            error?.detalles ?: error?.mensaje ?: "Error del servidor"
        } catch (e: Exception) {
            "Error del servidor (código ${excepcion.code()})"
        }
    }
}
