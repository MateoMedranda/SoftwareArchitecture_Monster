package ec.edu.monster.controladores;

import ec.edu.monster.modelos.Respuestas.LoginExcepcion;
import ec.edu.monster.modelos.Respuestas.RespuestaLogin;
import ec.edu.monster.servicios.LoginServicio;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import jakarta.jws.WebResult;
import jakarta.jws.WebService;
import jakarta.jws.soap.SOAPBinding;

/**
 * Controlador SOAP de autenticación de EUREKABANK.
 *
 * Expone tres operaciones:
 *
 *   1. login(usuario, contrasenia)
 *      → Autentica al usuario contra la BDD (SHA-256).
 *        Genera un JWT y lo registra en la tabla TOKEN.
 *        Retorna RespuestaLogin con el token.
 *
 *   2. logout(token)
 *      → Revoca el token (JTI → INACTIVO en tabla TOKEN).
 *        Operación protegida: requiere token válido.
 *
 *   3. verificarToken(token)
 *      → Valida que el token sea correcto y esté activo.
 *        Retorna los datos del usuario si es válido.
 *
 * URL del WSDL (Payara):
 *   https://localhost:8181/EUREKABANK/LoginServicio?wsdl
 */
@Stateless
@WebService(
    serviceName     = "LoginServicio",
    portName        = "LoginServicioPort",
    targetNamespace = "http://servicios.monster.edu.ec/"
)
@SOAPBinding(style = SOAPBinding.Style.DOCUMENT)
public class LoginControlador {

    @EJB private LoginServicio loginServicio;

    @WebMethod(operationName = "login")
    @WebResult(name = "respuestaLogin")
    public RespuestaLogin login(
            @WebParam(name = "usuario")     String usuario,
            @WebParam(name = "contrasenia") String contrasenia
    ) throws LoginExcepcion {
        return loginServicio.login(usuario, contrasenia);
    }

    @WebMethod(operationName = "logout")
    @WebResult(name = "exitoso")
    public boolean logout(
            @WebParam(name = "token") String token
    ) throws LoginExcepcion {
        return loginServicio.logout(token);
    }

    @WebMethod(operationName = "verificarToken")
    @WebResult(name = "respuestaToken")
    public RespuestaLogin verificarToken(
            @WebParam(name = "token") String token
    ) throws LoginExcepcion {
        return loginServicio.verificarToken(token);
    }
}
