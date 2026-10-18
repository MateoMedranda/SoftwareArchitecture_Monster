package ec.edu.monster.model.network

import android.util.Log
import ec.edu.monster.model.SoapConfig
import ec.edu.monster.model.seguridad.TlsInseguro
import java.io.StringReader
import java.net.HttpURLConnection
import java.net.URL
import org.xmlpull.v1.XmlPullParser
import org.xmlpull.v1.XmlPullParserFactory

/** Error de red o de protocolo SOAP (incluye SOAP Fault). */
open class SoapException(message: String, cause: Throwable? = null) : Exception(message, cause)

/** El token venció o es inválido: hay que iniciar sesión de nuevo. */
class SessionExpiredException(message: String) : SoapException(message)

/**
 * Cliente SOAP mínimo basado en HttpURLConnection, sin librerías externas.
 * Debe llamarse desde un hilo de fondo (nunca desde el hilo principal).
 */
class SoapClient(
    private val endpoint: String = SoapConfig.ENDPOINT,
    private val namespace: String = SoapConfig.NAMESPACE,
    private val security: String = SoapConfig.SOAP_SECURITY_NAMESPACE
) {

    /** Devuelve el token UUID. El login es la única operación que no lleva token. */
    fun login(usuario: String, contrasenia: String): String =
        call("login", listOf("usuario" to usuario, "contrasenia" to contrasenia))

    /** Llama a convertirLongitud / convertirMasa / convertirTemperatura enviando el token en el header. */
    fun convert(
        operacion: String,
        valor: Double,
        unidadInicial: String,
        unidadFinal: String,
        token: String?
    ): Double {
        val text = call(
            operacion,
            listOf(
                "valor" to valor.toString(),
                "unidadInicial" to unidadInicial,
                "unidadFinal" to unidadFinal
            ),
            token
        )
        return text.toDoubleOrNull() ?: throw SoapException("Respuesta inválida del servidor: $text")
    }

    /**
     * Envía <ws:operation> con los parámetros dados y devuelve el texto de <return>.
     * Si [token] no es null, va en <soapenv:Header><seg:token>.
     */
    fun call(operation: String, params: List<Pair<String, String>>, token: String? = null): String {
        val header = if (token != null) {
            "<soapenv:Header>\n    <seg:token>${escapeXml(token)}</seg:token>\n  </soapenv:Header>"
        } else {
            "<soapenv:Header/>"
        }
        val body = params.joinToString("\n") { (name, value) ->
            "      <$name>${escapeXml(value)}</$name>"
        }
        val envelope = """<?xml version="1.0" encoding="UTF-8"?>
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ws="$namespace" xmlns:seg="$security">
  $header
  <soapenv:Body>
    <ws:$operation>
$body
    </ws:$operation>
  </soapenv:Body>
</soapenv:Envelope>"""
        return parseReturn(post(envelope))
    }

    private fun post(body: String): String {
        val url = URL(endpoint)
        val conn = (url.openConnection() as HttpURLConnection).apply {
            requestMethod = "POST"
            connectTimeout = 10_000
            readTimeout = 15_000
            doOutput = true
            setRequestProperty("Content-Type", "text/xml; charset=utf-8")
            setRequestProperty("SOAPAction", "\"\"")   // el WSDL declara soapAction=""
        }
        TlsInseguro.aplicarA(conn, url.host)           // solo debug; en release no hace nada
        try {
            conn.outputStream.use { it.write(body.toByteArray(Charsets.UTF_8)) }
            // Los SOAP Fault llegan con HTTP 500 pero con cuerpo XML útil.
            val stream = if (conn.responseCode in 200..299) conn.inputStream else conn.errorStream
            val text = stream?.bufferedReader(Charsets.UTF_8)?.use { it.readText() }.orEmpty()
            if (text.isBlank()) throw SoapException("Respuesta vacía (HTTP ${conn.responseCode})")
            return text
        } catch (e: SoapException) {
            throw e
        } catch (e: Exception) {
            Log.e("SoapClient", "Fallo SOAP hacia $endpoint", e)
            throw SoapException("No se pudo conectar con el servidor", e)
        } finally {
            conn.disconnect()
        }
    }
    /** Extrae <token> o lanza SoapException si hay <faultstring>. */
    /** Extrae <return> (JAX-WS) o <token>; lanza SoapException si hay <faultstring>. */
    private fun parseReturn(xml: String): String {
        val parser = XmlPullParserFactory.newInstance().newPullParser()
        parser.setInput(StringReader(xml))
        var result: String? = null
        var fault: String? = null
        var event = parser.eventType
        while (event != XmlPullParser.END_DOCUMENT) {
            if (event == XmlPullParser.START_TAG) {
                when (parser.name) {
                    "return", "token" -> result = parser.nextText().trim()
                    "faultstring" -> fault = parser.nextText().trim()
                }
            }
            event = parser.next()
        }
        fault?.let {
            throw if (isSessionExpired(it)) SessionExpiredException(it) else SoapException(it)
        }
        return result?.takeIf { it.isNotEmpty() }
            ?: throw SoapException("La respuesta no contiene un resultado")
    }

    private fun isSessionExpired(fault: String): Boolean {
        val f = fault.lowercase()
        return f.contains("token") && f.contains("expirado")
    }

    private fun escapeXml(s: String) = s
        .replace("&", "&amp;")
        .replace("<", "&lt;")
        .replace(">", "&gt;")
        .replace("\"", "&quot;")
        .replace("'", "&apos;")
}
