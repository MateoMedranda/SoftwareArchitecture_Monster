package com.marpa.practica.view

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.marpa.practica.databinding.ActivityConverterBinding
import com.google.android.material.tabs.TabLayoutMediator
import com.marpa.practica.controller.ConverterController
import com.marpa.practica.controller.LoginController
import com.marpa.practica.model.AuthModel
import com.marpa.practica.model.UnitConverterModel
import com.marpa.practica.model.entities.UnitCategory
import com.marpa.practica.model.storage.SessionStore
import com.marpa.practica.view.fragments.CategoryPagerAdapter

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
