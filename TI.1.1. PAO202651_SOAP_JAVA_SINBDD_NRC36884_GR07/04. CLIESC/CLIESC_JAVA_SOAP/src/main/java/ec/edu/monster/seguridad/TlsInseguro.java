package ec.edu.monster.seguridad;

import java.security.SecureRandom;
import java.security.cert.X509Certificate;
import javax.net.ssl.HttpsURLConnection;
import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;

public final class TlsInseguro {

    private TlsInseguro() {
    }

    public static void habilitar() {
        try {
            TrustManager[] confianzaTotal = new TrustManager[]{
                new X509TrustManager() {
                    @Override
                    public X509Certificate[] getAcceptedIssuers() {
                        return new X509Certificate[0];
                    }

                    @Override
                    public void checkClientTrusted(X509Certificate[] cadena, String tipo) {
                    }

                    @Override
                    public void checkServerTrusted(X509Certificate[] cadena, String tipo) {
                    }
                }
            };
            SSLContext contexto = SSLContext.getInstance("TLS");
            contexto.init(null, confianzaTotal, new SecureRandom());
            HttpsURLConnection.setDefaultSSLSocketFactory(contexto.getSocketFactory());
            HttpsURLConnection.setDefaultHostnameVerifier((host, sesion) -> true);
        } catch (Exception ex) {
            throw new IllegalStateException("No fue posible habilitar TLS inseguro", ex);
        }
    }
}
