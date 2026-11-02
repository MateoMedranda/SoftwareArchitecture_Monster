package ec.edu.monster.modelos;
import java.math.BigDecimal;
public class MovimientoVista {private final String id,tipo,fecha,responsable;private final BigDecimal monto;public MovimientoVista(String id,String tipo,BigDecimal monto,String fecha,String responsable){this.id=id;this.tipo=tipo;this.monto=monto;this.fecha=fecha;this.responsable=responsable;}public String getId(){return id;}public String getTipo(){return tipo;}public BigDecimal getMonto(){return monto;}public String getFecha(){return fecha;}public String getResponsable(){return responsable;}}
