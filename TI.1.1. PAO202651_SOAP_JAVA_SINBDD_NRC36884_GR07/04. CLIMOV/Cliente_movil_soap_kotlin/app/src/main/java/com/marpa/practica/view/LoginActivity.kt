package com.marpa.practica.view

import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.marpa.practica.controller.LoginController
import com.marpa.practica.databinding.ActivityLoginBinding
import com.marpa.practica.model.AuthModel
import com.marpa.practica.model.storage.SessionStore

/** Vista: solo pinta la UI y reenvía eventos al controlador. */
class LoginActivity : AppCompatActivity(), LoginView {

    private lateinit var binding: ActivityLoginBinding
    private lateinit var controller: LoginController

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityLoginBinding.inflate(layoutInflater)
        setContentView(binding.root)

        controller = LoginController(
            view = this,
            model = AuthModel(),
            session = SessionStore(applicationContext),
            scope = lifecycleScope
        )

        binding.btnLogin.setOnClickListener {
            binding.tilUsername.error = null
            binding.tilPassword.error = null
            controller.onLoginClicked(
                binding.etUsername.text?.toString().orEmpty(),
                binding.etPassword.text?.toString().orEmpty()
            )
        }
    }

    override fun showLoading(show: Boolean) {
        binding.progress.visibility = if (show) View.VISIBLE else View.GONE
        binding.btnLogin.isEnabled = !show
    }

    override fun showUsernameError(message: String) {
        binding.tilUsername.error = message
    }

    override fun showPasswordError(message: String) {
        binding.tilPassword.error = message
    }

    override fun showError(message: String) {
        binding.tilUsername.error = " "
        binding.tilPassword.error = " "
        Toast.makeText(this, message, Toast.LENGTH_LONG).show()
    }

    override fun navigateToHome(token: String) {
        startActivity(
            Intent(this, ConverterActivity::class.java).putExtra(ConverterActivity.EXTRA_TOKEN, token)
        )
        finish()
    }
}
