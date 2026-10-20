# Planes de Cliente para el Servicio SOAP de Conversión de Unidades

A continuación se presentan **dos planes independientes** que consumen el servidor SOAP ya especificado (`ServicioSoapConversionUnidades`), reutilizando **los mensajes de error amigables que devuelve el servidor** en ambos casos.

---

# PLAN A — Cliente de Consola (.NET)

## A.1 Propósito

Construir una aplicación de consola en .NET que consuma el servicio SOAP existente, permitiendo al usuario:

1. Iniciar sesión (usuario visible, contraseña enmascarada).
2. Guardar el token devuelto para usarlo en cada petición de conversión.
3. Elegir el tipo de conversión (Temperatura / Longitud / Masa) mediante un **selector con flecha movible** (menú navegable con teclas de dirección).
4. Ingresar el valor numérico **incluyendo negativos** (usando el teclado).
5. Elegir unidades de origen y destino con el mismo selector de flecha movible.
6. Mostrar los mensajes de error amigables **tal cual los devuelve el servidor**.

## A.2 Stack Tecnológico

| Elemento | Descripción |
|---|---|
| Runtime | .NET 8 (Console App) |
| Cliente SOAP | `System.ServiceModel.Http` (WCF Client) o referencia WSDL generada con `dotnet-svcutil` |
| UI de consola | `Console` + `ConsoleKey` para navegación con flechas |
| Colores | `Console.ForegroundColor` / `Console.BackgroundColor` |
| Enmascaramiento de contraseña | Lectura con `Console.ReadKey(intercept: true)` |
| Manejo de errores | Mostrar `RespuestaConversion.Mensaje` / `RespuestaInicioSesion.Mensaje` del servidor |

## A.3 Estructura del Proyecto
ClienteConsolaConversion/
├── Program.cs # Punto de entrada y bucle principal
├── Cliente/
│ └── ClienteSoapConversion.cs # Wrapper del cliente WCF/SOAP
├── Interfaz/
│ ├── MenuSeleccionMultiple.cs # Selector con flecha movible
│ ├── EntradaNumerica.cs # Lectura de números (incluye negativos)
│ ├── EntradaContrasena.cs # Lectura enmascarada
│ └── Presentacion.cs # Colores, títulos, mensajes
├── Estado/
│ └── SesionActual.cs # Token en memoria del cliente
├── Configuracion/
│ └── ConfiguracionCliente.cs # URL del endpoint
├── appsettings.json
└── ClienteConsolaConversion.csproj

text

## A.4 Especificación Funcional

### A.4.1 Flujo general
[Inicio]
→ Mostrar título con logo ASCII
→ Pantalla de Inicio de Sesión
→ Pedir NombreUsuario (visible)
→ Pedir Contrasena (enmascarada con '*')
→ Llamar IniciarSesion
→ Si Exito: guardar token en SesionActual
→ Si falla: mostrar Mensaje del servidor en rojo y volver a pedir
→ Menú principal (bucle)
→ Opción: Convertir
→ Opción: Cerrar sesión
→ Opción: Salir
→ En "Convertir":
→ Selector de categoría (Temperatura / Longitud / Masa) con flecha
→ Si categoría = Temperatura → selector de unidades de Temperatura
(Celsius / Fahrenheit / Kelvin / Rankine)
→ Si categoría = Longitud → selector de UnidadLongitud
→ Si categoría = Masa → selector de UnidadMasa
→ Selector de unidad origen (flecha)
→ Selector de unidad destino (flecha)
→ Si origen == destino → mostrar mensaje amigable local y volver a elegir
→ Entrada de valor numérico (permite '-' para negativos)
→ Llamar Convertir con Token + Categoria + UnidadOrigen + UnidadDestino + Valor
→ Mostrar resultado o Mensaje del servidor

text

### A.4.2 Componente: `MenuSeleccionMultiple` (selector con flecha movible)

**Contrato de la clase**:

```csharp
public class MenuSeleccionMultiple
{
    public MenuSeleccionMultiple(string titulo, IReadOnlyList<string> opciones);
    public int Mostrar(); // devuelve el índice seleccionado
}
Comportamiento:

Dibuja el título con color (ConsoleColor.Cyan).

Dibuja las opciones en una columna, con ► como flecha del ítem activo.

El ítem activo se resalta con Console.BackgroundColor = ConsoleColor.DarkCyan y ForegroundColor = ConsoleColor.White.

Teclas soportadas:

↑ / ↓ → mover la flecha.

Enter → confirmar y devolver el índice.

Esc → devolver -1 para cancelar (útil para volver atrás).

Redibuja solo la región del menú (usando Console.SetCursorPosition) para evitar parpadeo.

Ejemplo visual:

text
  Categoría de conversión
  ─────────────────────────
  ► Temperatura
    Longitud
    Masa
A.4.3 Componente: EntradaNumerica
Contrato:

csharp
public static class EntradaNumerica
{
    public static bool Leer(string etiqueta, out double valor);
}
Comportamiento:

Muestra la etiqueta en color (ConsoleColor.Yellow).

Lee línea completa con Console.ReadLine().

Acepta el carácter - al inicio para negativos (ej: -10, -273.15).

Acepta separador decimal . y , (normaliza a . antes de double.TryParse con CultureInfo.InvariantCulture).

Si el parseo falla, muestra "Valor inválido, intente de nuevo" en rojo y vuelve a pedir.

Rechaza cadenas vacías.

A.4.4 Componente: EntradaContrasena
Contrato:

csharp
public static class EntradaContrasena
{
    public static string Leer(string etiqueta);
}
Comportamiento:

Muestra la etiqueta en color (ConsoleColor.Yellow).

Lee carácter por carácter con Console.ReadKey(intercept: true).

Muestra * por cada carácter ingresado.

Soporta Backspace para borrar.

Enter finaliza la lectura.

El valor real nunca se imprime en pantalla.

A.4.5 Componente: ClienteSoapConversion
Contrato:

csharp
public class ClienteSoapConversion
{
    public ClienteSoapConversion(string urlEndpoint);
    public Task<RespuestaInicioSesion> IniciarSesionAsync(string usuario, string contrasena);
    public Task<RespuestaConversion> ConvertirAsync(string token, CategoriaUnidad categoria,
                                                     string unidadOrigen, string unidadDestino,
                                                     double valor);
}
Comportamiento:

Envuelve al cliente WCF generado por dotnet-svcutil.

Captura excepciones de red (CommunicationException, TimeoutException) y las traduce a un RespuestaConversion con Exito = false y un Mensaje amigable (ej: "No se pudo conectar con el servidor").

No transforma los mensajes de negocio del servidor: los propaga tal cual.

A.4.6 Componente: SesionActual
csharp
public class SesionActual
{
    public string? Token { get; private set; }
    public string? NombreUsuario { get; private set; }
    public bool Autenticado => !string.IsNullOrEmpty(Token);
    public void Iniciar(string nombreUsuario, string token);
    public void Cerrar();
}
El token se guarda en memoria del cliente (no en disco).

Se pasa en cada llamada a Convertir.

A.4.7 Componente: Presentacion
csharp
public static class Presentacion
{
    public static void MostrarTitulo(string texto);
    public static void MostrarError(string mensajeDelServidor);   // rojo
    public static void MostrarExito(string mensaje);              // verde
    public static void MostrarInfo(string mensaje);               // cian
    public static void EsperarTecla();
}
MostrarError imprime exactamente el Mensaje recibido del servidor, con prefijo [ERROR] en rojo.

MostrarExito imprime el resultado numérico con 4 decimales en verde.

A.5 Plan de Desarrollo por Fases (Cliente Consola)
Fase A1: Inicialización
Crear proyecto dotnet new console -n ClienteConsolaConversion.

Agregar paquete System.ServiceModel.Http y System.ServiceModel.Primitives.

Generar cliente desde WSDL con dotnet-svcutil http://localhost:5000/?wsdl.

Configurar appsettings.json con la URL del endpoint.

Aceptación: el proyecto compila y referencia el proxy SOAP.

Fase A2: Utilidades de UI
Implementar MenuSeleccionMultiple con navegación por flechas y colores.

Implementar EntradaNumerica con soporte de negativos.

Implementar EntradaContrasena enmascarada.

Implementar Presentacion (título, error, éxito, info).

Aceptación: un Main de prueba muestra un menú navegable, lee -12.5 y lee una contraseña sin mostrarla.

Fase A3: Login
Implementar ClienteSoapConversion.IniciarSesionAsync.

Implementar la pantalla de login: usuario visible, contraseña enmascarada.

Guardar token en SesionActual si Exito = true.

Mostrar Mensaje del servidor en rojo si falla.

Aceptación: login exitoso guarda token; login fallido muestra el mensaje del servidor.

Fase A4: Menú principal y flujo de conversión
Menú principal con opciones: Convertir / Cerrar sesión / Salir.

Flujo: selector de categoría → selector de origen → selector de destino → si iguales, mensaje local → entrada de valor → llamada al servidor.

Mostrar resultado o Mensaje del servidor.

Aceptación: conversión 0 °C → K muestra 273.1500; -10 °C → °F muestra el mensaje del servidor.

Fase A5: Pruebas manuales
Casos de la tabla de la fase 9 del plan del servidor, verificados desde consola.

Verificar que los mensajes de error mostrados son idénticos a los del servidor.

Aceptación: todos los casos pasan.