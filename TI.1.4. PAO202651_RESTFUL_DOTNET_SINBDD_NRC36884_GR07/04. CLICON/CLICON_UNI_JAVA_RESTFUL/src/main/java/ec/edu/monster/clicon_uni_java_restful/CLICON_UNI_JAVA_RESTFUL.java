/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 */

package ec.edu.monster.clicon_uni_java_restful;

import ec.edu.monster.controladores.AplicacionController;
import ec.edu.monster.servicios.WSConversorCliente;
import ec.edu.monster.vistas.ConsolaVista;

/**
 *
 * @author marpa
 */
public class CLICON_UNI_JAVA_RESTFUL {

    public static void main(String[] args) {
        ConsolaVista vista = new ConsolaVista(System.in, System.out);
        try {
            WSConversorCliente cliente = new WSConversorCliente();
            new AplicacionController(vista, cliente).iniciar();
        } catch (Exception ex) {
            vista.mostrarError("No se pudo iniciar el cliente REST: " + ex.getMessage());
            System.exit(1);
        }
    }
}
