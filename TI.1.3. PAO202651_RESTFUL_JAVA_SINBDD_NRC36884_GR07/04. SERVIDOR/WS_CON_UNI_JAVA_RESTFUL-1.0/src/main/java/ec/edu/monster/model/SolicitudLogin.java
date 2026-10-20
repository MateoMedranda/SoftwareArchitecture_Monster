package ec.edu.monster.model;

import jakarta.validation.constraints.NotBlank;
import java.io.Serializable;
import org.eclipse.microprofile.openapi.annotations.media.Schema;
import org.eclipse.microprofile.openapi.annotations.media.SchemaProperty;

@Schema(description = "Objeto que contiene credenciales de inicio de sesión")
public class SolicitudLogin implements Serializable {

    @Schema(description = "Nombre de usuario", example ="MONSTER")
    @NotBlank(message = "El usuario es requerido")
    private String usuario;

    @Schema(description = "Nombre de usuario", example ="MONSTER9")
    @NotBlank(message = "La contraseña es requerida")
    private String contrasena;

    public SolicitudLogin() {}

    public SolicitudLogin(String usuario, String contrasena) {
        this.usuario = usuario;
        this.contrasena = contrasena;
    }

    public String getUsuario() {
        return usuario;
    }

    public void setUsuario(String usuario) {
        this.usuario = usuario;
    }

    public String getContrasena() {
        return contrasena;
    }

    public void setContrasena(String contrasena) {
        this.contrasena = contrasena;
    }
}