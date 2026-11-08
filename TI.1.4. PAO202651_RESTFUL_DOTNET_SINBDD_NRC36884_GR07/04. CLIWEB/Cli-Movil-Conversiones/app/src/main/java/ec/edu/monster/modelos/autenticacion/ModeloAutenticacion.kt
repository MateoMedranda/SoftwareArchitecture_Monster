package ec.edu.monster.modelos.autenticacion

import ec.edu.monster.modelos.ExcepcionAutenticacion
import ec.edu.monster.red.ClienteRest
import ec.edu.monster.red.ProveedorServicioAutenticacion
import ec.edu.monster.utilidades.UtilidadesRed
import retrofit2.HttpException
import java.io.IOException

/** Logica de negocio del inicio de sesion. */
class ModeloAutenticacion(private val cliente: ClienteRest) {

    suspend fun iniciarSesion(usuario: String, contrasena: String): RespuestaInicioSesion {
        val servicio = cliente.crearServicio(ProveedorServicioAutenticacion::class.java)

        return try {
            servicio.iniciarSesion(PeticionInicioSesion(usuario, contrasena))
        } catch (excepcion: HttpException) {
            throw ExcepcionAutenticacion(UtilidadesRed.mensajeDeError(excepcion))
        } catch (excepcion: IOException) {
            throw ExcepcionAutenticacion("No se pudo conectar con el servidor")
        }
    }
}
