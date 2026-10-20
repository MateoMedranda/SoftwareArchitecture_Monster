package ec.edu.monster.controladores;

import ec.edu.monster.servicios.ExcepcionServicio;
import ec.edu.monster.servicios.WSConversorCliente;
import ec.edu.monster.vistas.ConsolaVista;

/** Controla la autenticación y conserva el token en el cliente de servicios. */
public class LoginController {
    private final ConsolaVista vista;
    private final WSConversorCliente cliente;

    public LoginController(ConsolaVista vista, WSConversorCliente cliente) {
        this.vista = vista;
        this.cliente = cliente;
    }

    public boolean autenticar() {
        vista.mostrarMensaje("\n--- Inicio de sesión ---");
        String usuario = vista.solicitarTexto("Usuario: ");
        String contrasenia = vista.solicitarContrasenia("Contraseña: ");
        if (usuario.isBlank() || contrasenia.isBlank()) {
            vista.mostrarError("El usuario y la contraseña son obligatorios.");
            return false;
        }
        try {
            if (!cliente.login(usuario, contrasenia)) {
                vista.mostrarError("Credenciales inválidas. Verifique su usuario y contraseña.");
                return false;
            }
        } catch (ExcepcionServicio ex) {
            vista.mostrarError("No se pudo iniciar sesión: " + ex.getMessage());
            return false;
        }
        vista.mostrarMensaje("Sesión iniciada correctamente.");
        return true;
    }
}
