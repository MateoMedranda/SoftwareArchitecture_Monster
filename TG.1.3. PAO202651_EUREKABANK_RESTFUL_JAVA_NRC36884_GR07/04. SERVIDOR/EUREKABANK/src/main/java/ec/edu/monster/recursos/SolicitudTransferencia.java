package ec.edu.monster.recursos;

import java.math.BigDecimal;

/**
 * Cuerpo JSON para transferencias entre cuentas.
 */
public class SolicitudTransferencia {

    private String cuentaOrigenId;
    private String cuentaDestinoId;
    private BigDecimal monto;

    public String getCuentaOrigenId() {
        return cuentaOrigenId;
    }

    public void setCuentaOrigenId(String v) {
        cuentaOrigenId = v;
    }

    public String getCuentaDestinoId() {
        return cuentaDestinoId;
    }

    public void setCuentaDestinoId(String v) {
        cuentaDestinoId = v;
    }

    public BigDecimal getMonto() {
        return monto;
    }

    public void setMonto(BigDecimal v) {
        monto = v;
    }
}
