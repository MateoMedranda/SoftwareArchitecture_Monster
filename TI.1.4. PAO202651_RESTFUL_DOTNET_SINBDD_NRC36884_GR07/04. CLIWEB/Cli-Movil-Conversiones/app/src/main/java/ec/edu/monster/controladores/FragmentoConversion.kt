package ec.edu.monster.controladores

import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.inputmethod.EditorInfo
import android.widget.ArrayAdapter
import android.widget.Filter
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.core.os.bundleOf
import androidx.core.widget.doAfterTextChanged
import androidx.fragment.app.Fragment
import androidx.lifecycle.lifecycleScope
import com.google.android.material.dialog.MaterialAlertDialogBuilder
import ec.edu.monster.R
import ec.edu.monster.databinding.FragmentConversionBinding
import ec.edu.monster.modelos.conversion.ModeloConversion
import ec.edu.monster.modelos.datos.GestorSesion
import ec.edu.monster.modelos.entidades.CategoriaUnidad
import ec.edu.monster.modelos.entidades.Unidad
import ec.edu.monster.red.ClienteRest
import ec.edu.monster.vistas.VistaConversion

/** Vista: una pestana del conversor. Se reutiliza para todas las categorias. */
class FragmentoConversion : Fragment(), VistaConversion {

    private var _vinculo: FragmentConversionBinding? = null
    private val vinculo get() = _vinculo!!
    private lateinit var controlador: ControladorConversion

    private var dialogoSesion: AlertDialog? = null

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _vinculo = FragmentConversionBinding.inflate(inflater, container, false)
        return vinculo.root
    }

    override fun onViewCreated(vista: View, savedInstanceState: Bundle?) {
        super.onViewCreated(vista, savedInstanceState)

        val categoria = CategoriaUnidad.valueOf(requireArguments().getString(ARG_CATEGORIA)!!)
        val sesion = GestorSesion(requireContext().applicationContext)

        controlador = ControladorConversion(
            vista = this,
            modelo = ModeloConversion(ClienteRest(sesion), categoria),
            categoria = categoria,
            ambito = viewLifecycleOwner.lifecycleScope // se cancela solo al destruir la vista
        )

        vinculo.actOrigen.setOnItemClickListener { _, _, posicion, _ ->
            controlador.enSeleccionOrigen(posicion)
        }
        vinculo.actDestino.setOnItemClickListener { _, _, posicion, _ ->
            controlador.enSeleccionDestino(posicion)
        }
        vinculo.etValor.doAfterTextChanged { controlador.enCambioEntrada() }
        vinculo.etValor.setOnEditorActionListener { _, actionId, _ ->
            if (actionId == EditorInfo.IME_ACTION_DONE) {
                convertir()
                true
            } else {
                false
            }
        }
        vinculo.btnConvertir.setOnClickListener { convertir() }

        controlador.trasCrearVista()
    }

    private fun convertir() {
        controlador.enClicConvertir(vinculo.etValor.text?.toString().orEmpty())
    }

    // --- VistaConversion ---

    override fun mostrarTitulo(titulo: String) {
        vinculo.tvTitulo.text = titulo
    }

    override fun mostrarUnidades(unidades: List<Unidad>, indiceOrigen: Int, indiceDestino: Int) {
        val nombresCompletos = resources.getBoolean(R.bool.mostrar_nombres_completos_unidad)
        val etiquetas = unidades.map {
            if (nombresCompletos) it.nombre else it.simbolo
        }
        vinculo.actOrigen.setAdapter(AdaptadorSinFiltro(requireContext(), etiquetas))
        vinculo.actDestino.setAdapter(AdaptadorSinFiltro(requireContext(), etiquetas))
        // filter = false: evita que el adaptador filtre la lista con el texto actual.
        vinculo.actOrigen.setText(etiquetas[indiceOrigen], false)
        vinculo.actDestino.setText(etiquetas[indiceDestino], false)
    }

    override fun mostrarSimbolos(origen: String, destino: String) {
        vinculo.tilValor.suffixText = origen
        vinculo.tilResultado.suffixText = destino
    }

    override fun mostrarResultado(texto: String) {
        vinculo.etResultado.setText(texto)
    }

    override fun limpiarResultado() {
        vinculo.etResultado.setText("")
    }

    override fun mostrarErrorEntrada(mensaje: String?) {
        vinculo.tilValor.error = mensaje
    }

    override fun mostrarError(mensaje: String) {
        context?.let { Toast.makeText(it, mensaje, Toast.LENGTH_LONG).show() }
    }

    override fun mostrarCargando(mostrar: Boolean) {
        // Puede llegar despues de onDestroyView (la corrutina se cancela): no tocar un vinculo nulo.
        _vinculo?.btnConvertir?.isEnabled = !mostrar
    }

    override fun mostrarErrorSesionVencida() {
        val contexto = context ?: return
        if (dialogoSesion?.isShowing == true) return
        dialogoSesion = MaterialAlertDialogBuilder(contexto)
            .setTitle(R.string.sesion_expirada_titulo)
            .setMessage(R.string.sesion_expirada_mensaje)
            .setCancelable(false) // solo se sale con uno de los dos botones
            .setPositiveButton(R.string.accion_iniciar_sesion_nuevamente) { _, _ -> irAInicioSesion() }
            .setNegativeButton(R.string.accion_salir) { _, _ -> activity?.finishAffinity() }
            .show()
    }

    override fun mostrarMensajeMantenimiento() {
        context?.let {
            Toast.makeText(it, getString(R.string.masa_no_disponible), Toast.LENGTH_LONG).show()
        }
    }

    private fun irAInicioSesion() {
        // CLEAR_TASK vacia el historial: el boton "atras" no regresa al conversor.
        startActivity(
            Intent(requireContext(), ActividadInicioSesion::class.java)
                .addFlags(Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK)
        )
    }

    override fun onDestroyView() {
        dialogoSesion?.dismiss()
        dialogoSesion = null
        super.onDestroyView()
        _vinculo = null
    }

    companion object {
        private const val ARG_CATEGORIA = "categoria"

        fun newInstance(categoria: CategoriaUnidad) = FragmentoConversion().apply {
            arguments = bundleOf(ARG_CATEGORIA to categoria.name)
        }
    }
}

/** Adaptador para desplegables de seleccion fija: siempre muestra todas las opciones. */
private class AdaptadorSinFiltro(contexto: Context, elementos: List<String>) :
    ArrayAdapter<String>(contexto, android.R.layout.simple_dropdown_item_1line, elementos) {

    private val todos = elementos.toList()

    override fun getFilter(): Filter = object : Filter() {
        override fun performFiltering(constraint: CharSequence?) =
            FilterResults().apply {
                values = todos
                count = todos.size
            }

        override fun publishResults(constraint: CharSequence?, results: FilterResults?) {
            notifyDataSetChanged()
        }
    }
}
