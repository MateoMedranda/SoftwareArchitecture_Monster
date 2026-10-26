package ec.edu.monster.controladores;

import ec.edu.monster.modelos.DTO.CuentaDTO;
import ec.edu.monster.modelos.DTO.PerteneceDTO;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import ec.edu.monster.servicios.CuentaServicio;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import jakarta.jws.WebResult;
import jakarta.jws.WebService;
import jakarta.jws.soap.SOAPBinding;
import java.util.List;

@Stateless
@WebService(serviceName = "CuentaServicio", portName = "CuentaServicioPort", targetNamespace = "http://servicios.monster.edu.ec/")
@SOAPBinding(style = SOAPBinding.Style.DOCUMENT)
public class CuentaControlador {

    @EJB private CuentaServicio cuentaServicio;

    @WebMethod(operationName = "insertarCuenta")
    @WebResult(name = "exitoso")
    public boolean insertarCuenta(@WebParam(name = "token", header = true) String token, @WebParam(name = "cuenta") CuentaDTO cuenta) throws TransaccionExcepcion {
        return cuentaServicio.insertarCuenta(token, cuenta);
    }

    @WebMethod(operationName = "actualizarCuenta")
    @WebResult(name = "exitoso")
    public boolean actualizarCuenta(@WebParam(name = "token", header = true) String token, @WebParam(name = "cuenta") CuentaDTO cuenta) throws TransaccionExcepcion {
        return cuentaServicio.actualizarCuenta(token, cuenta);
    }

    @WebMethod(operationName = "eliminarCuenta")
    @WebResult(name = "exitoso")
    public boolean eliminarCuenta(@WebParam(name = "token", header = true) String token, @WebParam(name = "cuentaId") String cuentaId) throws TransaccionExcepcion {
        return cuentaServicio.eliminarCuenta(token, cuentaId);
    }

    @WebMethod(operationName = "obtenerCuenta")
    @WebResult(name = "cuenta")
    public CuentaDTO obtenerCuenta(@WebParam(name = "token", header = true) String token, @WebParam(name = "cuentaId") String cuentaId) throws TransaccionExcepcion {
        return cuentaServicio.obtenerCuenta(token, cuentaId);
    }

    @WebMethod(operationName = "listarCuentas")
    @WebResult(name = "return")
    public List<CuentaDTO> listarCuentas(@WebParam(name = "token", header = true) String token) throws TransaccionExcepcion {
        return cuentaServicio.listarCuentas(token);
    }

    @WebMethod(operationName = "listarCuentasPorUsuario")
    @WebResult(name = "return")
    public List<CuentaDTO> listarCuentasPorUsuario(@WebParam(name = "token", header = true) String token, @WebParam(name = "usuarioId") String usuarioId) throws TransaccionExcepcion {
        return cuentaServicio.listarCuentasPorUsuario(token, usuarioId);
    }
}
