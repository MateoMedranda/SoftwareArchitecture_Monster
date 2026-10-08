/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/WebServices/WebService.java to edit this template
 */
package ec.edu.monster.ws;

import ec.edu.monster.modelos.utilidades.ConversorLongitud;
import ec.edu.monster.modelos.utilidades.ConversorMasa;
import ec.edu.monster.modelos.utilidades.ConversorTemperatura;
import ec.edu.monster.modelos.utilidades.enums.UnidadLongitud;
import ec.edu.monster.modelos.utilidades.enums.UnidadMasa;
import ec.edu.monster.modelos.utilidades.enums.UnidadTemperatura;
import ec.edu.monster.modelos.utilidades.mapeadores.UnidadMapper;
import ec.edu.monster.seguridad.AdministradorCredenciales;
import ec.edu.monster.seguridad.AdministradorTokens;
import ec.edu.monster.servicios.ServicioConversor;
import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;

/**
 *
 * @author USER
 */
@WebService(serviceName = "WSConversorUnidades")
public class WSConversorUnidades {

    private final ServicioConversor<UnidadLongitud> servicioLongitud =
            new ServicioConversor<>(new ConversorLongitud());

    private final ServicioConversor<UnidadMasa> servicioMasa =
            new ServicioConversor<>(new ConversorMasa());

    private final ServicioConversor<UnidadTemperatura> servicioTemperatura =
            new ServicioConversor<>(new ConversorTemperatura());

    @WebMethod(operationName = "login")
    public String login(
            @WebParam(name = "usuario") String usuario,
            @WebParam(name = "contrasenia") String contrasenia) {
        
        if (AdministradorCredenciales.validarCredenciales(usuario, contrasenia)) {
            return AdministradorTokens.generarToken();
        }
        throw new RuntimeException("Credenciales incorrectas");
    }

    @WebMethod(operationName = "cambiarContrasenia")
    public String cambiarContrasenia(
            @WebParam(name = "contraseniaActual") String contraseniaActual,
            @WebParam(name = "contraseniaNueva") String contraseniaNueva) {
        AdministradorCredenciales.cambiarContrasenia(contraseniaActual, contraseniaNueva);
        return "Contrasenia actualizada correctamente";
    }
   
    @WebMethod(operationName = "convertirLongitud")
    public double convertirLongitud(
            @WebParam(name = "valor") double valor,
            @WebParam(name = "unidadInicial") String unidadInicial,
            @WebParam(name = "unidadFinal") String unidadFinal) {

        UnidadLongitud origen = UnidadMapper.toLongitud(unidadInicial);
        UnidadLongitud destino = UnidadMapper.toLongitud(unidadFinal);

        return servicioLongitud.convertir(valor, origen, destino);
    }
    
    @WebMethod(operationName = "convertirMasa")
    public double convertirMasa(
            @WebParam(name = "valor") double valor,
            @WebParam(name = "unidadInicial") String unidadInicial,
            @WebParam(name = "unidadFinal") String unidadFinal) {

        UnidadMasa origen = UnidadMapper.toMasa(unidadInicial);
        UnidadMasa destino = UnidadMapper.toMasa(unidadFinal);

        return servicioMasa.convertir(valor, origen, destino);
    }

    @WebMethod(operationName = "convertirTemperatura")
    public double convertirTemperatura(
            @WebParam(name = "valor") double valor,
            @WebParam(name = "unidadInicial") String unidadInicial,
            @WebParam(name = "unidadFinal") String unidadFinal) {

        UnidadTemperatura origen = UnidadMapper.toTemperatura(unidadInicial);
        UnidadTemperatura destino = UnidadMapper.toTemperatura(unidadFinal);

        return servicioTemperatura.convertir(valor, origen, destino);
    }
}
