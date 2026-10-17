package ec.edu.monster.view.fragments

import androidx.fragment.app.FragmentActivity
import androidx.viewpager2.adapter.FragmentStateAdapter
import ec.edu.monster.model.entities.UnitCategory

/** Crea un ConverterFragment por cada categoría (una pestaña cada una). */
class CategoryPagerAdapter(
    activity: FragmentActivity,
    private val categories: List<UnitCategory>
) : FragmentStateAdapter(activity) {

    override fun getItemCount() = categories.size

    override fun createFragment(position: Int) = ConverterFragment.newInstance(categories[position])
}
