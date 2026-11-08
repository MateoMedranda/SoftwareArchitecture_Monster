package ec.edu.monster.servicios;

import com.google.gson.Gson;
import com.google.gson.JsonObject;
import ec.edu.monster.configuracion.RestConfig;
import ec.edu.monster.modelos.RespuestaError;
import ec.edu.monster.seguridad.AdministradorTokensCliente;
import ec.edu.monster.seguridad.TlsInseguro;
import java.io.IOException;
import java.net.ConnectException;
import java.net.URI;
import java.net.UnknownHostException;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.net.http.HttpTimeoutException;
import java.time.Duration;
import java.util.function.Supplier;
import javax.net.ssl.SSLException;

/**
 * Capa de acceso al servicio RESTful. Mantiene el token sólo durante la sesión actual.
 * Los fallos se reportan como {@link ExcepcionServicio}: los de red/configuración sin
 * respuesta, y los 4xx/5xx con el cuerpo estructurado (RespuestaError) de la spec.
 */
public class WSConversorCliente {
    private static final String TIPO_JSON = "application/json";
    private final HttpClient http;
    private final Gson gson = new Gson();

    public WSConversorCliente() {
        HttpClient.Builder constructor = HttpClient.newBuilder()
                .connectTimeout(Duration.ofMillis(configurado(RestConfig::connectTimeout)));
        if (RestConfig.insecureTls()) {
            constructor.sslContext(TlsInseguro.contextoInseguro())
                    .sslParameters(TlsInseguro.parametrosSinVerificacionDeHost());
        }
        http = constructor.build();
    }

    /**
     * Intenta iniciar sesión. Devuelve {@code false} sólo cuando el servicio rechaza
     * las credenciales (401) o responde sin token; lanza {@link ExcepcionServicio}
     * cuando falla la comunicación, la configuración o el servicio mismo.
     */
    public boolean login(String usuario, String contrasenia) {
        JsonObject solicitud = new JsonObject();
        solicitud.addProperty("usuario", usuario);
        solicitud.addProperty("contrasena", contrasenia);
        HttpResponse<String> respuesta = enviar(
                configurado(RestConfig::endpointAutenticacion), solicitud.toString(), false);
        if (respuesta.statusCode() != 200) {
            AdministradorTokensCliente.limpiar();
            if (respuesta.statusCode() == 401) {
                return false;
            }
            throw errorDeRespuesta(respuesta);
        }
        JsonObject cuerpo = jsonPermitido(respuesta.body());
        String token = (cuerpo == null || !esTexto(cuerpo, "token")) ? null
                : cuerpo.get("token").getAsString().trim();
        if (token == null || token.isBlank()) {
            AdministradorTokensCliente.limpiar();
            return false;
        }
        AdministradorTokensCliente.guardarToken(token);
        return true;
    }

    public double convertirTemperatura(double valor, String origen, String destino) {
        return convertir(configurado(RestConfig::endpointConvertirTemperatura), valor, origen, destino);
    }

    public double convertirMasa(double valor, String origen, String destino) {
        return convertir(configurado(RestConfig::endpointConvertirMasa), valor, origen, destino);
    }

    public double convertirLongitud(double valor, String origen, String destino) {
        return convertir(configurado(RestConfig::endpointConvertirLongitud), valor, origen, destino);
    }

    private double convertir(String ruta, double valor, String origen, String destino) {
        JsonObject solicitud = new JsonObject();
        solicitud.addProperty("valor", valor);
        solicitud.addProperty("unidadOrigen", origen);
        solicitud.addProperty("unidadDestino", destino);
        HttpResponse<String> respuesta = enviar(ruta, solicitud.toString(), true);
        if (respuesta.statusCode() != 200) {
            if (respuesta.statusCode() == 401) {
                // Token ausente o inválido: no dejar una sesión viva que ya no sirve.
                AdministradorTokensCliente.limpiar();
            }
            throw errorDeRespuesta(respuesta);
        }
        JsonObject cuerpo = jsonPermitido(respuesta.body());
        if (cuerpo == null || !esTexto(cuerpo, "valorConvertido")) {
            throw ExcepcionServicio.inesperada("El servicio RESTful devolvió una respuesta inesperada.", null);
        }
        try {
            return cuerpo.get("valorConvertido").getAsDouble();
        } catch (RuntimeException ex) {
            throw ExcepcionServicio.inesperada("El servicio RESTful devolvió un valor convertido no numérico.", ex);
        }
    }

    private HttpResponse<String> enviar(String ruta, String cuerpo, boolean conToken) {
        HttpRequest.Builder solicitud = HttpRequest.newBuilder()
                .uri(URI.create(configurado(RestConfig::baseUrl) + ruta))
                .timeout(Duration.ofMillis(configurado(RestConfig::readTimeout)))
                .header("Accept", TIPO_JSON)
                .header("Content-Type", TIPO_JSON)
                .POST(HttpRequest.BodyPublishers.ofString(cuerpo));
        if (conToken) {
            String token = AdministradorTokensCliente.obtenerToken();
            if (token != null && !token.isBlank()) {
                solicitud.header("Authorization", "Bearer " + token);
            }
        }
        try {
            return http.send(solicitud.build(), HttpResponse.BodyHandlers.ofString());
        } catch (InterruptedException ex) {
            Thread.currentThread().interrupt();
            throw ExcepcionServicio.deRed("Se interrumpió la comunicación con el servicio RESTful.", ex);
        } catch (HttpTimeoutException ex) {
            throw ExcepcionServicio.deRed("El servicio no respondió dentro del tiempo esperado.", ex);
        } catch (ConnectException ex) {
            throw ExcepcionServicio.deRed(
                    "No se pudo conectar con el servicio RESTful. Verifique que esté desplegado y accesible.", ex);
        } catch (UnknownHostException ex) {
            throw ExcepcionServicio.deRed(
                    "No se resolvió el nombre del servidor '" + ex.getMessage()
                            + "'. Verifique la configuración de red.", ex);
        } catch (SSLException ex) {
            throw ExcepcionServicio.deRed("Error de seguridad TLS/SSL al contactar al servicio: " + ex.getMessage(), ex);
        } catch (IOException ex) {
            throw ExcepcionServicio.deRed("No fue posible comunicarse con el servicio RESTful.", ex);
        }
    }

    /** Convierte una respuesta de error (4xx/5xx) en una ExcepcionServicio con el detalle del servicio. */
    private ExcepcionServicio errorDeRespuesta(HttpResponse<String> respuesta) {
        RespuestaError error = parsearError(respuesta.body());
        if (error != null) {
            // El código HTTP real prevalece sobre el declarado en el cuerpo.
            error.setCodigo(respuesta.statusCode());
        }
        return ExcepcionServicio.deRespuesta(respuesta.statusCode(), error,
                "El servicio RESTful respondió con el código " + respuesta.statusCode() + ".");
    }

    /** Extrae el cuerpo de error (codigo/mensaje/detalles/fechaHora) sin fallar con formatos inesperados. */
    private RespuestaError parsearError(String cuerpo) {
        JsonObject json = jsonPermitido(cuerpo);
        if (json == null || json.entrySet().isEmpty()) {
            return null;
        }
        RespuestaError error = new RespuestaError();
        if (esEntero(json, "codigo")) {
            error.setCodigo(json.get("codigo").getAsInt());
        }
        if (esTexto(json, "mensaje")) {
            error.setMensaje(json.get("mensaje").getAsString());
        }
        if (esTexto(json, "detalles")) {
            error.setDetalles(json.get("detalles").getAsString());
        }
        if (esTexto(json, "fechaHora")) {
            error.setFechaHora(json.get("fechaHora").getAsString());
        }
        return error;
    }

    /** Parsea un cuerpo JSON devolviendo {@code null} (sin lanzar) si no es un objeto válido. */
    private JsonObject jsonPermitido(String cuerpo) {
        try {
            return gson.fromJson(cuerpo, JsonObject.class);
        } catch (RuntimeException ex) {
            return null;
        }
    }

    private boolean esTexto(JsonObject json, String campo) {
        return json.has(campo) && json.get(campo).isJsonPrimitive();
    }

    private boolean esEntero(JsonObject json, String campo) {
        try {
            return esTexto(json, campo) && json.get(campo).getAsNumber().intValue() >= 0;
        } catch (RuntimeException ex) {
            return false;
        }
    }

    /** Lee una variable de configuración convirtiendo su error en uno de servicio comprensible. */
    private static <T> T configurado(Supplier<T> proveedor) {
        try {
            return proveedor.get();
        } catch (IllegalStateException ex) {
            throw ExcepcionServicio.deRed("La configuración del cliente RESTful no es válida: " + ex.getMessage(), ex);
        }
    }

    public void cerrarSesion() { AdministradorTokensCliente.limpiar(); }
}
