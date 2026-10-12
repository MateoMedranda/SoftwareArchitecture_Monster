/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.controladores.config;

import io.github.cdimascio.dotenv.Dotenv;
import java.io.File;
import java.net.URL;
import java.nio.file.Path;
import java.nio.file.Paths;

public final class SoapConfig {

    private static final String ARCHIVO_ENV = ".development.env";

    private static final Dotenv DOTENV = cargar();

    private SoapConfig() {
    }

    private static Dotenv cargar() {
        return Dotenv.configure()
                .directory(ubicarDirectorio())
                .filename(ARCHIVO_ENV)
                .ignoreIfMissing()
                .load();
    }

    private static String ubicarDirectorio() {
        if (new File(ARCHIVO_ENV).isFile()) {
            return ".";
        }
        URL recurso = SoapConfig.class.getResource("/" + ARCHIVO_ENV);
        if (recurso != null && "file".equalsIgnoreCase(recurso.getProtocol())) {
            try {
                Path ruta = Paths.get(recurso.toURI());
                return ruta.getParent().toString();
            } catch (Exception ignorado) {
            }
        }
        Path desarrollo = Paths.get("src", "main", "recursos", ARCHIVO_ENV);
        if (desarrollo.toFile().isFile()) {
            return desarrollo.getParent().toString();
        }
        return ".";
    }

    private static String requerido(String clave) {
        String valor = DOTENV.get(clave);
        if (valor == null || valor.isBlank()) {
            throw new IllegalStateException(
                    "Falta la variable '" + clave + "' en " + ARCHIVO_ENV);
        }
        return valor;
    }

    public static String endpoint() {
        return requerido("SOAP_ENDPOINT");
    }

    public static String wsdl() {
        return DOTENV.get("SOAP_WSDL");
    }

    public static URL wsdlLocal() {
        URL recurso = SoapConfig.class.getResource("/WSConversorUnidades.wsdl");
        if (recurso == null) {
            throw new IllegalStateException(
                    "No se encontró WSConversorUnidades.wsdl en el classpath");
        }
        return recurso;
    }

    public static int connectTimeout() {
        return Integer.parseInt(requerido("SOAP_CONNECT_TIMEOUT"));
    }

    public static int readTimeout() {
        return Integer.parseInt(requerido("SOAP_READ_TIMEOUT"));
    }

    public static boolean insecureTls() {
        return Boolean.parseBoolean(DOTENV.get("SOAP_INSECURE_TLS", "false"));
    }
}
