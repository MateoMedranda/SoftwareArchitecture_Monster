package ec.edu.monster.controladores;

import ec.edu.monster.modelos.TipoConversion;
import ec.edu.monster.servicios.WSConversorCliente;
import ec.edu.monster.vistas.ConsolaVista;

/** Controlador principal del flujo de la aplicación de consola. */
public class AplicacionController {
    private final ConsolaVista vista;
    private final WSConversorCliente cliente;
    private final LoginController loginController;
    private final ConversionController conversionController;

    public AplicacionController(ConsolaVista vista, WSConversorCliente cliente) {
        this.vista = vista;
        this.cliente = cliente;
        this.loginController = new LoginController(vista, cliente);
        this.conversionController = new ConversionController(vista, cliente);
    }

    public void iniciar() {
        vista.mostrarBienvenida();
        while (!loginController.autenticar()) {
            vista.mostrarMensaje("Intente nuevamente.\n");
        }
        ejecutarMenu();
    }

    private void ejecutarMenu() {
        boolean activo = true;
        while (activo) {
            switch (vista.mostrarMenuPrincipal()) {
                case 1 -> conversionController.convertir(TipoConversion.TEMPERATURA);
                case 2 -> conversionController.convertir(TipoConversion.MASA);
                case 3 -> conversionController.convertir(TipoConversion.LONGITUD);
                case 4 -> cambiarContrasenia();
                case 0 -> activo = false;
                default -> vista.mostrarError("Opción no disponible.");
            }
        }
        cliente.cerrarSesion();
        vista.mostrarMensaje("Sesión finalizada. Hasta pronto.");
    }

    private void cambiarContrasenia() {
        vista.mostrarMensaje("\n--- Cambiar contraseña ---");
        String actual = vista.solicitarContrasenia("Contraseña actual: ");
        String nueva = vista.solicitarContrasenia("Nueva contraseña: ");
        String confirmacion = vista.solicitarContrasenia("Confirme la nueva contraseña: ");
        if (nueva.isBlank()) {
            vista.mostrarError("La nueva contraseña es obligatoria.");
            return;
        }
        if (!nueva.equals(confirmacion)) {
            vista.mostrarError("La confirmación no coincide.");
            return;
        }
        try {
            vista.mostrarMensaje(cliente.cambiarContrasenia(actual, nueva));
        } catch (RuntimeException ex) {
            String mensaje = ex.getMessage();
            vista.mostrarError(mensaje == null ? "No fue posible cambiar la contraseña." : mensaje);
        }
    }
}
