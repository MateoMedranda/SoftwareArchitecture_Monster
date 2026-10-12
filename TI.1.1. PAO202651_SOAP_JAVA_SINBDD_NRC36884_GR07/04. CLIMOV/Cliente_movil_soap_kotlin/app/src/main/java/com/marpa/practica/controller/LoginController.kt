package com.marpa.practica.controller

import com.marpa.practica.model.AuthModel
import com.marpa.practica.model.entities.LoginResult
import com.marpa.practica.model.storage.SessionStore
import com.marpa.practica.view.LoginView
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

/**
 * Controlador: recibe eventos de la vista, valida, llama al modelo
 * y le indica a la vista qué mostrar. No usa clases de UI de Android.
 */
class LoginController(
    private val view: LoginView,
    private val model: AuthModel,
    private val session: SessionStore,
    private val scope: CoroutineScope
) {

    fun onLoginClicked(username: String, password: String) {
        val user = username.trim()
        if (user.isEmpty()) {
            view.showUsernameError("Ingresa tu usuario")
            return
        }
        if (password.isEmpty()) {
            view.showPasswordError("Ingresa tu contraseña")
            return
        }

        view.showLoading(true)
        scope.launch {
            // La llamada SOAP es bloqueante: se ejecuta fuera del hilo principal.
            val result = withContext(Dispatchers.IO) { model.login(user, password) }
            view.showLoading(false)
            when (result) {
                is LoginResult.Success -> {
                    session.saveToken(result.token)
                    view.navigateToHome(result.token)
                }
                is LoginResult.Failure -> view.showError(result.message)
            }
        }
    }
}
