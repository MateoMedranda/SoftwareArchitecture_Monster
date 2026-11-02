package ec.edu.monster.controladores;
import ec.edu.monster.modelos.Modelos.*;
import ec.edu.monster.servicios.ClienteSoapEureka;
import java.math.BigDecimal; import java.util.List;
/** Controlador MVC de la consola. */
public final class BancaControlador {
 private final ClienteSoapEureka servicio=new ClienteSoapEureka(); private Sesion sesion;
 public Sesion iniciar(String u,String c)throws Exception{ return sesion=servicio.login(u,c); } public Sesion sesion(){return sesion;}
 public List<Cuenta> cuentas()throws Exception{return servicio.cuentas(sesion.token());} public java.util.Map<String,String> disponibilidades()throws Exception{return servicio.disponibilidades(sesion.token());} public List<Cliente> clientesParaCuenta()throws Exception{return servicio.clientesParaCuenta(sesion.token());} public Cuenta crearCuentaParaUsuario(String usuarioId)throws Exception{return servicio.crearCuentaParaUsuario(sesion.token(),usuarioId);} public List<Movimiento> movimientos(String c)throws Exception{return servicio.movimientos(sesion.token(),c);}
 public Resultado transferir(String o,String d,BigDecimal m)throws Exception{return servicio.transferencia(sesion.token(),o,d,m);} public Resultado registrar(String t,String c,BigDecimal m)throws Exception{return servicio.movimiento(sesion.token(),t,c,m);} public Resultado reversar(String id)throws Exception{return servicio.reversar(sesion.token(),id);} public Perfil perfil()throws Exception{return servicio.perfil(sesion.token(),sesion.usuarioId());}
}
