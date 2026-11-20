package ec.edu.monster.controladores;

import ec.edu.monster.modelos.Modelos.*;
import ec.edu.monster.servicios.ClienteSoapEureka;
import java.math.BigDecimal;
import java.util.List;

/** Controlador MVC móvil: conserva sesión y orquesta los casos de uso. */
public final class BancaControlador {
 private final ClienteSoapEureka soap=new ClienteSoapEureka();
 private Sesion sesion;
 public Sesion login(String u,String p)throws Exception{return sesion=soap.login(u,p);}
 public Sesion sesion(){return sesion;}
 public List<Cuenta> cuentas()throws Exception{return soap.cuentas(sesion.token);}
 public java.util.Map<String,String> disponibilidades()throws Exception{return soap.disponibilidades(sesion.token);}
 public List<Cliente> clientesParaCuenta()throws Exception{return soap.clientesParaCuenta(sesion.token);}
 public Cuenta crearCuentaParaUsuario(String usuarioId)throws Exception{return soap.crearCuentaParaUsuario(sesion.token,usuarioId);}
 public Perfil perfil()throws Exception{return soap.perfil(sesion.token,sesion.usuarioId);}
 public List<Movimiento> movimientos(String id)throws Exception{return soap.movimientos(sesion.token,id);}
 public void transferir(String o,String d,BigDecimal m)throws Exception{soap.transferir(sesion.token,o,d,m);}
 public void movimiento(String tipo,String cuenta,BigDecimal monto)throws Exception{soap.movimiento(sesion.token,tipo,cuenta,monto);}
 public void reversar(String transaccionId)throws Exception{soap.reversar(sesion.token,transaccionId);}
 public void salir(){sesion=null;}
}
