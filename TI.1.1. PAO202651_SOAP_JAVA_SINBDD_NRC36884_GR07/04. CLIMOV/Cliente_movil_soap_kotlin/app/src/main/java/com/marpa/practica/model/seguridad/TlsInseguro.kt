package com.marpa.practica.model.seguridad

import java.net.HttpURLConnection
import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.net.ssl.HostnameVerifier
import javax.net.ssl.HttpsURLConnection
import javax.net.ssl.SSLContext
import javax.net.ssl.X509TrustManager

/** SOLO DESARROLLO: acepta cualquier certificado del servidor indicado. */
object TlsInseguro {

    private val confianzaTotal = object : X509TrustManager {
        override fun getAcceptedIssuers(): Array<X509Certificate> = emptyArray()
        override fun checkClientTrusted(chain: Array<X509Certificate>, authType: String) {}
        override fun checkServerTrusted(chain: Array<X509Certificate>, authType: String) {}
    }

    private val socketFactory by lazy {
        SSLContext.getInstance("TLS").apply {
            init(null, arrayOf(confianzaTotal), SecureRandom())
        }.socketFactory
    }

    /** Aplica el TLS inseguro solo a esta conexión, no a toda la app. */
    fun aplicarA(conn: HttpURLConnection, hostPermitido: String) {
        if (conn !is HttpsURLConnection) return
        conn.sslSocketFactory = socketFactory
        conn.hostnameVerifier = HostnameVerifier { host, _ -> host == hostPermitido }
    }
}