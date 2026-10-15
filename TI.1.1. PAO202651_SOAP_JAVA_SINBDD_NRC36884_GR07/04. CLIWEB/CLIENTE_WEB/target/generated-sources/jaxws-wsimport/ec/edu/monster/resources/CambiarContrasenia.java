
package ec.edu.monster.resources;

import jakarta.xml.bind.annotation.XmlAccessType;
import jakarta.xml.bind.annotation.XmlAccessorType;
import jakarta.xml.bind.annotation.XmlType;


/**
 * &lt;p&gt;Clase Java para cambiarContrasenia complex type.&lt;/p&gt;
 * 
 * &lt;p&gt;El siguiente fragmento de esquema especifica el contenido que se espera que haya en esta clase.&lt;/p&gt;
 * 
 * &lt;pre&gt;{&#064;code
 * &lt;complexType name="cambiarContrasenia"&gt;
 *   &lt;complexContent&gt;
 *     &lt;restriction base="{http://www.w3.org/2001/XMLSchema}anyType"&gt;
 *       &lt;sequence&gt;
 *         &lt;element name="contraseniaActual" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="contraseniaNueva" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *       &lt;/sequence&gt;
 *     &lt;/restriction&gt;
 *   &lt;/complexContent&gt;
 * &lt;/complexType&gt;
 * }&lt;/pre&gt;
 * 
 * 
 */
@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "cambiarContrasenia", propOrder = {
    "contraseniaActual",
    "contraseniaNueva"
})
public class CambiarContrasenia {

    protected String contraseniaActual;
    protected String contraseniaNueva;

    /**
     * Obtiene el valor de la propiedad contraseniaActual.
     * 
     * @return
     *     possible object is
     *     {@link String }
     *     
     */
    public String getContraseniaActual() {
        return contraseniaActual;
    }

    /**
     * Define el valor de la propiedad contraseniaActual.
     * 
     * @param value
     *     allowed object is
     *     {@link String }
     *     
     */
    public void setContraseniaActual(String value) {
        this.contraseniaActual = value;
    }

    /**
     * Obtiene el valor de la propiedad contraseniaNueva.
     * 
     * @return
     *     possible object is
     *     {@link String }
     *     
     */
    public String getContraseniaNueva() {
        return contraseniaNueva;
    }

    /**
     * Define el valor de la propiedad contraseniaNueva.
     * 
     * @param value
     *     allowed object is
     *     {@link String }
     *     
     */
    public void setContraseniaNueva(String value) {
        this.contraseniaNueva = value;
    }

}
