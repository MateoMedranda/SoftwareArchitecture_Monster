package ec.edu.monster.service;

import ec.edu.monster.exception.ExcepcionAutenticacion;
import ec.edu.monster.model.RespuestaLogin;
import ec.edu.monster.model.SolicitudLogin;
import ec.edu.monster.security.ProveedorTokenJwt;
import jakarta.enterprise.context.ApplicationScoped;
import jakarta.inject.Inject;

@ApplicationScoped
public class ServicioAutenticacion {

    private static final String USUARIO_VALIDO = "MONSTER";
    private static final String CONTRASENA_VALIDA = "MONSTER9";

    @Inject
    private ProveedorTokenJwt proveedorTokenJwt;

    public ServicioAutenticacion() {}

    public RespuestaLogin iniciarSesion(SolicitudLogin solicitud) {
        if (!USUARIO_VALIDO.equals(solicitud.getUsuario()) || !CONTRASENA_VALIDA.equals(solicitud.getContrasena())) {
            throw new ExcepcionAutenticacion("Credenciales inválidas");
        }

        String token = proveedorTokenJwt.generarToken(solicitud.getUsuario());
        long expiraEn = proveedorTokenJwt.obtenerTiempoExpiracion();

        return new RespuestaLogin(token, expiraEn);
    }

    public void validarToken(String token) {
        if (!proveedorTokenJwt.validarToken(token)) {
            throw new ExcepcionAutenticacion("Token inválido o expirado");
        }
    }
}