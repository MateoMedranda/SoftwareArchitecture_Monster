package ec.edu.monster.util;

import jakarta.servlet.ServletContextEvent;
import jakarta.servlet.ServletContextListener;
import jakarta.servlet.annotation.WebListener;

/**
 * Inicializa la configuracion SSL por identidad de certificado al arrancar la
 * aplicacion web (JSF + PrimeFaces sobre Payara), de modo que los managed beans
 * puedan invocar el servicio SOAP por HTTPS sin exportar el certificado.
 *
 * Los dominios extra pueden indicarse con el context-param "ssl.trustedHosts"
 * (separados por coma) o con la system property monster.ssl.trustedHosts.
 */
@WebListener
public class SSLConfigListener implements ServletContextListener {

    @Override
    public void contextInitialized(ServletContextEvent sce) {
        String extra = sce.getServletContext().getInitParameter("ssl.trustedHosts");
        if (extra != null && !extra.isBlank()) {
            String current = System.getProperty(SSLUtil.TRUSTED_HOSTS_PROPERTY, "");
            String merged = current.isBlank() ? extra.trim() : current + "," + extra.trim();
            System.setProperty(SSLUtil.TRUSTED_HOSTS_PROPERTY, merged);
        }
        SSLUtil.installGlobally();
    }

    @Override
    public void contextDestroyed(ServletContextEvent sce) {
        // Sin recursos que liberar.
    }
}
