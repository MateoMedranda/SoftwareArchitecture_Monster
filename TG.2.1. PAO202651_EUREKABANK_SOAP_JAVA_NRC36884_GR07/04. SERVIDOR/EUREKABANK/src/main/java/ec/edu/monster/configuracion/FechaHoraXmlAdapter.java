package ec.edu.monster.configuracion;

import jakarta.xml.bind.annotation.adapters.XmlAdapter;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

public final class FechaHoraXmlAdapter extends XmlAdapter<String, LocalDateTime> {
    private static final DateTimeFormatter FORMATO = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");

    @Override
    public LocalDateTime unmarshal(String valor) {
        return valor == null || valor.isBlank() ? null : LocalDateTime.parse(valor, FORMATO);
    }

    @Override
    public String marshal(LocalDateTime valor) {
        return valor == null ? null : FORMATO.format(valor);
    }
}
