import java.util.Properties

val environment = Properties().apply {
    val file = rootProject.file(".development.env")
    if (file.exists()) file.inputStream().use { load(it) }
}
val eurekaUrl = environment.getProperty("EUREKABANK_URL", "http://10.0.2.2:9080")

plugins { id("com.android.application"); id("org.jetbrains.kotlin.android") }

android { namespace="ec.edu.monster"; compileSdk=35
 defaultConfig { applicationId="ec.edu.monster.eurekabank"; minSdk=24; targetSdk=35; versionCode=1; versionName="1.0" }
 buildFeatures { viewBinding=true; buildConfig=true }
 defaultConfig { buildConfigField("String", "EUREKABANK_URL", "\"$eurekaUrl\"") }
 buildTypes { release { isMinifyEnabled=false } }
 compileOptions { sourceCompatibility=JavaVersion.VERSION_17; targetCompatibility=JavaVersion.VERSION_17 }
 kotlinOptions { jvmTarget="17" }
}
dependencies { implementation("androidx.core:core-ktx:1.15.0"); implementation("androidx.appcompat:appcompat:1.7.0"); implementation("com.google.android.material:material:1.12.0"); implementation("androidx.constraintlayout:constraintlayout:2.2.0") }
