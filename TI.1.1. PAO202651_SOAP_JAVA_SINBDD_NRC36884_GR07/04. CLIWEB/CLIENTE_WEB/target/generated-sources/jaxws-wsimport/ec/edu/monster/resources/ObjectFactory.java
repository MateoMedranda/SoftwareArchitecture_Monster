
package ec.edu.monster.resources;

import javax.xml.namespace.QName;
import jakarta.xml.bind.JAXBElement;
import jakarta.xml.bind.annotation.XmlElementDecl;
import jakarta.xml.bind.annotation.XmlRegistry;


/**
 * This object contains factory methods for each 
 * Java content interface and Java element interface 
 * generated in the ec.edu.monster.resources package. 
 * <p>An ObjectFactory allows you to programmatically 
 * construct new instances of the Java representation 
 * for XML content. The Java representation of XML 
 * content can consist of schema derived interfaces 
 * and classes representing the binding of schema 
 * type definitions, element declarations and model 
 * groups.  Factory methods for each of these are 
 * provided in this class.
 * 
 */
@XmlRegistry
public class ObjectFactory {

    private static final QName _CambiarContrasenia_QNAME = new QName("http://ws.monster.edu.ec/", "cambiarContrasenia");
    private static final QName _CambiarContraseniaResponse_QNAME = new QName("http://ws.monster.edu.ec/", "cambiarContraseniaResponse");
    private static final QName _ConvertirLongitud_QNAME = new QName("http://ws.monster.edu.ec/", "convertirLongitud");
    private static final QName _ConvertirLongitudResponse_QNAME = new QName("http://ws.monster.edu.ec/", "convertirLongitudResponse");
    private static final QName _ConvertirMasa_QNAME = new QName("http://ws.monster.edu.ec/", "convertirMasa");
    private static final QName _ConvertirMasaResponse_QNAME = new QName("http://ws.monster.edu.ec/", "convertirMasaResponse");
    private static final QName _ConvertirTemperatura_QNAME = new QName("http://ws.monster.edu.ec/", "convertirTemperatura");
    private static final QName _ConvertirTemperaturaResponse_QNAME = new QName("http://ws.monster.edu.ec/", "convertirTemperaturaResponse");
    private static final QName _Login_QNAME = new QName("http://ws.monster.edu.ec/", "login");
    private static final QName _LoginResponse_QNAME = new QName("http://ws.monster.edu.ec/", "loginResponse");

    /**
     * Create a new ObjectFactory that can be used to create new instances of schema derived classes for package: ec.edu.monster.resources
     * 
     */
    public ObjectFactory() {
    }

    /**
     * Create an instance of {@link CambiarContrasenia }
     * 
     * @return
     *     the new instance of {@link CambiarContrasenia }
     */
    public CambiarContrasenia createCambiarContrasenia() {
        return new CambiarContrasenia();
    }

    /**
     * Create an instance of {@link CambiarContraseniaResponse }
     * 
     * @return
     *     the new instance of {@link CambiarContraseniaResponse }
     */
    public CambiarContraseniaResponse createCambiarContraseniaResponse() {
        return new CambiarContraseniaResponse();
    }

    /**
     * Create an instance of {@link ConvertirLongitud }
     * 
     * @return
     *     the new instance of {@link ConvertirLongitud }
     */
    public ConvertirLongitud createConvertirLongitud() {
        return new ConvertirLongitud();
    }

    /**
     * Create an instance of {@link ConvertirLongitudResponse }
     * 
     * @return
     *     the new instance of {@link ConvertirLongitudResponse }
     */
    public ConvertirLongitudResponse createConvertirLongitudResponse() {
        return new ConvertirLongitudResponse();
    }

    /**
     * Create an instance of {@link ConvertirMasa }
     * 
     * @return
     *     the new instance of {@link ConvertirMasa }
     */
    public ConvertirMasa createConvertirMasa() {
        return new ConvertirMasa();
    }

    /**
     * Create an instance of {@link ConvertirMasaResponse }
     * 
     * @return
     *     the new instance of {@link ConvertirMasaResponse }
     */
    public ConvertirMasaResponse createConvertirMasaResponse() {
        return new ConvertirMasaResponse();
    }

    /**
     * Create an instance of {@link ConvertirTemperatura }
     * 
     * @return
     *     the new instance of {@link ConvertirTemperatura }
     */
    public ConvertirTemperatura createConvertirTemperatura() {
        return new ConvertirTemperatura();
    }

    /**
     * Create an instance of {@link ConvertirTemperaturaResponse }
     * 
     * @return
     *     the new instance of {@link ConvertirTemperaturaResponse }
     */
    public ConvertirTemperaturaResponse createConvertirTemperaturaResponse() {
        return new ConvertirTemperaturaResponse();
    }

    /**
     * Create an instance of {@link Login }
     * 
     * @return
     *     the new instance of {@link Login }
     */
    public Login createLogin() {
        return new Login();
    }

    /**
     * Create an instance of {@link LoginResponse }
     * 
     * @return
     *     the new instance of {@link LoginResponse }
     */
    public LoginResponse createLoginResponse() {
        return new LoginResponse();
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link CambiarContrasenia }{@code >}
     * 
     * @param value
     *     Java instance representing xml element's value.
     * @return
     *     the new instance of {@link JAXBElement }{@code <}{@link CambiarContrasenia }{@code >}
     */
    @XmlElementDecl(namespace = "http://ws.monster.edu.ec/", name = "cambiarContrasenia")
    public JAXBElement<CambiarContrasenia> createCambiarContrasenia(CambiarContrasenia value) {
        return new JAXBElement<>(_CambiarContrasenia_QNAME, CambiarContrasenia.class, null, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link CambiarContraseniaResponse }{@code >}
     * 
     * @param value
     *     Java instance representing xml element's value.
     * @return
     *     the new instance of {@link JAXBElement }{@code <}{@link CambiarContraseniaResponse }{@code >}
     */
    @XmlElementDecl(namespace = "http://ws.monster.edu.ec/", name = "cambiarContraseniaResponse")
    public JAXBElement<CambiarContraseniaResponse> createCambiarContraseniaResponse(CambiarContraseniaResponse value) {
        return new JAXBElement<>(_CambiarContraseniaResponse_QNAME, CambiarContraseniaResponse.class, null, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link ConvertirLongitud }{@code >}
     * 
     * @param value
     *     Java instance representing xml element's value.
     * @return
     *     the new instance of {@link JAXBElement }{@code <}{@link ConvertirLongitud }{@code >}
     */
    @XmlElementDecl(namespace = "http://ws.monster.edu.ec/", name = "convertirLongitud")
    public JAXBElement<ConvertirLongitud> createConvertirLongitud(ConvertirLongitud value) {
        return new JAXBElement<>(_ConvertirLongitud_QNAME, ConvertirLongitud.class, null, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link ConvertirLongitudResponse }{@code >}
     * 
     * @param value
     *     Java instance representing xml element's value.
     * @return
     *     the new instance of {@link JAXBElement }{@code <}{@link ConvertirLongitudResponse }{@code >}
     */
    @XmlElementDecl(namespace = "http://ws.monster.edu.ec/", name = "convertirLongitudResponse")
    public JAXBElement<ConvertirLongitudResponse> createConvertirLongitudResponse(ConvertirLongitudResponse value) {
        return new JAXBElement<>(_ConvertirLongitudResponse_QNAME, ConvertirLongitudResponse.class, null, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link ConvertirMasa }{@code >}
     * 
     * @param value
     *     Java instance representing xml element's value.
     * @return
     *     the new instance of {@link JAXBElement }{@code <}{@link ConvertirMasa }{@code >}
     */
    @XmlElementDecl(namespace = "http://ws.monster.edu.ec/", name = "convertirMasa")
    public JAXBElement<ConvertirMasa> createConvertirMasa(ConvertirMasa value) {
        return new JAXBElement<>(_ConvertirMasa_QNAME, ConvertirMasa.class, null, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link ConvertirMasaResponse }{@code >}
     * 
     * @param value
     *     Java instance representing xml element's value.
     * @return
     *     the new instance of {@link JAXBElement }{@code <}{@link ConvertirMasaResponse }{@code >}
     */
    @XmlElementDecl(namespace = "http://ws.monster.edu.ec/", name = "convertirMasaResponse")
    public JAXBElement<ConvertirMasaResponse> createConvertirMasaResponse(ConvertirMasaResponse value) {
        return new JAXBElement<>(_ConvertirMasaResponse_QNAME, ConvertirMasaResponse.class, null, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link ConvertirTemperatura }{@code >}
     * 
     * @param value
     *     Java instance representing xml element's value.
     * @return
     *     the new instance of {@link JAXBElement }{@code <}{@link ConvertirTemperatura }{@code >}
     */
    @XmlElementDecl(namespace = "http://ws.monster.edu.ec/", name = "convertirTemperatura")
    public JAXBElement<ConvertirTemperatura> createConvertirTemperatura(ConvertirTemperatura value) {
        return new JAXBElement<>(_ConvertirTemperatura_QNAME, ConvertirTemperatura.class, null, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link ConvertirTemperaturaResponse }{@code >}
     * 
     * @param value
     *     Java instance representing xml element's value.
     * @return
     *     the new instance of {@link JAXBElement }{@code <}{@link ConvertirTemperaturaResponse }{@code >}
     */
    @XmlElementDecl(namespace = "http://ws.monster.edu.ec/", name = "convertirTemperaturaResponse")
    public JAXBElement<ConvertirTemperaturaResponse> createConvertirTemperaturaResponse(ConvertirTemperaturaResponse value) {
        return new JAXBElement<>(_ConvertirTemperaturaResponse_QNAME, ConvertirTemperaturaResponse.class, null, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link Login }{@code >}
     * 
     * @param value
     *     Java instance representing xml element's value.
     * @return
     *     the new instance of {@link JAXBElement }{@code <}{@link Login }{@code >}
     */
    @XmlElementDecl(namespace = "http://ws.monster.edu.ec/", name = "login")
    public JAXBElement<Login> createLogin(Login value) {
        return new JAXBElement<>(_Login_QNAME, Login.class, null, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link LoginResponse }{@code >}
     * 
     * @param value
     *     Java instance representing xml element's value.
     * @return
     *     the new instance of {@link JAXBElement }{@code <}{@link LoginResponse }{@code >}
     */
    @XmlElementDecl(namespace = "http://ws.monster.edu.ec/", name = "loginResponse")
    public JAXBElement<LoginResponse> createLoginResponse(LoginResponse value) {
        return new JAXBElement<>(_LoginResponse_QNAME, LoginResponse.class, null, value);
    }

}
