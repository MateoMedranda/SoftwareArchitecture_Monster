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
        vista.getBtnLogin().addActionListener(e-> onLogin()); 
    }
    
     private void onLogin() {
        String user = vista.getUsuario();
        String pass = vista.getPassword();

         SwingWorker<Boolean, Void> worker = new SwingWorker<>() {
            @Override protected Boolean doInBackground() {
                return client.login(user, pass);
            }
            @Override protected void done() {
                try {
                    if (get()) {
                        vista.dispose();
                        new MainController(new MainVista(), client).init();
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
