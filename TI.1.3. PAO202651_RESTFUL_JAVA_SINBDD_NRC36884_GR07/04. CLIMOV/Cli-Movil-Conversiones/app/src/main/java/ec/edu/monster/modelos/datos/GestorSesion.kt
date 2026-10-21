package ec.edu.monster.modelos.datos

import android.content.Context
import android.content.SharedPreferences
import androidx.security.crypto.EncryptedSharedPreferences
import androidx.security.crypto.MasterKey

/** Persiste el token JWT de la sesion en preferencias encriptadas. */
class GestorSesion(contexto: Context) {

    private val preferencias: SharedPreferences

    init {
        val llaveMaestra = MasterKey.Builder(contexto.applicationContext)
            .setKeyScheme(MasterKey.KeyScheme.AES256_GCM)
            .build()

        preferencias = EncryptedSharedPreferences.create(
            contexto.applicationContext,
            ARCHIVO_PREFERENCIAS,
            llaveMaestra,
            EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
            EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
        )
    }

    fun guardarToken(token: String) {
        preferencias.edit().putString(CLAVE_TOKEN, token).apply()
    }

    fun obtenerToken(): String? = preferencias.getString(CLAVE_TOKEN, null)

    fun eliminarToken() {
        preferencias.edit().remove(CLAVE_TOKEN).apply()
    }

    private companion object {
        const val ARCHIVO_PREFERENCIAS = "gestor_sesion"
        const val CLAVE_TOKEN = "token_autenticacion"
    }
}
