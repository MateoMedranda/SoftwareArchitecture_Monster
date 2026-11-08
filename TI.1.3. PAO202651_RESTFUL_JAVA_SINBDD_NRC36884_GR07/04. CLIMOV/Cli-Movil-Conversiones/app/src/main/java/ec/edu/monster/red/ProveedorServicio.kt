package ec.edu.monster.red

import ec.edu.monster.BuildConfig
import ec.edu.monster.modelos.autenticacion.PeticionInicioSesion
import ec.edu.monster.modelos.autenticacion.RespuestaInicioSesion
import ec.edu.monster.modelos.conversion.PeticionConversion
import ec.edu.monster.modelos.conversion.RespuestaConversion
import retrofit2.http.Body
import retrofit2.http.POST

/** Endpoints de autenticacion (no requieren token). */
interface ProveedorServicioAutenticacion {

    @POST(BuildConfig.RUTA_AUTENTICACION)
    suspend fun iniciarSesion(@Body peticion: PeticionInicioSesion): RespuestaInicioSesion
}

/** Endpoints de conversion (requieren cabecera Authorization: Bearer). */
interface ProveedorServicioConversion {

    @POST(BuildConfig.RUTA_LONGITUD)
    suspend fun convertirLongitud(@Body peticion: PeticionConversion): RespuestaConversion

    @POST(BuildConfig.RUTA_TEMPERATURA)
    suspend fun convertirTemperatura(@Body peticion: PeticionConversion): RespuestaConversion

    @POST(BuildConfig.RUTA_MASA)
    suspend fun convertirMasa(@Body peticion: PeticionConversion): RespuestaConversion
}
