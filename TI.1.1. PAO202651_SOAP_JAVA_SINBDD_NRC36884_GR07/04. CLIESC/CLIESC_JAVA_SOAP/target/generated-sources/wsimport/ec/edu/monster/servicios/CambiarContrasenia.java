
package ec.edu.monster.servicios;

import jakarta.xml.bind.annotation.XmlAccessType;
import jakarta.xml.bind.annotation.XmlAccessorType;
import jakarta.xml.bind.annotation.XmlType;


/**
 * <p>Clase Java para cambiarContrasenia complex type.</p>
 * 
 * <p>El siguiente fragmento de esquema especifica el contenido que se espera que haya en esta clase.</p>
 * 
 * <pre>{@code
 * <complexType name="cambiarContrasenia">
 *   <complexContent>
 *     <restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
 *       <sequence>
 *         <element name="contraseniaActual" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/>
 *         <element name="contraseniaNueva" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/>
 *       </sequence>
 *     </restriction>
 *   </complexContent>
 * </complexType>
 * }</pre>
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
