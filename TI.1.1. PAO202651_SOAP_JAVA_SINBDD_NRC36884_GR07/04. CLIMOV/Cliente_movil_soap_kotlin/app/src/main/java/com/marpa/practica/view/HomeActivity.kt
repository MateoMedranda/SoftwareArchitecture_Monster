package com.marpa.practica.view

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.marpa.practica.databinding.ActivityHomeBinding

/** Pantalla de destino provisional: muestra el token recibido. Reemplázala por tu pantalla real. */
class HomeActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        val binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)
        binding.tvToken.text = intent.getStringExtra(EXTRA_TOKEN).orEmpty()
    }

    companion object {
        const val EXTRA_TOKEN = "extra_token"
    }
}
