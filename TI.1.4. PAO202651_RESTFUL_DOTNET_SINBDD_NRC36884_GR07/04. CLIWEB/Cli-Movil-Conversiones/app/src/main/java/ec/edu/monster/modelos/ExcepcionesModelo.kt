package ec.edu.monster.modelos

/** Excepcion del cliente: credenciales rechazadas o fallo de autenticacion. */
class ExcepcionAutenticacion(override val message: String) : Exception(message)

/** Excepcion del cliente: error de comunicacion o de negocio del servidor. */
class ExcepcionServidor(val mensaje: String) : Exception(mensaje)

/** El servidor rechazo el token (HTTP 401): la sesion ya no es valida. */
class ExcepcionSesionVencida : Exception("Sesión vencida")

/** El endpoint solicitado aun no esta disponible en el servidor. */
class ExcepcionMantenimiento : Exception("Servicio en mantenimiento")
