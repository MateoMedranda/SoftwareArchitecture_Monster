
package ec.edu.monster.servicios;

import jakarta.xml.bind.annotation.XmlAccessType;
import jakarta.xml.bind.annotation.XmlAccessorType;
import jakarta.xml.bind.annotation.XmlType;


/**
 * <p>Clase Java para convertirMasa complex type.</p>
 * 
 * <p>El siguiente fragmento de esquema especifica el contenido que se espera que haya en esta clase.</p>
 * 
 * <pre>{@code
 * <complexType name="convertirMasa">
 *   <complexContent>
 *     <restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
 *       <sequence>
 *         <element name="valor" type="{http://www.w3.org/2001/XMLSchema}double"/>
 *         <element name="unidadInicial" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/>
 *         <element name="unidadFinal" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/>
 *       </sequence>
 *     </restriction>
 *   </complexContent>
 * </complexType>
 * }</pre>
 * 
 * 
 */
@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "convertirMasa", propOrder = {
    "valor",
    "unidadInicial",
    "unidadFinal"
})
public class ConvertirMasa {

    protected double valor;
    protected String unidadInicial;
    protected String unidadFinal;

    /**
     * Obtiene el valor de la propiedad valor.
     * 
     */
    public double getValor() {
        return valor;
    }

    /**
     * Define el valor de la propiedad valor.
     * 
     */
    public void setValor(double value) {
        this.valor = value;
    }

    /**
     * Obtiene el valor de la propiedad unidadInicial.
     * 
     * @return
     *     possible object is
     *     {@link String }
     *     
     */
    public String getUnidadInicial() {
        return unidadInicial;
    }

    /**
     * Define el valor de la propiedad unidadInicial.
     * 
     * @param value
     *     allowed object is
     *     {@link String }
     *     
     */
    public void setUnidadInicial(String value) {
        this.unidadInicial = value;
    }

    /**
     * Obtiene el valor de la propiedad unidadFinal.
     * 
     * @return
     *     possible object is
     *     {@link String }
     *     
     */
    public String getUnidadFinal() {
        return unidadFinal;
    }

    /**
     * Define el valor de la propiedad unidadFinal.
     * 
     * @param value
     *     allowed object is
     *     {@link String }
     *     
     */
    public void setUnidadFinal(String value) {
        this.unidadFinal = value;
    }

}
