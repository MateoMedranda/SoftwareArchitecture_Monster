/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Interface.java to edit this template
 */
package ec.edu.monster.servicios;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import jakarta.jws.WebResult;
import jakarta.xml.ws.RequestWrapper;
import jakarta.xml.bind.annotation.XmlSeeAlso;
import jakarta.xml.ws.Action;
import jakarta.xml.ws.ResponseWrapper;

/**
 *
 * @author gmlop
 */
@WebService(
    name = "WSConversorUnidades",
    targetNamespace = "http://ws.monster.edu.ec/"
)    
@XmlSeeAlso({ObjectFactory.class})
public interface WSConversorUnidadesInterface {
    
    @WebMethod
    @WebResult(targetNamespace = "")
    @RequestWrapper(
        localName = "Convertir Temperatura",
        targetNamespace = "http://ws.monster.edu.ec/",
        className = "ec.edu.monster.cliente.service.generated.ConvertirTemperatura"
            
    )
    @ResponseWrapper(
        localName = "convertirTemperaturaResponse",
        targetNamespace = "http://ws.monster.edu.ec/",
        className = "ec.edu.monster.cliente.service.generated.ConvertirTemperaturaResponse"
    )
    @Action(
        input = "http://ws.monster.edu.ec/WSConversorUnidades/convertirTemperaturaRequest",
        output = "http://ws.monster.edu.ec/WSConversorUnidades/convertirTemperaturaResponse"
    )
    double ConvertirTemperatura(
        @WebParam(name = "valor",targetNamespace = "")double valor,
        @WebParam(name = "origen",targetNamespace = "")String origen,
        @WebParam(name = "fin",targetNamespace = "")String fin);
    @WebMethod
    @WebResult(targetNamespace = "")
    @RequestWrapper(localName = "convertirLongitud", targetNamespace = "http://ws.monster.edu.ec/")
    @ResponseWrapper(localName = "convertirLongitudResponse", targetNamespace = "http://ws.monster.edu.ec/")
    @Action(
        input = "http://ws.monster.edu.ec/WSConversorUnidades/convertirLongitudRequest",
        output = "http://ws.monster.edu.ec/WSConversorUnidades/convertirLongitudResponse"
    )
            
    double convertirLongitud(
        @WebParam(name = "arg0", targetNamespace = "") double arg0,
        @WebParam(name = "arg1", targetNamespace = "") String arg1,
        @WebParam(name = "arg2", targetNamespace = "") String arg2
    );

    @WebMethod
    @WebResult(targetNamespace = "")
    @RequestWrapper(localName = "convertirMasa", targetNamespace = "http://ws.monster.edu.ec/")
    @ResponseWrapper(localName = "convertirMasaResponse", targetNamespace = "http://ws.monster.edu.ec/")
    @Action(
        input = "http://ws.monster.edu.ec/WSConversorUnidades/convertirMasaRequest",
        output = "http://ws.monster.edu.ec/WSConversorUnidades/convertirMasaResponse"
    )
    double convertirMasa(
        @WebParam(name = "arg0", targetNamespace = "") double arg0,
        @WebParam(name = "arg1", targetNamespace = "") String arg1,
        @WebParam(name = "arg2", targetNamespace = "") String arg2
    );

    @WebMethod
    @WebResult(targetNamespace = "")
    @RequestWrapper(localName = "login", targetNamespace = "http://ws.monster.edu.ec/")
    @ResponseWrapper(localName = "loginResponse", targetNamespace = "http://ws.monster.edu.ec/")
    @Action(
        input = "http://ws.monster.edu.ec/WSConversorUnidades/loginRequest",
        output = "http://ws.monster.edu.ec/WSConversorUnidades/loginResponse"
    )
    boolean login(
        @WebParam(name = "arg0", targetNamespace = "") String arg0,
        @WebParam(name = "arg1", targetNamespace = "") String arg1
    );

    @WebMethod
    @WebResult(targetNamespace = "")
    @RequestWrapper(localName = "cambiarContrasenia", targetNamespace = "http://ws.monster.edu.ec/")
    @ResponseWrapper(localName = "cambiarContraseniaResponse", targetNamespace = "http://ws.monster.edu.ec/")
    @Action(
        input = "http://ws.monster.edu.ec/WSConversorUnidades/cambiarContraseniaRequest",
        output = "http://ws.monster.edu.ec/WSConversorUnidades/cambiarContraseniaResponse"
    )
    boolean cambiarContrasenia(
        @WebParam(name = "arg0", targetNamespace = "") String arg0,
        @WebParam(name = "arg1", targetNamespace = "") String arg1,
        @WebParam(name = "arg2", targetNamespace = "") String arg2
    );
}
