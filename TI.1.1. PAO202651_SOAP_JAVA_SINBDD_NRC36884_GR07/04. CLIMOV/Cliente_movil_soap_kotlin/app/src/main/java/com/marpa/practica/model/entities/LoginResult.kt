package com.marpa.practica.model.entities

/** Resultado de un intento de login. */
sealed class LoginResult {
    data class Success(val token: String) : LoginResult()
    data class Failure(val message: String) : LoginResult()
}
