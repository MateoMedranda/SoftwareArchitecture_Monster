package ec.edu.monster.controladores;

import ec.edu.monster.modelos.DTO.CuentaDTO;
import ec.edu.monster.modelos.DTO.PerteneceDTO;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import ec.edu.monster.servicios.PerteneceServicio;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import jakarta.jws.WebResult;
import jakarta.jws.WebService;
import jakarta.jws.soap.SOAPBinding;
import java.util.List;

@Stateless
@WebService(serviceName = "PerteneceServicio", portName = "PerteneceServicioPort", targetNamespace = "http://servicios.monster.edu.ec/")
@SOAPBinding(style = SOAPBinding.Style.DOCUMENT)
public class PerteneceControlador {

    @EJB private PerteneceServicio perteneceServicio;

    @WebMethod(operationName = "asignarCuentaUsuario")
    @WebResult(name = "exitoso")
    public boolean asignarCuentaUsuario(@WebParam(name = "token", header = true) String token, @WebParam(name = "cuentaId") String cuentaId, @WebParam(name = "usuarioId") String usuarioId) throws TransaccionExcepcion {
        return perteneceServicio.asignarCuentaUsuario(token, cuentaId, usuarioId);
    }

    @WebMethod(operationName = "desasignarCuentaUsuario")
    @WebResult(name = "exitoso")
    public boolean desasignarCuentaUsuario(@WebParam(name = "token", header = true) String token, @WebParam(name = "cuentaId") String cuentaId, @WebParam(name = "usuarioId") String usuarioId) throws TransaccionExcepcion {
        return perteneceServicio.desasignarCuentaUsuario(token, cuentaId, usuarioId);
    }

    @WebMethod(operationName = "listarPorCuenta")
    @WebResult(name = "pertenece")
    public List<PerteneceDTO> listarAsignacionesPorCuenta(@WebParam(name = "token", header = true) String token, @WebParam(name = "cuentaId") String cuentaId) throws TransaccionExcepcion {
        return perteneceServicio.listarPorCuenta(token, cuentaId);
    }

    @WebMethod(operationName = "listarPorUsuario")
    @WebResult(name = "pertenece")
    public List<PerteneceDTO> listarAsignacionesPorUsuario(@WebParam(name = "token", header = true) String token, @WebParam(name = "usuarioId") String usuarioId) throws TransaccionExcepcion {
        return perteneceServicio.listarPorUsuario(token, usuarioId);
    }

    @WebMethod(operationName = "buscarCuentaPorUsuario")
    @WebResult(name = "cuentas")
    public List<CuentaDTO> buscarCuentaPorUsuario(@WebParam(name = "token", header = true) String token, @WebParam(name = "nombreOUsuario") String nombreOUsuario) throws TransaccionExcepcion {
        return perteneceServicio.buscarCuentaPorUsuario(token, nombreOUsuario);
    }
}
