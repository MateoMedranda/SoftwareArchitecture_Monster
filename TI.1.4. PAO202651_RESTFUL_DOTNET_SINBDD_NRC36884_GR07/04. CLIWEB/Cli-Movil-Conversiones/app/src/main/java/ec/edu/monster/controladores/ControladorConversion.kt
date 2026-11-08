package ec.edu.monster.controladores

import androidx.lifecycle.LifecycleCoroutineScope
import ec.edu.monster.modelos.ExcepcionMantenimiento
import ec.edu.monster.modelos.ExcepcionServidor
import ec.edu.monster.modelos.ExcepcionSesionVencida
import ec.edu.monster.modelos.conversion.ModeloConversion
import ec.edu.monster.modelos.entidades.CategoriaUnidad
import ec.edu.monster.vistas.VistaConversion
import kotlinx.coroutines.launch
import java.util.Locale

/** Controlador de una pestana del conversor: validacion, llamada y presentacion. */
class ControladorConversion(
    private val vista: VistaConversion,
    private val modelo: ModeloConversion,
    private val categoria: CategoriaUnidad,
    private val ambito: LifecycleCoroutineScope
) {

    private var indiceOrigen = 0
    private var indiceDestino = 1

    fun trasCrearVista() {
        vista.mostrarTitulo(categoria.titulo)
        vista.mostrarUnidades(categoria.unidades, indiceOrigen, indiceDestino)
        actualizarSimbolos()
    }

    fun enSeleccionOrigen(posicion: Int) {
        indiceOrigen = posicion
        actualizarSimbolos()
        vista.limpiarResultado()
    }

    fun enSeleccionDestino(posicion: Int) {
        indiceDestino = posicion
        actualizarSimbolos()
        vista.limpiarResultado()
    }

    fun enCambioEntrada() {
        vista.mostrarErrorEntrada(null)
        vista.limpiarResultado()
    }

    fun enClicConvertir(entrada: String) {
        val texto = entrada.trim().replace(',', '.')

        if (texto.isEmpty()) {
            vista.mostrarErrorEntrada("Ingrese un valor a convertir")
            return
        }

        val valor = texto.toDoubleOrNull()
        if (valor == null) {
            vista.mostrarErrorEntrada("Valor no válido")
            return
        }

        // La temperatura admite valores negativos (bajo cero); el servidor valida
        // el rango fisico por unidad. Longitud y masa no permiten negativos.
        if (categoria != CategoriaUnidad.TEMPERATURA && valor < 0) {
            vista.mostrarErrorEntrada("No se permiten valores negativos")
            return
        }

        vista.mostrarErrorEntrada(null)
        vista.mostrarCargando(true)

        ambito.launch {
            try {
                val respuesta = modelo.convertir(
                    valor,
                    categoria.unidades[indiceOrigen],
                    categoria.unidades[indiceDestino]
                )
                vista.mostrarResultado(formatearResultado(respuesta.valorConvertido))
            } catch (excepcion: ExcepcionMantenimiento) {
                vista.mostrarMensajeMantenimiento()
            } catch (excepcion: ExcepcionSesionVencida) {
                vista.mostrarErrorSesionVencida()
            } catch (excepcion: ExcepcionServidor) {
                vista.mostrarError(excepcion.mensaje)
            } finally {
                vista.mostrarCargando(false)
            }
        }
    }

    private fun actualizarSimbolos() {
        vista.mostrarSimbolos(
            categoria.unidades[indiceOrigen].simbolo,
            categoria.unidades[indiceDestino].simbolo
        )
    }

    /** Limita el resultado a 4 decimales y quita los ceros sobrantes. */
    private fun formatearResultado(valor: Double): String {
        val formateado = String.format(Locale.getDefault(), "%.4f", valor)
        return formateado.trimEnd('0').trimEnd(',', '.')
    }
}
