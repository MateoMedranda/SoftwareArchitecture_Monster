/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.config;

import org.eclipse.microprofile.openapi.OASFilter;
import org.eclipse.microprofile.openapi.models.media.Schema;


/**
 *
 * @author marpa
 */
public class FiltroOpenApi implements OASFilter{
    @Override
    public Schema filterSchema(Schema s) {
        if (isZero(s.getMinLength())) s.setMinLength(null);
        if (isZero(s.getMinProperties())) s.setMinProperties(null);
        if (isZero(s.getMaxProperties())) s.setMaxProperties(null);
        if (isZero(s.getMinContains())) s.setMinContains(null);
        if (Boolean.FALSE.equals(s.getUniqueItems())) s.setUniqueItems(null);
        if (Boolean.FALSE.equals(s.getReadOnly())) s.setReadOnly(null);
        if (Boolean.FALSE.equals(s.getWriteOnly())) s.setWriteOnly(null);
        if (Boolean.FALSE.equals(s.getDeprecated())) s.setDeprecated(null);
        if (s.getExamples() != null) {
            var limpios = new java.util.ArrayList<Object>(s.getExamples());
            limpios.removeIf(e -> e == null || "".equals(e));
            s.setExamples(limpios.isEmpty() ? null : limpios);
        }
        return s;
    }

    private static boolean isZero(Integer n) { return n != null && n == 0;} 
}
