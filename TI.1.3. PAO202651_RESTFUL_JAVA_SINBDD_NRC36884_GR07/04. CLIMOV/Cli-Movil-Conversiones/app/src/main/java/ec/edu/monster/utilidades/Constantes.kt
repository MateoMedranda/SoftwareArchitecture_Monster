package ec.edu.monster.utilidades

import android.os.Build
import ec.edu.monster.BuildConfig

/**
 * Configuracion de conexion del cliente movil.
 * Los valores se inyectan en tiempo de compilacion desde el
 * .development.env del proyecto (ver app/build.gradle.kts).
 */
object Constantes {

    /** URL del servidor: 10.0.2.2 en el emulador, IP LAN en un dispositivo fisico. */
    val URL_BASE: String =
        if (esEmulador()) BuildConfig.URL_BASE_EMULADOR else BuildConfig.URL_BASE_LOCAL

    val URL_BASE_EMULADOR: String = BuildConfig.URL_BASE_EMULADOR

    val URL_BASE_LOCAL: String = BuildConfig.URL_BASE_LOCAL

    val CABECERA_AUTORIZACION: String = BuildConfig.CABECERA_AUTORIZACION

    val PREFIJO_BEARER: String = BuildConfig.TIPO_TOKEN + " "

    /** true: la pestana de masa llama al endpoint; false: mensaje de mantenimiento. */
    val MASA_DISPONIBLE: Boolean = BuildConfig.MASA_DISPONIBLE

    private fun esEmulador(): Boolean =
        Build.FINGERPRINT.startsWith("generic") ||
            Build.FINGERPRINT.startsWith("unknown") ||
            Build.MODEL.contains("google_sdk") ||
            Build.MODEL.contains("Emulator") ||
            Build.MODEL.contains("Android SDK built for") ||
            Build.HARDWARE.contains("goldfish") ||
            Build.HARDWARE.contains("ranchu") ||
            Build.PRODUCT.contains("sdk") ||
            Build.PRODUCT.contains("emulator") ||
            Build.PRODUCT.contains("simulator")
}
