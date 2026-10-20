package ec.edu.monster.seguridad;

import java.security.SecureRandom;
import java.security.cert.X509Certificate;
import javax.net.ssl.SSLContext;
import javax.net.ssl.SSLParameters;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;

/** Permite confiar en el certificado autofirmado de desarrollo cuando se habilita en el .env. */
public final class TlsInseguro {
    private TlsInseguro() { }

    /** Contexto TLS que acepta cualquier certificado; lo usa el HttpClient del cliente RESTful. */
    public static SSLContext contextoInseguro() {
        try {
            TrustManager[] confianzaTotal = {new X509TrustManager() {
                @Override public X509Certificate[] getAcceptedIssuers() { return new X509Certificate[0]; }
                @Override public void checkClientTrusted(X509Certificate[] cadena, String tipo) { }
                @Override public void checkServerTrusted(X509Certificate[] cadena, String tipo) { }
            }};
            SSLContext contexto = SSLContext.getInstance("TLS");
            contexto.init(null, confianzaTotal, new SecureRandom());
            return contexto;
        } catch (Exception ex) {
            throw new IllegalStateException("No fue posible habilitar TLS inseguro", ex);
        }
    }

    /** Parámetros que omiten la verificación del nombre de host del certificado autofirmado. */
    public static SSLParameters parametrosSinVerificacionDeHost() {
        SSLParameters parametros = new SSLParameters();
        parametros.setEndpointIdentificationAlgorithm(null);
        return parametros;
    }
}
