package ec.edu.monster.controladores;

import ec.edu.monster.modelos.DTO.TransaccionDTO;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import ec.edu.monster.servicios.TransaccionServicio;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import jakarta.jws.WebResult;
import jakarta.jws.WebService;
import jakarta.jws.soap.SOAPBinding;
import java.math.BigDecimal;
import java.util.List;

@Stateless
@WebService(serviceName = "TransaccionServicio", portName = "TransaccionServicioPort", targetNamespace = "http://servicios.monster.edu.ec/")
@SOAPBinding(style = SOAPBinding.Style.DOCUMENT)
public class TransaccionControlador {

    @EJB private TransaccionServicio transaccionServicio;

    @WebMethod(operationName = "generarTransaccion")
    @WebResult(name = "transaccion")
    public TransaccionDTO generarTransaccion(@WebParam(name = "token", header = true) String token, @WebParam(name = "tipo") String tipo, @WebParam(name = "cuentaId") String cuentaId, @WebParam(name = "monto") BigDecimal monto) throws TransaccionExcepcion {
        return transaccionServicio.generarTransaccion(token, tipo, cuentaId, monto);
    }

    @WebMethod(operationName = "obtenerTransaccion")
    @WebResult(name = "transaccion")
    public TransaccionDTO obtenerTransaccion(@WebParam(name = "token", header = true) String token, @WebParam(name = "transaccionId") String transaccionId) throws TransaccionExcepcion {
        return transaccionServicio.obtenerTransaccion(token, transaccionId);
    }

    @WebMethod(operationName = "listarTransacciones")
    @WebResult(name = "transacciones")
    public List<TransaccionDTO> listarTransacciones(@WebParam(name = "token", header = true) String token) throws TransaccionExcepcion {
        return transaccionServicio.listarTransacciones(token);
    }

    @WebMethod(operationName = "listarPorCuenta")
    @WebResult(name = "transacciones")
    public List<TransaccionDTO> listarPorCuenta(@WebParam(name = "token", header = true) String token, @WebParam(name = "cuentaId") String cuentaId) throws TransaccionExcepcion {
        return transaccionServicio.listarPorCuenta(token, cuentaId);
    }

    @WebMethod(operationName = "listarPorUsuario")
    @WebResult(name = "transacciones")
    public List<TransaccionDTO> listarPorUsuario(@WebParam(name = "token", header = true) String token, @WebParam(name = "usuarioId") String usuarioId) throws TransaccionExcepcion {
        return transaccionServicio.listarPorUsuario(token, usuarioId);
    }

    @WebMethod(operationName = "eliminarTransaccion")
    @WebResult(name = "exitoso")
    public boolean eliminarTransaccion(@WebParam(name = "token", header = true) String token, @WebParam(name = "transaccionId") String transaccionId) throws TransaccionExcepcion {
        return transaccionServicio.eliminarTransaccion(token, transaccionId);
    }
}
