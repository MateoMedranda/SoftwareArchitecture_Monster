package com.marpa.practica.model

import com.marpa.practica.model.entities.LoginResult
import com.marpa.practica.model.network.SoapClient
import com.marpa.practica.model.network.SoapException
import java.util.UUID

/** Lógica de negocio de autenticación (capa Modelo). No conoce vistas ni Android UI. */
class AuthModel(private val soap: SoapClient = SoapClient()) {

    /** Bloqueante: ejecutar en Dispatchers.IO. */
    fun login(username: String, password: String): LoginResult =
        try {
            val token = soap.login(username, password)
            // El servidor debe devolver un UUID; si no lo es, la respuesta es inválida.
            UUID.fromString(token)
            LoginResult.Success(token)
        } catch (e: IllegalArgumentException) {
            LoginResult.Failure("El servidor devolvió un token inválido")
        } catch (e: SoapException) {
            LoginResult.Failure(e.message ?: "Error de comunicación con el servidor")
        }
}
