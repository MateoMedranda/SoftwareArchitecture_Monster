package ec.edu.monster.controladores

import androidx.fragment.app.FragmentActivity
import androidx.viewpager2.adapter.FragmentStateAdapter
import ec.edu.monster.modelos.entidades.CategoriaUnidad

/** Crea un FragmentoConversion por cada categoria (una pestana cada una). */
class AdaptadorCategorias(
    actividad: FragmentActivity,
    private val categorias: List<CategoriaUnidad>
) : FragmentStateAdapter(actividad) {

    override fun getItemCount() = categorias.size

    override fun createFragment(position: Int) = FragmentoConversion.newInstance(categorias[position])
}
