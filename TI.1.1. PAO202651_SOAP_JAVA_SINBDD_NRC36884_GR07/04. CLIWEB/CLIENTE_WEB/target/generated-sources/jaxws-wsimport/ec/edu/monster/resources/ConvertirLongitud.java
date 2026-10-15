
package ec.edu.monster.resources;

import jakarta.xml.bind.annotation.XmlAccessType;
import jakarta.xml.bind.annotation.XmlAccessorType;
import jakarta.xml.bind.annotation.XmlType;


/**
 * &lt;p&gt;Clase Java para convertirLongitud complex type.&lt;/p&gt;
 * 
 * &lt;p&gt;El siguiente fragmento de esquema especifica el contenido que se espera que haya en esta clase.&lt;/p&gt;
 * 
 * &lt;pre&gt;{&#064;code
 * &lt;complexType name="convertirLongitud"&gt;
 *   &lt;complexContent&gt;
 *     &lt;restriction base="{http://www.w3.org/2001/XMLSchema}anyType"&gt;
 *       &lt;sequence&gt;
 *         &lt;element name="valor" type="{http://www.w3.org/2001/XMLSchema}double"/&gt;
 *         &lt;element name="unidadInicial" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="unidadFinal" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *       &lt;/sequence&gt;
 *     &lt;/restriction&gt;
 *   &lt;/complexContent&gt;
 * &lt;/complexType&gt;
 * }&lt;/pre&gt;
 * 
 * 
 */
@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "convertirLongitud", propOrder = {
    "valor",
    "unidadInicial",
    "unidadFinal"
})
public class ConvertirLongitud {

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
