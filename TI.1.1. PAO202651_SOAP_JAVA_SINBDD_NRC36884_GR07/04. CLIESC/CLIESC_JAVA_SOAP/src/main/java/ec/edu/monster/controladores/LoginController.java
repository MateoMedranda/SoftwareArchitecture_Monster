/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.controladores;

import ec.edu.monster.servicios.WSConversorCliente;
import ec.edu.monster.vistas.LoginVista;
import ec.edu.monster.vistas.MainVista;
import javax.swing.SwingWorker;

/**
 *
 * @author gmlop
 */
public class LoginController {
    private final LoginVista vista;
    private final WSConversorCliente client;
    
    public LoginController(LoginVista vista, WSConversorCliente cliente) {
        this.vista = vista;
        this.client = cliente;
        vista.getBtnLogin().addActionListener(e -> onLogin());
        vista.getBtnSalir().addActionListener(e -> System.exit(0));
    }
    
     private void onLogin() {
        String user = vista.getUsuario();
        String pass = vista.getPassword();

        if (user.isBlank() || pass.isBlank()) {
            vista.setMensaje("Ingresa tu usuario y contraseña para continuar.");
            return;
        }

        vista.setMensaje("Verificando tus credenciales...");
        vista.setProcesando(true);

         SwingWorker<Boolean, Void> worker = new SwingWorker<>() {
            @Override protected Boolean doInBackground() {
                return client.login(user, pass);
            }
            @Override protected void done() {
                vista.setProcesando(false);
                try {
                    if (get()) {
                        vista.dispose();
                        new MainController(new MainVista(user), client).init();
                    } else {
                        vista.setMensaje("Credenciales inválidas");
                    }
                } catch (Exception ex) {
                    vista.setMensaje("Error: " + ex.getMessage());
                }
            }
        };
        worker.execute();
    }
}
