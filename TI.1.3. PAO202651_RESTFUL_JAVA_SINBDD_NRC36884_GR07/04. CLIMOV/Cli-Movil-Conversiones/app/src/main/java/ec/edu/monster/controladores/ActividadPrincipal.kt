package ec.edu.monster.controladores

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.google.android.material.tabs.TabLayoutMediator
import ec.edu.monster.databinding.ActivityPrincipalBinding
import ec.edu.monster.modelos.entidades.CategoriaUnidad

/** Pantalla principal: pestanas (Longitud, Temperatura, Masa) con el conversor. */
class ActividadPrincipal : AppCompatActivity() {

    private lateinit var vinculo: ActivityPrincipalBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        vinculo = ActivityPrincipalBinding.inflate(layoutInflater)
        setContentView(vinculo.root)

        val categorias = CategoriaUnidad.entries
        vinculo.paginador.adapter = AdaptadorCategorias(this, categorias)
        // Mantiene las 3 pestanas vivas para no perder lo que el usuario escribio al cambiar.
        vinculo.paginador.offscreenPageLimit = categorias.size

        TabLayoutMediator(vinculo.tabsConversiones, vinculo.paginador) { pestana, posicion ->
            pestana.text = categorias[posicion].titulo
        }.attach()
    }

    companion object {
        const val CLAVE_TOKEN = "clave_token"
    }
}
