package ec.edu.monster.controladores

import androidx.lifecycle.LifecycleCoroutineScope
import ec.edu.monster.modelos.ExcepcionAutenticacion
import ec.edu.monster.modelos.autenticacion.ModeloAutenticacion
import ec.edu.monster.modelos.datos.GestorSesion
import ec.edu.monster.vistas.VistaInicioSesion
import kotlinx.coroutines.launch

/** Controlador del inicio de sesion: valida, llama al modelo y gestiona la sesion. */
class ControladorInicioSesion(
    private val vista: VistaInicioSesion,
    private val modelo: ModeloAutenticacion,
    private val sesion: GestorSesion,
    private val ambito: LifecycleCoroutineScope
) {

    fun enClicIniciarSesion(usuario: String, contrasena: String) {
        if (usuario.isBlank()) {
            vista.mostrarErrorUsuario("Ingrese el usuario")
            return
        }
        if (contrasena.isBlank()) {
            vista.mostrarErrorContrasena("Ingrese la contraseña")
            return
        }

        vista.mostrarCargando(true)
        ambito.launch {
            try {
                val respuesta = modelo.iniciarSesion(usuario.trim(), contrasena)
                sesion.guardarToken(respuesta.token)
                vista.navegarAlConversor(respuesta.token)
            } catch (excepcion: ExcepcionAutenticacion) {
                vista.mostrarError(excepcion.message)
            } finally {
                vista.mostrarCargando(false)
            }
        }
    }
}
