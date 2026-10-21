package ec.edu.monster.controladores;

import ec.edu.monster.modelos.TipoConversion;
import ec.edu.monster.modelos.excepciones.ExcepcionSesionRechazada;
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
        boolean ejecutarApp = true;

        while (ejecutarApp) {
            vista.mostrarBienvenida();

            // 1. Proceso de autenticación
            while (!loginController.autenticar()) {
                vista.mostrarMensaje("Intente nuevamente.\n");
            }

            // 2. Ejecución del menú con manejo de expiración de sesión
            boolean sesionExpirada = false;
            try {
                ejecutarMenu();
            } catch (ExcepcionSesionRechazada e) {
                vista.mostrarError("\n[!] " + e.getMessage());
                sesionExpirada = true;
            } finally {
                cliente.cerrarSesion();
            }

            // 3. Evaluar si se debe reiniciar sesión o salir
            if (sesionExpirada) {
                // Si la sesión expiró por 401, preguntamos si desea reautenticarse
                ejecutarApp = preguntarReinicioSesion();
            } else {
                // El usuario decidió salir voluntariamente seleccionando 0
                vista.mostrarMensaje("Sesión finalizada. Hasta pronto.");
                ejecutarApp = false;
            }
        }
    }

    private void ejecutarMenu() {
        boolean activo = true;
        while (activo) {
            switch (vista.mostrarMenuPrincipal()) {
                case 1 -> conversionController.convertir(TipoConversion.TEMPERATURA);
                case 2 -> conversionController.convertir(TipoConversion.MASA);
                case 3 -> conversionController.convertir(TipoConversion.LONGITUD);
                case 0 -> activo = false;
                default -> vista.mostrarError("Opción no disponible.");
            }
        }
    }
    
    public boolean preguntarReinicioSesion()
    {
        boolean activo = true;
        while (activo) {
            switch (vista.menuReinicio()) {
                case 1 -> {return true;}
                case 0 -> {return false;}
                default -> vista.mostrarError("Opción no disponible.");
            }
        }
        return false;
    }
    
}
