/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.controladores;

import ec.edu.monster.servicios.WSConversorCliente;
import ec.edu.monster.vistas.MainVista;

/**
 *
 * @author gmlop
 */
public class MainController {
    private final MainVista vista;
    private final WSConversorCliente client;
    
    public MainController(MainVista vista, WSConversorCliente cliente) {
        this.vista = vista;
        this.client = cliente;
    }
    
    public void init() {
        

        // --- Conversión de temperatura ---

        // --- Conversión de longitud ---

        // --- Conversión de masa ---

        // --- Cambio de contraseña ---
        vista.setLocationRelativeTo(null);
        vista.setVisible(true);
    }
}
