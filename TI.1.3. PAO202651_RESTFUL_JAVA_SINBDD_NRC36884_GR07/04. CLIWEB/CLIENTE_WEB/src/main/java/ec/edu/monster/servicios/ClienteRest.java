package ec.edu.monster.servicios;

import jakarta.enterprise.context.ApplicationScoped;
import jakarta.json.Json;
import jakarta.json.JsonObject;
import jakarta.json.JsonReader;
import java.io.IOException;
import java.io.StringReader;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;

/** Cliente HTTP centralizado para el servicio RESTful de conversiones. */
@ApplicationScoped
public class ClienteRest {

    public static final String ENDPOINT_PROPERTY = "monster.rest.base-url";
    public static final String ENDPOINT_ENV = "MONSTER_REST_BASE_URL";
    private static final String ENDPOINT_PREDETERMINADO =
            "http://127.0.0.1:9080/CONV_REST_JAVA/api";

    private final HttpClient httpClient = HttpClient.newBuilder()
            .connectTimeout(Duration.ofSeconds(10))
            .build();

    public JsonObject post(String ruta, JsonObject cuerpo, String token) {
        HttpRequest.Builder solicitud = HttpRequest.newBuilder()
                .uri(URI.create(getBaseUrl() + ruta))
                .timeout(Duration.ofSeconds(20))
                .header("Accept", "application/json")
                .header("Content-Type", "application/json")
                .POST(HttpRequest.BodyPublishers.ofString(cuerpo.toString()));

        if (token != null && !token.isBlank()) {
            solicitud.header("Authorization", "Bearer " + token);
        }

        try {
            HttpResponse<String> respuesta = httpClient.send(
                    solicitud.build(), HttpResponse.BodyHandlers.ofString());
            JsonObject json = leerJson(respuesta.body());
            if (respuesta.statusCode() < 200 || respuesta.statusCode() >= 300) {
                throw new ApiRestException(mensajeError(respuesta.statusCode(), json));
            }
            return json;
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
            throw new ApiRestException("La solicitud al servidor fue interrumpida.", e);
        } catch (IOException e) {
            throw new ApiRestException("No fue posible conectar con el servidor REST: "
                    + getBaseUrl(), e);
        }
    }

    private JsonObject leerJson(String contenido) {
        if (contenido == null || contenido.isBlank()) {
            return Json.createObjectBuilder().build();
        }
        try (JsonReader lector = Json.createReader(new StringReader(contenido))) {
            return lector.readObject();
        } catch (RuntimeException e) {
            throw new ApiRestException("El servidor devolvió una respuesta JSON inválida.", e);
        }
    }

    private String mensajeError(int codigoHttp, JsonObject error) {
        String detalle = error.getString("detalles", error.getString("mensaje", ""));
        if (detalle.isBlank()) {
            detalle = "Error HTTP " + codigoHttp + ".";
        }
        return codigoHttp == 401
                ? "Sesión expirada o no autorizada. Inicie sesión nuevamente."
                : detalle;
    }

    private String getBaseUrl() {
        String url = System.getProperty(ENDPOINT_PROPERTY);
        if (url == null || url.isBlank()) {
            url = System.getenv(ENDPOINT_ENV);
        }
        if (url == null || url.isBlank()) {
            url = ENDPOINT_PREDETERMINADO;
        }
        return url.trim().replaceAll("/+$", "");
    }
}
