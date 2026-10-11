/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.seguridad;

import java.time.Instant;


/**
 *
 * @author marpa
 */
public class Sesion {
    private Instant creacion;
    private Instant expiracion;
    private String token;
    
    public Sesion(String token, long ttlMinutos)
    {
        this.token = token;
        this.creacion = Instant.now();
        this.expiracion = creacion.plusSeconds(ttlMinutos*60); 
    }

    public Instant getCreacion() {
        return creacion;
    }

    public Instant getExpiracion() {
        return expiracion;
    }
    
    
}
