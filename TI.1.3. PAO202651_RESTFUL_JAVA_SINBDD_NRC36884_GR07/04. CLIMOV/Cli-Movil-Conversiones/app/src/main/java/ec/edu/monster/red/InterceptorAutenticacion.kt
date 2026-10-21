package ec.edu.monster.red

import ec.edu.monster.modelos.datos.GestorSesion
import ec.edu.monster.utilidades.Constantes
import okhttp3.Interceptor
import okhttp3.Response

/** Anade la cabecera Authorization: Bearer <token> a cada peticion saliente. */
class InterceptorAutenticacion(
    private val gestorSesion: GestorSesion
) : Interceptor {

    override fun intercept(cadena: Interceptor.Chain): Response {
        val token = gestorSesion.obtenerToken()

        val peticion = if (!token.isNullOrBlank()) {
            cadena.request().newBuilder()
                .header(Constantes.CABECERA_AUTORIZACION, Constantes.PREFIJO_BEARER + token)
                .build()
        } else {
            cadena.request()
        }

        return cadena.proceed(peticion)
    }
}
