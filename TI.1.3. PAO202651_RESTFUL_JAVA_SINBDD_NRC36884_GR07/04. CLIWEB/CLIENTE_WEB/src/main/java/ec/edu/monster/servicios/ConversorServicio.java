package ec.edu.monster.servicios;

import ec.edu.monster.modelos.ResultadoConversion;
import jakarta.enterprise.context.ApplicationScoped;
import jakarta.inject.Inject;
import jakarta.json.Json;
import jakarta.json.JsonObject;

@ApplicationScoped
public class ConversorServicio {

    @Inject
    private ClienteRest clienteRest;

    public ResultadoConversion convertirLongitud(String token, double valor, String origen, String destino) {
        return convertir("/conversion/longitud", token, valor, origen, destino);
    }

    public ResultadoConversion convertirMasa(String token, double valor, String origen, String destino) {
        return convertir("/conversion/masa", token, valor, origen, destino);
    }

    public ResultadoConversion convertirTemperatura(String token, double valor, String origen, String destino) {
        return convertir("/conversion/temperatura", token, valor, origen, destino);
    }

    private ResultadoConversion convertir(String ruta, String token, double valor,
            String origen, String destino) {
        JsonObject solicitud = Json.createObjectBuilder()
                .add("valor", valor)
                .add("unidadOrigen", origen)
                .add("unidadDestino", destino)
                .build();
        JsonObject respuesta = clienteRest.post(ruta, solicitud, token);
        return new ResultadoConversion(
                respuesta.getJsonNumber("valorOriginal").doubleValue(),
                respuesta.getJsonNumber("valorConvertido").doubleValue(),
                respuesta.getString("unidadOrigen"),
                respuesta.getString("unidadDestino"));
    }
}
