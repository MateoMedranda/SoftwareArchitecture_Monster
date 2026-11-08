package ec.edu.monster.util;

import java.io.InputStream;
import java.net.URL;
import java.security.SecureRandom;
import java.security.cert.X509Certificate;
import java.util.Collections;
import java.util.LinkedHashSet;
import java.util.Set;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import javax.naming.ldap.LdapName;
import javax.naming.ldap.Rdn;
import javax.net.ssl.HostnameVerifier;
import javax.net.ssl.HttpsURLConnection;
import javax.net.ssl.SSLContext;
import javax.net.ssl.SSLSocketFactory;
import javax.net.ssl.TrustManager;
import javax.net.ssl.TrustManagerFactory;
import javax.net.ssl.X509TrustManager;

/**
 * Configuracion SSL para pruebas: mantiene TLS pero confia en certificados
 * (autofirmados o internos) solo cuando su identidad (CN/SAN) pertenece al
 * dominio permitido. El dominio se deriva del soap:address del WSDL y se puede
 * ampliar con la system property "monster.ssl.trustedHosts" o el context-param
 * "ssl.trustedHosts".
 *
 * El pinning es por identidad de certificado, no por host solicitado, de modo
 * que funciona igual si el servidor se alcanza por localhost, IP interna o IP
 * externa mientras el certificado mantenga su dominio. Los puertos son
 * irrelevantes.
 */
public final class SSLUtil {

    private static final Logger LOG = Logger.getLogger(SSLUtil.class.getName());

    public static final String WSDL_RESOURCE = "/WSConversorUnidades.wsdl";
    public static final String TRUSTED_HOSTS_PROPERTY = "monster.ssl.trustedHosts";

    private static final Pattern SOAP_ADDRESS =
            Pattern.compile("soap:address\\s+location=\"([^\"]+)\"");

    private static volatile boolean installed = false;

    private SSLUtil() {
    }

    /**
     * Instala globalmente un SSLContext y un HostnameVerifier que confian por
     * identidad de certificado. Idempotente.
     */
    public static synchronized void installGlobally() {
        if (installed) {
            return;
        }
        try {
            final Set<String> trusted = getTrustedIdentities();

            final X509TrustManager defaultTm = getDefaultTrustManager();
            X509TrustManager pinningTm = new X509TrustManager() {
                @Override
                public void checkClientTrusted(X509Certificate[] chain, String authType)
                        throws java.security.cert.CertificateException {
                    defaultTm.checkClientTrusted(chain, authType);
                }

                @Override
                public void checkServerTrusted(X509Certificate[] chain, String authType)
                        throws java.security.cert.CertificateException {
                    try {
                        defaultTm.checkServerTrusted(chain, authType);
                        return;
                    } catch (java.security.cert.CertificateException ignored) {
                        // Certificado no emitido por una CA conocida: aplicar pinning.
                    }
                    if (chain == null || chain.length == 0) {
                        throw new java.security.cert.CertificateException("Cadena de certificados vacia");
                    }
                    X509Certificate leaf = chain[0];
                    String matched = matchIdentity(leaf, trusted);
                    if (matched == null) {
                        throw new java.security.cert.CertificateException(
                                "Certificado no confiable: identidad " + describeIdentity(leaf)
                                + " no pertenece al allowlist " + trusted);
                    }
                    LOG.log(Level.INFO, "Certificado aceptado por pinning (identidad {0})", matched);
                }

                @Override
                public X509Certificate[] getAcceptedIssuers() {
                    return defaultTm.getAcceptedIssuers();
                }
            };

            SSLContext context = SSLContext.getInstance("TLS");
            context.init(null, new TrustManager[]{pinningTm}, new SecureRandom());

            HostnameVerifier verifier = (host, session) -> {
                try {
                    java.security.cert.Certificate[] peer = session.getPeerCertificates();
                    if (peer != null && peer.length > 0 && peer[0] instanceof X509Certificate) {
                        if (matchIdentity((X509Certificate) peer[0], trusted) != null) {
                            return true;
                        }
                    }
                } catch (Exception e) {
                    LOG.log(Level.FINE, "No se pudo evaluar la identidad del certificado", e);
                }
                return HttpsURLConnection.getDefaultHostnameVerifier().verify(host, session);
            };

            SSLContext.setDefault(context);
            SSLSocketFactory factory = context.getSocketFactory();
            HttpsURLConnection.setDefaultSSLSocketFactory(factory);
            HttpsURLConnection.setDefaultHostnameVerifier(verifier);

            installed = true;
            LOG.log(Level.INFO, "SSL configurado (TLS + pinning por identidad). Dominios permitidos: {0}",
                    trusted);
        } catch (Exception e) {
            throw new IllegalStateException("No se pudo configurar el contexto SSL", e);
        }
    }

    /**
     * Obtiene las identidades confiables: host del soap:address del WSDL mas
     * los dominios extra configurados.
     */
    public static Set<String> getTrustedIdentities() {
        Set<String> hosts = new LinkedHashSet<>();

        String wsdlHost = readHostFromWsdl();
        if (wsdlHost != null && !wsdlHost.isBlank()) {
            hosts.add(wsdlHost.toLowerCase());
        }

        String extra = System.getProperty(TRUSTED_HOSTS_PROPERTY, "");
        for (String h : extra.split(",")) {
            String v = h.trim().toLowerCase();
            if (!v.isEmpty()) {
                hosts.add(v);
            }
        }

        if (hosts.isEmpty()) {
            hosts.add("localhost");
        }
        return Collections.unmodifiableSet(hosts);
    }

    private static String readHostFromWsdl() {
        try (InputStream in = SSLUtil.class.getResourceAsStream(WSDL_RESOURCE)) {
            if (in == null) {
                LOG.log(Level.WARNING, "No se encontro {0} en el classpath", WSDL_RESOURCE);
                return null;
            }
            String content = new String(in.readAllBytes(), java.nio.charset.StandardCharsets.UTF_8);
            Matcher m = SOAP_ADDRESS.matcher(content);
            if (m.find()) {
                URL url = new URL(m.group(1));
                return url.getHost();
            }
            LOG.log(Level.WARNING, "No se encontro soap:address en el WSDL");
        } catch (Exception e) {
            LOG.log(Level.WARNING, "Error leyendo el WSDL para derivar el dominio", e);
        }
        return null;
    }

    private static X509TrustManager getDefaultTrustManager() throws Exception {
        TrustManagerFactory tmf =
                TrustManagerFactory.getInstance(TrustManagerFactory.getDefaultAlgorithm());
        tmf.init((java.security.KeyStore) null);
        for (TrustManager tm : tmf.getTrustManagers()) {
            if (tm instanceof X509TrustManager) {
                return (X509TrustManager) tm;
            }
        }
        throw new IllegalStateException("No se obtuvo un X509TrustManager por defecto");
    }

    /**
     * Devuelve la identidad del allowlist que coincide con el certificado, o
     * null si ninguna coincide. Considera CN y SAN (DNS/IP) con soporte basico
     * de comodin "*.dominio".
     */
    private static String matchIdentity(X509Certificate cert, Set<String> trusted) {
        Set<String> identities = new LinkedHashSet<>();
        String cn = extractCn(cert);
        if (cn != null) {
            identities.add(cn.toLowerCase());
        }
        identities.addAll(extractSan(cert));

        for (String id : identities) {
            if (matchesAny(id, trusted)) {
                return id;
            }
        }
        return null;
    }

    private static boolean matchesAny(String identity, Set<String> trusted) {
        for (String allowed : trusted) {
            if (identity.equals(allowed)) {
                return true;
            }
            if (allowed.startsWith("*.")) {
                String suffix = allowed.substring(1);
                if (identity.endsWith(suffix) && identity.indexOf('.') == identity.length() - suffix.length() - 1) {
                    return true;
                }
            }
        }
        return false;
    }

    private static String extractCn(X509Certificate cert) {
        try {
            LdapName name = new LdapName(cert.getSubjectX500Principal().getName());
            for (Rdn rdn : name.getRdns()) {
                if ("CN".equalsIgnoreCase(rdn.getType())) {
                    return String.valueOf(rdn.getValue());
                }
            }
        } catch (Exception e) {
            LOG.log(Level.FINE, "No se pudo extraer el CN", e);
        }
        return null;
    }

    private static Set<String> extractSan(X509Certificate cert) {
        Set<String> sans = new LinkedHashSet<>();
        try {
            java.util.Collection<java.util.List<?>> entries = cert.getSubjectAlternativeNames();
            if (entries == null) {
                return sans;
            }
            for (java.util.List<?> entry : entries) {
                if (entry == null || entry.size() < 2) {
                    continue;
                }
                Integer type = (Integer) entry.get(0);
                // 2 = dNSName, 7 = iPAddress
                if (type == 2 || type == 7) {
                    Object value = entry.get(1);
                    if (value != null) {
                        sans.add(String.valueOf(value).toLowerCase());
                    }
                }
            }
        } catch (Exception e) {
            LOG.log(Level.FINE, "No se pudieron extraer los SAN", e);
        }
        return sans;
    }

    private static String describeIdentity(X509Certificate cert) {
        String cn = extractCn(cert);
        Set<String> san = extractSan(cert);
        return "CN=" + cn + (san.isEmpty() ? "" : " SAN=" + san);
    }
}
