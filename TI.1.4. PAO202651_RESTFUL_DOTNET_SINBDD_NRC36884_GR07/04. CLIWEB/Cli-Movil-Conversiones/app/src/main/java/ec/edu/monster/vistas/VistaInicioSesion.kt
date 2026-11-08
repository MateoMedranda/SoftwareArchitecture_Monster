package ec.edu.monster.vistas

/** Contrato que el controlador usa para actualizar la pantalla de inicio de sesion. */
interface VistaInicioSesion {
    fun mostrarCargando(mostrar: Boolean)
    fun mostrarErrorUsuario(mensaje: String)
    fun mostrarErrorContrasena(mensaje: String)
    fun mostrarError(mensaje: String)
    fun navegarAlConversor(token: String)
}
