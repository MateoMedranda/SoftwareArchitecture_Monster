Plan de Especificaciones Técnicas (Cliente Móvil Kotlin - MVC)
1. Arquitectura y Estructura del Proyecto
Patrón de Diseño: MVC (Modelo-Vista-Controlador).

Modelo (modelos/): Clases de datos (data class), entidades de red, peticiones, respuestas y la lógica de negocio para comunicación con el servidor REST.

Vista (vistas/ / res/layout/): Archivos XML de interfaz de usuario existentes (se conservan y reutilizan) integrados en las actividades mediante VinculacionVistas (ViewBinding).

Controlador (controladores/): Actividades (Activity) y Fragmentos (Fragment) que manejan la captura de eventos de la interfaz de usuario, llaman a la capa del modelo de red y actualizan las vistas directamente.

Stack Tecnológico:

Lenguaje: Kotlin (migrando clases .java a .kt).

Cliente HTTP: Retrofit 2 + OkHttp con interceptores para añadir cabeceras.

Serialización: Gson para mapeo de JSON.

Almacenamiento Local (Token): SharedPreferences encriptadas mediante EncryptedSharedPreferences para persitir la sesión del usuario.

Regla de Nomenclatura: Todos los archivos, clases, interfaces, variables y funciones deben estar estrictamente redactados en español.

2. Autenticación y Gestión de Sesión
Controlador de Inicio de Sesión (ActividadInicioSesion.kt):

Captura los campos de la interfaz (VistaInicioSesion / XML).

Envía las credenciales al modelo de red (ServicioAutenticacion).

Recibe la respuesta con el token JWT.

Interceptor de Autorización (InterceptorAutenticacion.kt):

Configura OkHttpClient para interceptar cada petición REST enviada al servidor e incluir la cabecera:

Plaintext
Authorization: Bearer <token_guardado>
Gestor de Preferencias (GestorSesion.kt):

Métodos para guardar, recuperar y eliminar el token de autenticación.

3. Definición de Modelos y Mapeo REST (modelos/)
Dado que el servidor reside en otra carpeta del proyecto, el agente revisará los controladores backend para replicar exactamente las estructuras JSON en data classes de Kotlin.

Modelos de Autenticación (modelos/autenticacion/):

PeticionInicioSesion.kt (usuario, clave)

RespuestaInicioSesion.kt (token, mensaje, exito)

Modelos de Conversión (modelos/conversion/):

PeticionConversion.kt (valorOrigen, unidadOrigen, unidadDestino)

RespuestaConversion.kt (valorOrigen, valorDestino, unidadOrigen, unidadDestino, exito, mensaje)

Definición de Endpoints (red/ClienteRest.kt):

Longitud: Endpoint activo en el servidor (POST /api/conversion/longitud).

Temperatura: Endpoint activo en el servidor (POST /api/conversion/temperatura).

Masa: Endpoint en desarrollo en el servidor (POST /api/conversion/masa).

Manejo en cliente: El modelo está listo para consumir el endpoint, pero el controlador mostrará un mensaje descriptivo ("Servicio de conversión de masa en mantenimiento") o usará un simulador hasta que el backend esté finalizado.

4. Interfaz de Usuario y Selector de Conversión
Controlador Principal (ActividadPrincipal.kt):

Selector de Tipos (Spinner / TabLayout): Permite cambiar entre tres tipos de conversión: Longitud, Temperatura y Masa.

Lógica del Selector: Al seleccionar una opción:

Actualiza dinámicamente los listados (Spinners / adaptadores) de las unidades de origen y destino disponibles para esa categoría.

Limpia los campos de texto de entrada y resultado.

Proceso de Conversión:

El usuario ingresa un valor y hace clic en el botón de conversión.

El controlador valida la entrada y ejecuta la llamada asíncrona (usando Corrutinas de Kotlin) mediante la interfaz REST del modelo.

Al recibir la respuesta, el controlador actualiza la vista con el resultado o muestra un mensaje de error si la petición falla.

5. Estructura de Directorios del Proyecto Móvil
Plaintext
app/src/main/java/com/empresa/cliente/
├── controladores/
│   ├── ActividadInicioSesion.kt
│   └── ActividadPrincipal.kt
├── modelos/
│   ├── autenticacion/
│   │   ├── PeticionInicioSesion.kt
│   │   └── RespuestaInicioSesion.kt
│   ├── conversion/
│   │   ├── PeticionConversion.kt
│   │   └── RespuestaConversion.kt
│   └── datos/
│       └── GestorSesion.kt
├── red/
│   ├── InterceptorAutenticacion.kt
│   ├── ClienteRest.kt
│   └── ProveedorServicio.kt
└── utilidades/
    └── Constantes.kt
6. Instrucciones para el Agente Integrado
Paso 1 (Inspección del Servidor): Revisar los archivos JSON/Controladores REST situados en la carpeta del backend del proyecto para mapear correctamente los nombres de las propiedades en los modelos de Kotlin (PeticionConversion, RespuestaConversion, etc.).

Paso 2 (Migración MVC y XML): Conservar los archivos XML de la interfaz original (versión SOAP), habilitar viewBinding en Gradle y conectar los IDs de las vistas con los nuevos controladores en Kotlin.

Paso 3 (Implementación de Red): Configurar Retrofit con el InterceptorAutenticacion y la ruta base del servidor local/remoto.

Paso 4 (Verificación): Comprobar que los endpoints de longitud y temperatura funcionen correctamente enviando el Bearer token, y controlar la respuesta/falta del endpoint de masa adecuadamente.