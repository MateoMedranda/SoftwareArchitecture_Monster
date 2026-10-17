import java.util.Properties;

val envProps = Properties().apply {
    val file = rootProject.file(".development.env").takeIf { it.exists() } ?: rootProject.file("development.env")
    if (file.exists()) file.inputStream().use { load(it) }
}

fun env(key: String): String =
    envProps.getProperty(key) ?: error("Falta '$key' en el archivo .env")

plugins {
    alias(libs.plugins.android.application)
}

android {
    namespace = "com.marpa.practica"
    compileSdk {
        version = release(37)
    }

    defaultConfig {
        applicationId = "com.marpa.practica"
        minSdk = 24
        targetSdk = 37
        versionCode = 1
        versionName = "1.0"

        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner"

        buildConfigField("String", "SOAP_HOST", "\"${env("SOAP_HOST")}\"")
        buildConfigField("String", "SOAP_PORT", "\"${env("SOAP_PORT")}\"")
        buildConfigField("String", "SOAP_PROTOCOL", "\"${env("SOAP_PROTOCOL")}\"")
        buildConfigField("String", "SOAP_CONTEXT", "\"${env("SOAP_CONTEXT")}\"")
        buildConfigField("String", "SOAP_SERVICE", "\"${env("SOAP_SERVICE")}\"")
        buildConfigField("String", "SOAP_ENDPOINT", "\"${env("SOAP_ENDPOINT")}\"")
        buildConfigField("String", "SOAP_WSDL", "\"${env("SOAP_WSDL")}\"")
        buildConfigField("String", "SOAP_TARGET_NAMESPACE", "\"${env("SOAP_TARGET_NAMESPACE")}\"")
        buildConfigField("String", "SOAP_USER", "\"${env("SOAP_USER")}\"")
        buildConfigField("String", "SOAP_PASSWORD", "\"${env("SOAP_PASSWORD")}\"")
        buildConfigField("String", "SOAP_CONNECT_TIMEOUT", "\"${env("SOAP_CONNECT_TIMEOUT")}\"")
        buildConfigField("String", "SOAP_READ_TIMEOUT", "\"${env("SOAP_READ_TIMEOUT")}\"")
        buildConfigField("String", "SOAP_INSECURE_TLS", "\"${env("SOAP_INSECURE_TLS")}\"")
        buildConfigField("String", "SOAP_SECURITY_NAMESPACE", "\"${env("SOAP_SECURITY_NAMESPACE")}\"")
    }

    buildFeatures{
        viewBinding = true
        buildConfig = true
    }

    buildTypes {
        release {
            optimization {
                enable = true
                packageScope = setOf("androidx.**", "kotlin.**", "kotlinx.**")
            }
        }
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_11
        targetCompatibility = JavaVersion.VERSION_11
    }
}

dependencies {
    implementation(libs.androidx.activity.ktx)
    implementation(libs.androidx.appcompat)
    implementation(libs.androidx.constraintlayout)
    implementation(libs.androidx.core.ktx)
    implementation(libs.material)
    testImplementation(libs.junit)
    androidTestImplementation(libs.androidx.espresso.core)
    androidTestImplementation(libs.androidx.junit)
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-android:1.11.0")
    implementation("androidx.lifecycle:lifecycle-runtime-ktx:2.11.0")
    implementation("androidx.viewpager2:viewpager2:1.1.0")
    implementation("androidx.fragment:fragment-ktx:1.8.5")
}