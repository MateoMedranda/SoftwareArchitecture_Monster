package com.marpa.practica.view

/** Contrato que el controlador usa para actualizar la pantalla de login. */
interface LoginView {
    fun showLoading(show: Boolean)
    fun showUsernameError(message: String)
    fun showPasswordError(message: String)
    fun showError(message: String)
    fun navigateToHome(token: String)
}
