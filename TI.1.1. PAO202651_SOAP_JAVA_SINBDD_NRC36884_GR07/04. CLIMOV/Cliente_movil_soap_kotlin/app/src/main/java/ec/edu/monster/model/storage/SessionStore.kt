package ec.edu.monster.model.storage

import android.content.Context

/**
 * Guarda el token de sesión. Para producción, cambia a EncryptedSharedPreferences
 * (androidx.security:security-crypto) o DataStore cifrado.
 */
class SessionStore(context: Context) {
    private val prefs = context.getSharedPreferences("session", Context.MODE_PRIVATE)

    fun saveToken(token: String) = prefs.edit().putString(KEY_TOKEN, token).apply()
    fun getToken(): String? = prefs.getString(KEY_TOKEN, null)
    fun clear() = prefs.edit().remove(KEY_TOKEN).apply()

    private companion object {
        const val KEY_TOKEN = "token"
    }
}
