package com.marpa.practica.model.network

import com.marpa.practica.model.SoapConfig
import com.marpa.practica.model.seguridad.TlsInseguro
import java.io.StringReader
import java.net.HttpURLConnection
import java.net.URL
import org.xmlpull.v1.XmlPullParser
import org.xmlpull.v1.XmlPullParserFactory

/** Error de red o de protocolo SOAP (incluye SOAP Fault). */
class SoapException(message: String, cause: Throwable? = null) : Exception(message, cause)

/**
 * Cliente SOAP mínimo basado en HttpURLConnection, sin librerías externas.
 * Debe llamarse desde un hilo de fondo (nunca desde el hilo principal).
 */
class SoapClient(
    private val endpoint: String = SoapConfig.ENDPOINT,
    private val namespace: String = SoapConfig.NAMESPACE
) {

    /** Envía la operación Login y devuelve el valor del elemento <token>. */
    fun login(username: String, password: String): String {
        val envelope = """<?xml version="1.0" encoding="UTF-8"?>
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ws="$namespace">
  <soapenv:Header/>
  <soapenv:Body>
    <ws:login>
      <usuario>${escapeXml(username)}</usuario>
      <contrasenia>${escapeXml(password)}</contrasenia>
    </ws:login>
  </soapenv:Body>
</soapenv:Envelope>"""

        val responseXml = post(envelope)
        return parseToken(responseXml)
    }

    private fun post(body: String): String {
        val url = URL(endpoint)
        val conn = (url.openConnection() as HttpURLConnection).apply {
            requestMethod = "POST"
            connectTimeout = 10_000
            readTimeout = 30_000
            doOutput = true
            setRequestProperty("Content-Type", "text/xml; charset=utf-8")
            setRequestProperty("SOAPAction", "\"\"")
        }
        try {
            TlsInseguro.aplicarA(conn, url.host)
            conn.outputStream.use { it.write(body.toByteArray(Charsets.UTF_8)) }
            // Los SOAP Fault llegan con HTTP 500 pero con cuerpo XML útil.
            val stream = if (conn.responseCode in 200..299) conn.inputStream else conn.errorStream
            val text = stream?.bufferedReader(Charsets.UTF_8)?.use { it.readText() }.orEmpty()
            if (text.isBlank()) throw SoapException("Respuesta vacía (HTTP ${conn.responseCode})")
            return text
        } catch (e: SoapException) {
            throw e
        } catch (e: Exception) {
            android.util.Log.e("SoapClient", "Fallo SOAP hacia $endpoint", e)
            throw SoapException("No se pudo conectar con el servidor", e)
        } finally {
            conn.disconnect()
        }
    }

    /** Extrae <token> o lanza SoapException si hay <faultstring>. */
    private fun parseToken(xml: String): String {
        val parser = XmlPullParserFactory.newInstance().newPullParser()
        parser.setInput(StringReader(xml))
        var token: String? = null
        var fault: String? = null
        var event = parser.eventType
        while (event != XmlPullParser.END_DOCUMENT) {
            if (event == XmlPullParser.START_TAG) {
                when (parser.name) {
                    "return" -> token = parser.nextText().trim()
                    "faultstring" -> fault = parser.nextText().trim()
                }
            }
            event = parser.next()
        }
        fault?.let { throw SoapException(it) }
        return token?.takeIf { it.isNotEmpty() }
            ?: throw SoapException("La respuesta no contiene un token")
    }

    private fun escapeXml(s: String) = s
        .replace("&", "&amp;")
        .replace("<", "&lt;")
        .replace(">", "&gt;")
        .replace("\"", "&quot;")
        .replace("'", "&apos;")
}
