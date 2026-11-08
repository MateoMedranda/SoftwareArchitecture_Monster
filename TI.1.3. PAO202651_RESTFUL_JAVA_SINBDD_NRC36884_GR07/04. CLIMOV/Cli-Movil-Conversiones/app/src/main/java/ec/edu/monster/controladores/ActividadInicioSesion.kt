package ec.edu.monster.controladores

import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import ec.edu.monster.databinding.ActivityInicioSesionBinding
import ec.edu.monster.modelos.autenticacion.ModeloAutenticacion
import ec.edu.monster.modelos.datos.GestorSesion
import ec.edu.monster.red.ClienteRest
import ec.edu.monster.vistas.VistaInicioSesion

/** Pantalla de inicio de sesion: implementa el contrato de vista y delega en el controlador. */
class ActividadInicioSesion : AppCompatActivity(), VistaInicioSesion {

    private lateinit var vinculo: ActivityInicioSesionBinding
    private lateinit var controlador: ControladorInicioSesion

    /** Evita ocultar el overlay si ya se navego al conversor. */
    private var navegando = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        vinculo = ActivityInicioSesionBinding.inflate(layoutInflater)
        setContentView(vinculo.root)

        // Overlay visible hasta que la pantalla termine de dibujarse.
        mostrarCargando(true)

        val sesion = GestorSesion(applicationContext)
        controlador = ControladorInicioSesion(
            vista = this,
            modelo = ModeloAutenticacion(ClienteRest(sesion)),
            sesion = sesion,
            ambito = lifecycleScope
        )

        vinculo.btnIniciarSesion.setOnClickListener {
            vinculo.tilUsuario.error = null
            vinculo.tilContrasena.error = null
            controlador.enClicIniciarSesion(
                vinculo.etUsuario.text?.toString().orEmpty(),
                vinculo.etContrasena.text?.toString().orEmpty()
            )
        }

        vinculo.root.post { mostrarCargando(false) }
    }

    // --- VistaInicioSesion ---

    override fun mostrarCargando(mostrar: Boolean) {
        // Mientras se navega al conversor el overlay permanece visible.
        if (!mostrar && navegando) return
        vinculo.capaCarga.visibility = if (mostrar) View.VISIBLE else View.GONE
        vinculo.btnIniciarSesion.isEnabled = !mostrar
    }

    override fun mostrarErrorUsuario(mensaje: String) {
        vinculo.tilUsuario.error = mensaje
    }

    override fun mostrarErrorContrasena(mensaje: String) {
        vinculo.tilContrasena.error = mensaje
    }

    override fun mostrarError(mensaje: String) {
        vinculo.tilUsuario.error = null
        vinculo.tilContrasena.error = null
        Toast.makeText(this, mensaje, Toast.LENGTH_LONG).show()
    }

    override fun navegarAlConversor(token: String) {
        navegando = true
        startActivity(
            Intent(this, ActividadPrincipal::class.java)
                .putExtra(ActividadPrincipal.CLAVE_TOKEN, token)
        )
        finish()
    }
}
