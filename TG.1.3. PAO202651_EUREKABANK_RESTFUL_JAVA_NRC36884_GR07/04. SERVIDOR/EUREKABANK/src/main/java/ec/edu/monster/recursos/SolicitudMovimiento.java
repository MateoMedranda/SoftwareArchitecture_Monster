package ec.edu.monster.recursos;

import java.math.BigDecimal;

/**
 * Cuerpo JSON para registrar un ingreso o retiro.
 */
public class SolicitudMovimiento {

    private String tipo;
    private String cuentaId;
    private BigDecimal monto;

    public String getTipo() {
        return tipo;
    }

    public void setTipo(String v) {
        tipo = v;
    }

    public String getCuentaId() {
        return cuentaId;
    }

    public void setCuentaId(String v) {
        cuentaId = v;
    }

    public BigDecimal getMonto() {
        return monto;
    }

    public void setMonto(BigDecimal v) {
        monto = v;
    }
}
