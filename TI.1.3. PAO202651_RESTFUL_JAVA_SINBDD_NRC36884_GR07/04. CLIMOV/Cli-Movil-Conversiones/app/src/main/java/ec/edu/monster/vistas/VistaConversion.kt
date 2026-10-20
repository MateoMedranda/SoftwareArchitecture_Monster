package ec.edu.monster.vistas

import ec.edu.monster.modelos.entidades.Unidad

/** Contrato que el controlador usa para actualizar una pestana del conversor. */
interface VistaConversion {
    fun mostrarTitulo(titulo: String)
    fun mostrarUnidades(unidades: List<Unidad>, indiceOrigen: Int, indiceDestino: Int)
    fun mostrarSimbolos(origen: String, destino: String)
    fun mostrarResultado(texto: String)
    fun limpiarResultado()
    /** Pasa null para quitar el error. */
    fun mostrarErrorEntrada(mensaje: String?)
    /** Error de comunicacion o del servicio. */
    fun mostrarError(mensaje: String)
    fun mostrarCargando(mostrar: Boolean)
    fun mostrarErrorSesionVencida()
    /** Endpoint de masa todavia no disponible en el servidor. */
    fun mostrarMensajeMantenimiento()
}
