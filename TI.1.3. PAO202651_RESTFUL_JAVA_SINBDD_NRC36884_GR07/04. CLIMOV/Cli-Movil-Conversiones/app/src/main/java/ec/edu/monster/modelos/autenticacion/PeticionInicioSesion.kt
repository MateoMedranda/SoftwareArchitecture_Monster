package ec.edu.monster.modelos.autenticacion

data class PeticionInicioSesion(
    val usuario: String,
    val contrasena: String
)
