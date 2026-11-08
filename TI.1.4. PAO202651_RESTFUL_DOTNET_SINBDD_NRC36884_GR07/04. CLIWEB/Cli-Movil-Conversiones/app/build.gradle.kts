import java.util.Properties

plugins {
    id("com.android.application")
    id("org.jetbrains.kotlin.android")
}

val archivoEntorno = rootProject.file(".development.env")

fun leerEntorno(archivo: File): Properties {
    require(archivo.isFile) { "No se encontro el archivo de entorno: ${archivo.absolutePath}" }
    return Properties().apply { archivo.inputStream().use { load(it) } }
}

val entornoServidor = leerEntorno(archivoEntorno)

fun valorEntorno(clave: String): String = requireNotNull(entornoServidor.getProperty(clave)) {
    "Falta la clave $clave en ${archivoEntorno.absolutePath}"
}.trim()

fun urlBase(clave: String): String = valorEntorno(clave).let { if (it.endsWith("/")) it else "$it/" }

fun indicadorEntorno(clave: String): String = when (valorEntorno(clave).lowercase()) {
    "true" -> "true"
    "false" -> "false"
    else -> error("La clave $clave debe valer true o false en ${archivoEntorno.absolutePath}")
}

android {
    namespace = "ec.edu.monster"
    compileSdk = 35

    defaultConfig {
        applicationId = "ec.edu.monster"
        minSdk = 24
        targetSdk = 35
        versionCode = 1
        versionName = "1.0"

        buildConfigField("String", "URL_BASE_EMULADOR", "\"${urlBase("URL_BASE_EMULADOR_ANDROID")}\"")
        buildConfigField("String", "URL_BASE_LOCAL", "\"${urlBase("URL_BASE")}\"")
        buildConfigField("String", "RUTA_AUTENTICACION", "\"${valorEntorno("ENDPOINT_AUTENTICACION")}\"")
        buildConfigField("String", "RUTA_LONGITUD", "\"${valorEntorno("ENDPOINT_LONGITUD")}\"")
        buildConfigField("String", "RUTA_TEMPERATURA", "\"${valorEntorno("ENDPOINT_TEMPERATURA")}\"")
        buildConfigField("String", "RUTA_MASA", "\"${valorEntorno("ENDPOINT_MASA")}\"")
        buildConfigField("String", "CABECERA_AUTORIZACION", "\"${valorEntorno("CABECERA_AUTORIZACION")}\"")
        buildConfigField("String", "TIPO_TOKEN", "\"${valorEntorno("TIPO_TOKEN")}\"")
        buildConfigField("boolean", "MASA_DISPONIBLE", indicadorEntorno("MASA_DISPONIBLE"))
    }

    buildTypes {
        release {
            isMinifyEnabled = false
            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"),
                "proguard-rules.pro"
            )
        }
    }

    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }

    kotlinOptions {
        jvmTarget = "17"
    }

    buildFeatures {
        viewBinding = true
        buildConfig = true
    }

    lint {
        abortOnError = false
    }
}

dependencies {
    implementation("androidx.core:core-ktx:1.15.0")
    implementation("androidx.appcompat:appcompat:1.7.0")
    implementation("com.google.android.material:material:1.12.0")
    implementation("androidx.fragment:fragment-ktx:1.8.5")
    implementation("androidx.lifecycle:lifecycle-runtime-ktx:2.8.7")
    implementation("androidx.viewpager2:viewpager2:1.1.0")
    implementation("androidx.security:security-crypto:1.1.0-alpha06")
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-android:1.9.0")
    implementation("com.squareup.retrofit2:retrofit:2.11.0")
    implementation("com.squareup.retrofit2:converter-gson:2.11.0")
    implementation("com.squareup.okhttp3:logging-interceptor:4.12.0")
    implementation("com.google.code.gson:gson:2.11.0")
}
