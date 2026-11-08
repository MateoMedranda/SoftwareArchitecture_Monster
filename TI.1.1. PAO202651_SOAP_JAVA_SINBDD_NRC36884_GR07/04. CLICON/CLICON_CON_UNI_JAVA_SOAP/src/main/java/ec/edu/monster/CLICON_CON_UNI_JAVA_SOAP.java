/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 */

package ec.edu.monster;

import ec.edu.monster.controladores.AplicacionController;
import ec.edu.monster.servicios.WSConversorCliente;
import ec.edu.monster.vistas.ConsolaVista;

/**
 *
 */
public class CLICON_CON_UNI_JAVA_SOAP {

    public static void main(String[] args) {
        ConsolaVista vista = new ConsolaVista(System.in, System.out);
        try {
            WSConversorCliente cliente = new WSConversorCliente();
            new AplicacionController(vista, cliente).iniciar();
        } catch (Exception ex) {
            vista.mostrarError("No se pudo iniciar el cliente SOAP: " + ex.getMessage());
            System.exit(1);
        }
    }
}
