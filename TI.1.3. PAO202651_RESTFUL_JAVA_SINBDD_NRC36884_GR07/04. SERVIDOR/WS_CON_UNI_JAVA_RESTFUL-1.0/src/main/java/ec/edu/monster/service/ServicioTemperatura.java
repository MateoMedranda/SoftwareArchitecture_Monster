package ec.edu.monster.service;

import ec.edu.monster.exception.ExcepcionTemperaturaInvalida;
import ec.edu.monster.model.RespuestaConversion;
import ec.edu.monster.model.SolicitudConversion;
import ec.edu.monster.model.UnidadTemperatura;
import jakarta.enterprise.context.ApplicationScoped;

@ApplicationScoped
public class ServicioTemperatura {

    public ServicioTemperatura() {}

    public RespuestaConversion convertir(SolicitudConversion solicitud) {
        UnidadTemperatura unidadOrigen = UnidadTemperatura.desdeString(solicitud.getUnidadOrigen());
        UnidadTemperatura unidadDestino = UnidadTemperatura.desdeString(solicitud.getUnidadDestino());
        double valor = solicitud.getValor();

        unidadOrigen.validar(valor);

        double valorEnKelvin = aKelvin(valor, unidadOrigen);
        double valorConvertido = desdeKelvin(valorEnKelvin, unidadDestino);

        return new RespuestaConversion(valor, valorConvertido, unidadOrigen.name(), unidadDestino.name());
    }

    private double aKelvin(double valor, UnidadTemperatura unidad) {
        return switch (unidad) {
            case CELSIUS -> valor + 273.15;
            case FAHRENHEIT -> (valor + 459.67) * 5.0 / 9.0;
            case KELVIN -> valor;
            case RANKINE -> valor * 5.0 / 9.0;
        };
    }

    private double desdeKelvin(double kelvin, UnidadTemperatura unidad) {
        return switch (unidad) {
            case CELSIUS -> kelvin - 273.15;
            case FAHRENHEIT -> kelvin * 9.0 / 5.0 - 459.67;
            case KELVIN -> kelvin;
            case RANKINE -> kelvin * 9.0 / 5.0;
        };
    }
}