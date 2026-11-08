package ec.edu.monster.view

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import ec.edu.monster.databinding.ActivityConverterBinding
import com.google.android.material.tabs.TabLayoutMediator
import ec.edu.monster.model.entities.UnitCategory
import ec.edu.monster.view.fragments.CategoryPagerAdapter

/** Pantalla de destino provisional: muestra el token recibido. Reemplázala por tu pantalla real. */
class ConverterActivity : AppCompatActivity() {
    
    private lateinit var binding: ActivityConverterBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityConverterBinding.inflate(layoutInflater)
        setContentView(binding.root)

        val categories = UnitCategory.entries
        binding.pager.adapter = CategoryPagerAdapter(this, categories)
        // Mantiene las 3 pestañas vivas para no perder lo que el usuario escribió al cambiar.
        binding.pager.offscreenPageLimit = categories.size

        TabLayoutMediator(binding.tabs, binding.pager) { tab, position ->
            tab.text = categories[position].title
        }.attach()
    }

    companion object {
        const val EXTRA_TOKEN = "extra_token"
    }
}
