package ec.edu.monster.modelos.DTO;

import java.util.ArrayList;
import java.util.List;

/**
 * Vista consolidada de los datos del usuario y las cuentas a las que pertenece.
 */
public class PerfilBancarioDTO {

    private UsuarioDTO usuario;
    private List<CuentaDTO> cuentas = new ArrayList<>();

    public PerfilBancarioDTO() {
    }

    public UsuarioDTO getUsuario() {
        return usuario;
    }

    public void setUsuario(UsuarioDTO v) {
        usuario = v;
    }

    public List<CuentaDTO> getCuentas() {
        return cuentas;
    }

    public void setCuentas(List<CuentaDTO> v) {
        cuentas = v;
    }
}
