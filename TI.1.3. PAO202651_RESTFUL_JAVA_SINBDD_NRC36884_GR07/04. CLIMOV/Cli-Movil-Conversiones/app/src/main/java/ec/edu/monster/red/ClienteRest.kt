package ec.edu.monster.red

import ec.edu.monster.BuildConfig
import ec.edu.monster.modelos.datos.GestorSesion
import ec.edu.monster.utilidades.Constantes
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager

/** Cliente HTTP unico del proyecto: OkHttp + Retrofit con interceptor JWT. */
class ClienteRest(gestorSesion: GestorSesion) {

    private val constructorCliente = OkHttpClient.Builder()
        .addInterceptor(InterceptorAutenticacion(gestorSesion))
        .addInterceptor(HttpLoggingInterceptor().apply {
            level = HttpLoggingInterceptor.Level.BASIC
        })

    init {
        if (BuildConfig.DEBUG) {
            // Payara usa un certificado autofirmado con CN=localhost: en desarrollo
            // se acepta sin validar para poder usar 10.0.2.2 o la IP LAN.
            constructorCliente.sslSocketFactory(
                contextoInseguro().socketFactory,
                gestorConfianzaInseguro
            )
            constructorCliente.hostnameVerifier { _, _ -> true }
        }
    }

    private val clienteOkHttp = constructorCliente.build()

    private val instanciaRetrofit = Retrofit.Builder()
        .baseUrl(Constantes.URL_BASE)
        .client(clienteOkHttp)
        .addConverterFactory(GsonConverterFactory.create())
        .build()

    fun <T> crearServicio(clase: Class<T>): T = instanciaRetrofit.create(clase)

    private fun contextoInseguro(): SSLContext = SSLContext.getInstance("TLS").apply {
        init(null, arrayOf<TrustManager>(gestorConfianzaInseguro), SecureRandom())
    }

    private companion object {
        val gestorConfianzaInseguro = object : X509TrustManager {
            override fun checkClientTrusted(cadena: Array<X509Certificate>, tipo: String) {}

            override fun checkServerTrusted(cadena: Array<X509Certificate>, tipo: String) {}

            override fun getAcceptedIssuers(): Array<X509Certificate> = emptyArray()
        }
    }
}
