package ec.edu.monster.modelos.autenticacion

/** Respuesta de POST /autenticacion/iniciar-sesion */
data class RespuestaInicioSesion(
    val token: String,
    val expiraEn: Long
)
