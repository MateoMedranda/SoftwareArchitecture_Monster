/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.controladores;

import ec.edu.monster.servicios.ClienteSOAPFactory;
import ec.edu.monster.util.SSLUtil;

/**
 *
 * @author gmlop
 */
public class ControladorPrueba {
    public static void main(String[] args) {
        try {
            SSLUtil.installGlobally();
            var puerto = ClienteSOAPFactory.crearPuerto(null);

            String resultado = puerto.login("MONSTER", "MONSTER9");
            System.out.println(resultado);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
