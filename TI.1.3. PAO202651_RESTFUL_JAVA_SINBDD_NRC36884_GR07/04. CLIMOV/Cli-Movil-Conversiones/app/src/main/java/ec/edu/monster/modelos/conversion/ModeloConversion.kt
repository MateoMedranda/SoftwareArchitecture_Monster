package ec.edu.monster.modelos.conversion

import ec.edu.monster.modelos.ExcepcionMantenimiento
import ec.edu.monster.modelos.ExcepcionServidor
import ec.edu.monster.modelos.ExcepcionSesionVencida
import ec.edu.monster.modelos.entidades.CategoriaUnidad
import ec.edu.monster.modelos.entidades.Unidad
import ec.edu.monster.red.ClienteRest
import ec.edu.monster.red.ProveedorServicioConversion
import ec.edu.monster.utilidades.Constantes
import ec.edu.monster.utilidades.UtilidadesRed
import retrofit2.HttpException
import java.io.IOException

/**
 * Logica de negocio de la conversion de unidades.
 * MASA opera en modo mantenimiento hasta que el backend publique su endpoint.
 */
class ModeloConversion(
    private val cliente: ClienteRest,
    private val categoria: CategoriaUnidad
) {

    suspend fun convertir(valor: Double, origen: Unidad, destino: Unidad): RespuestaConversion {
        // Masa semi-funcional: no se realiza la llamada HTTP mientras
        // Constantes.MASA_DISPONIBLE sea false.
        if (categoria == CategoriaUnidad.MASA && !Constantes.MASA_DISPONIBLE) {
            throw ExcepcionMantenimiento()
        }

        val servicio = cliente.crearServicio(ProveedorServicioConversion::class.java)
        val peticion = PeticionConversion(valor, origen.clave, destino.clave)

        return try {
            when (categoria) {
                CategoriaUnidad.LONGITUD -> servicio.convertirLongitud(peticion)
                CategoriaUnidad.TEMPERATURA -> servicio.convertirTemperatura(peticion)
                CategoriaUnidad.MASA -> servicio.convertirMasa(peticion)
            }
        } catch (excepcion: HttpException) {
            if (excepcion.code() == HTTP_NO_AUTORIZADO) {
                throw ExcepcionSesionVencida()
            }
            throw ExcepcionServidor(UtilidadesRed.mensajeDeError(excepcion))
        } catch (excepcion: IOException) {
            throw ExcepcionServidor("No se pudo conectar con el servidor")
        }
    }

    private companion object {
        const val HTTP_NO_AUTORIZADO = 401
    }
}
