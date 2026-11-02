package ec.edu.monster.modelos;
import java.math.BigDecimal;
public class CuentaVista {private final String id,numero,estado,sucursal;private final BigDecimal saldo;public CuentaVista(String id,String numero,BigDecimal saldo,String estado,String sucursal){this.id=id;this.numero=numero;this.saldo=saldo;this.estado=estado;this.sucursal=sucursal;}public String getId(){return id;}public String getNumero(){return numero;}public BigDecimal getSaldo(){return saldo;}public String getEstado(){return estado;}public String getSucursal(){return sucursal;}}
