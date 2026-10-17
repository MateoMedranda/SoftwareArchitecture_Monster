package com.marpa.practica.model
import com.marpa.practica.BuildConfig

/**
 * Configuración del servicio SOAP. Ajusta estos valores a tu WSDL.
 * Nota: en el emulador, localhost del PC es 10.0.2.2.
 */
object SoapConfig {


    const val ENDPOINT = BuildConfig.SOAP_ENDPOINT   // URL del servicio
    const val NAMESPACE = BuildConfig.SOAP_TARGET_NAMESPACE // targetNamespace del WSDL
    const val LOGIN_ACTION = "$NAMESPACE/login"            // SOAPAction

    const val SOAP_WSDL = BuildConfig.SOAP_WSDL

    const val SOAP_CONNECT_TIMEOUT = BuildConfig.SOAP_CONNECT_TIMEOUT
    const val SOAP_READ_TIMEOUT = BuildConfig.SOAP_READ_TIMEOUT
    const val INSECURE_TLS = BuildConfig.SOAP_INSECURE_TLS
    const val SOAP_SECURITY_NAMESPACE = BuildConfig.SOAP_SECURITY_NAMESPACE
}
