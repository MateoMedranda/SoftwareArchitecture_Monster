/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 */

package ec.edu.monster;

import ec.edu.monster.controladores.LoginController;
import ec.edu.monster.servicios.WSConversorCliente;
import ec.edu.monster.vistas.LoginVista;
import javax.swing.JOptionPane;
import javax.swing.SwingUtilities;

/**
 *
 * @author gmlop
 */
public class CLIESC_JAVA_SOAP {

    public static void main(String[] args) {
        SwingUtilities.invokeLater(() -> {
            try {
                WSConversorCliente client = new WSConversorCliente();
                LoginVista login = new LoginVista();
                new LoginController(login, client);
                login.setVisible(true);
            } catch (Exception e) {
                JOptionPane.showMessageDialog(null,
                    "No se pudo conectar al servidor SOAP:\n" + e.getMessage(),
                    "Error de conexión", JOptionPane.ERROR_MESSAGE);
                System.exit(1);
            }
        });
    }
}
