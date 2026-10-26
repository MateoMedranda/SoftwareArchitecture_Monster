package ec.edu.monster.controladores;

import ec.edu.monster.modelos.DTO.UsuarioDTO;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import ec.edu.monster.servicios.UsuarioServicio;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import jakarta.jws.WebResult;
import jakarta.jws.WebService;
import jakarta.jws.soap.SOAPBinding;
import java.util.List;

@Stateless
@WebService(serviceName = "UsuarioServicio", portName = "UsuarioServicioPort", targetNamespace = "http://servicios.monster.edu.ec/")
@SOAPBinding(style = SOAPBinding.Style.DOCUMENT)
public class UsuarioControlador {

    @EJB private UsuarioServicio usuarioServicio;

    @WebMethod(operationName = "insertarUsuario")
    @WebResult(name = "exitoso")
    public boolean insertarUsuario(@WebParam(name = "token", header = true) String token, @WebParam(name = "usuario") UsuarioDTO usuario) throws TransaccionExcepcion {
        return usuarioServicio.insertarUsuario(token, usuario);
    }

    @WebMethod(operationName = "actualizarUsuario")
    @WebResult(name = "exitoso")
    public boolean actualizarUsuario(@WebParam(name = "token", header = true) String token, @WebParam(name = "usuario") UsuarioDTO usuario) throws TransaccionExcepcion {
        return usuarioServicio.actualizarUsuario(token, usuario);
    }

    @WebMethod(operationName = "eliminarUsuario")
    @WebResult(name = "exitoso")
    public boolean eliminarUsuario(@WebParam(name = "token", header = true) String token, @WebParam(name = "usuarioId") String usuarioId) throws TransaccionExcepcion {
        return usuarioServicio.eliminarUsuario(token, usuarioId);
    }

    @WebMethod(operationName = "obtenerUsuario")
    @WebResult(name = "usuario")
    public UsuarioDTO obtenerUsuario(@WebParam(name = "token", header = true) String token, @WebParam(name = "usuarioId") String usuarioId) throws TransaccionExcepcion {
        return usuarioServicio.obtenerUsuario(token, usuarioId);
    }

    @WebMethod(operationName = "listarUsuarios")
    @WebResult(name = "usuarios")
    public List<UsuarioDTO> listarUsuarios(@WebParam(name = "token", header = true) String token) throws TransaccionExcepcion {
        return usuarioServicio.listarUsuarios(token);
    }

    @WebMethod(operationName = "buscarUsuarioPorNombre")
    @WebResult(name = "usuarios")
    public List<UsuarioDTO> buscarUsuarioPorNombre(@WebParam(name = "token", header = true) String token, @WebParam(name = "nombre") String nombre) throws TransaccionExcepcion {
        return usuarioServicio.buscarUsuarioPorNombre(token, nombre);
    }
}
