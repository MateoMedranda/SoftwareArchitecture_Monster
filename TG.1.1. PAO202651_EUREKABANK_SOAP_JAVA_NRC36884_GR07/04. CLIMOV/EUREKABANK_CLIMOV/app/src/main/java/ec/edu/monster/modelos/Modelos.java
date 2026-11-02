package ec.edu.monster.modelos;

import java.math.BigDecimal;
import java.util.List;

public final class Modelos {
 private Modelos(){}
 public static final class Sesion { public final String token, usuarioId, nombre; public Sesion(String t,String i,String n){token=t;usuarioId=i;nombre=n;} }
 public static final class Cuenta { public final String id,numero,estado,sucursal; public final BigDecimal saldo; public String disponibilidad="DISPONIBLE"; public Cuenta(String i,String n,BigDecimal s,String e,String su){id=i;numero=n;saldo=s;estado=e;sucursal=su;} }
 public static final class Cliente { public final String id,nombre,usuario; public Cliente(String i,String n,String u){id=i;nombre=n;usuario=u;} @Override public String toString(){return nombre+" · @"+usuario;} }
 public static final class Movimiento { public final String id,tipo,fecha; public final BigDecimal monto; public Movimiento(String i,String t,BigDecimal m,String f){id=i;tipo=t;monto=m;fecha=f;} }
 public static final class Perfil { public final String nombre,usuario,telefono; public final List<Cuenta> cuentas; public Perfil(String n,String u,String t,List<Cuenta> c){nombre=n;usuario=u;telefono=t;cuentas=c;} }
}
