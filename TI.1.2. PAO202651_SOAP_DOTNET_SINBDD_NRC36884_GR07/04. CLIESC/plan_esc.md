# PLAN B — Cliente de Escritorio (WPF)
## B.1 Propósito
Construir una aplicación de escritorio WPF en .NET que consuma el mismo servicio SOAP, con:

Pantalla de login con logo en la parte superior, usuario y contraseña, y validación de campos no vacíos en el cliente.

Pantalla de conversión con 3 ComboBox (Tipo de conversión + Unidad origen + Unidad destino).

Los ComboBox de unidades se recargan dinámicamente según el tipo de conversión elegido.

Bloqueo de conversión a la misma unidad: si el usuario selecciona la misma unidad, se muestra un mensaje de advertencia y se impide la conversión.

Mensajes de error mostrados en un panel pequeño en pantalla (banner/toast) con el texto amigable devuelto por el servidor.

## B.2 Stack Tecnológico
Elemento	Descripción
Runtime	.NET 8 (WPF App, Windows)
Patrón	MVVM ligero (ViewModel + INotifyPropertyChanged + ICommand)
Cliente SOAP	System.ServiceModel.Http (WCF Client)
Binding UI	ObservableCollection<T> para ComboBox
Mensajería	Banner superior o Snackbar casero (Border + TextBlock + temporizador)
B.3 Estructura del Proyecto
text
ClienteEscritorioConversion/
├── App.xaml / App.xaml.cs
├── MainWindow.xaml / MainWindow.xaml.cs        # Contenedor con navegación Login <-> Conversión
├── Vistas/
│   ├── VistaInicioSesion.xaml / .xaml.cs
│   └── VistaConversion.xaml / .xaml.cs
├── VistasModelo/
│   ├── ModeloVistaInicioSesion.cs
│   └── ModeloVistaConversion.cs
├── Comandos/
│   └── ComandoRelay.cs
├── Cliente/
│   └── ClienteSoapConversion.cs                # Wrapper del proxy SOAP
├── Estado/
│   └── SesionActual.cs                         # Token en memoria
├── Componentes/
│   └── ControlMensaje.xaml / .xaml.cs          # Banner pequeño para mensajes amigables
├── Recursos/
│   ├── logo.png
│   └── Estilos.xaml
├── Configuracion/
│   └── ConfiguracionCliente.cs
├── appsettings.json
└── ClienteEscritorioConversion.csproj
## B.4 Especificación Funcional
### B.4.1 Vista de Inicio de Sesión (VistaInicioSesion)
Layout:

text
┌─────────────────────────────────────────┐
│              [ LOGO ]                   │
│         Conversión de Unidades          │
│  ─────────────────────────────────────  │
│  Usuario:      [______________]         │
│  Contraseña:   [**************]         │
│  ─────────────────────────────────────  │
│  [        Iniciar Sesión        ]       │
│                                         │
│  (mensaje de error del servidor aquí)   │
└─────────────────────────────────────────┘
Especificación:

Logo: Image en la parte superior con Source="/Recursos/logo.png", centrado, Width="120".

Título: TextBlock con "Conversión de Unidades".

Usuario: TextBox.

Contraseña: PasswordBox (nunca expone el valor en texto plano).

Botón Iniciar Sesión: Button con Command="{Binding ComandoIniciarSesion}".

Validación de campos no vacíos (en el cliente, antes de llamar al servidor):

Si NombreUsuario está vacío → mostrar banner "El usuario no puede estar vacío".

Si Contrasena está vacía → mostrar banner "La contraseña no puede estar vacía".

ComandoIniciarSesion con CanExecute que se reevalúa al cambiar los campos.

Llamada al servidor: IniciarSesionAsync(usuario, contrasena).

Si Exito = true → guardar token en SesionActual, navegar a VistaConversion.

Si Exito = false → mostrar RespuestaInicioSesion.Mensaje en el banner (rojo).

Si hay excepción de red → mostrar mensaje amigable local "No se pudo conectar con el servidor".

ModeloVistaInicioSesion:

csharp
public class ModeloVistaInicioSesion : INotifyPropertyChanged
{
    public string NombreUsuario { get; set; }
    public string Contrasena { get; set; }              // se enlaza al PasswordBox por code-behind
    public string MensajeError { get; set; }
    public bool MensajeVisible { get; set; }
    public ICommand ComandoIniciarSesion { get; }
    public event Action? InicioSesionExitoso;
}
### B.4.2 Vista de Conversión (VistaConversion)
Layout:

text
┌─────────────────────────────────────────┐
│  Conversión de Unidades    [Cerrar Sesión] │
│  ─────────────────────────────────────  │
│  Tipo:      [ Temperatura ▼ ]           │
│  De:        [ Celsius    ▼ ]            │
│  A:         [ Kelvin     ▼ ]            │
│  Valor:     [ 0.0        ]              │
│  ─────────────────────────────────────  │
│  [        Convertir           ]         │
│                                         │
│  Resultado: 273.1500                    │
│                                         │
│  (banner de mensajes del servidor aquí) │
└─────────────────────────────────────────┘
Especificación:

ComboBox "Tipo": 3 opciones → Temperatura, Longitud, Masa. Bind a CategoriasDisponibles.

ComboBox "De" y ComboBox "A": se recargan cuando cambia el Tipo.

Temperatura → UnidadTemperatura (Celsius, Fahrenheit, Kelvin, Rankine).

Longitud → UnidadLongitud (Metro, Kilometro, Centimetro, Milimetro, Pulgada, Pie, Yarda, Milla).

Masa → UnidadMasa (Kilogramo, Gramo, Miligramo, Libra, Onza, Tonelada).

ComboBox Valor: TextBox (numérico). Acepta negativos y separador decimal (. o ,).

Regla de misma unidad:

Al cambiar De o A, el ModeloVistaConversion reevalúa EsMismaUnidad.

Si De == A:

El botón Convertir se deshabilita (CanExecute = false).

El banner muestra el mensaje "No es posible convertir a la misma unidad" en amarillo/ámbar.

Si el usuario intenta hacer clic estando habilitado por algún error de estado, el comando en su Execute revalida y aborta mostrando el mismo banner.

Llamada al servidor:

ConvertirAsync(token, categoria, unidadOrigen, unidadDestino, valor).

Si Exito = true → mostrar ValorConvertido con 4 decimales en verde en el campo Resultado.

Si Exito = false → mostrar RespuestaConversion.Mensaje del servidor en el banner (rojo).

Botón Cerrar Sesión: limpia SesionActual y navega de vuelta a VistaInicioSesion.

ModeloVistaConversion (resumen):

csharp
public class ModeloVistaConversion : INotifyPropertyChanged
{
    public ObservableCollection<CategoriaUnidad> CategoriasDisponibles { get; }
    public CategoriaUnidad CategoriaSeleccionada { get; set; }   // recarga las unidades
    public ObservableCollection<object> UnidadesDisponibles { get; }  // se recarga con la categoría
    public object? UnidadOrigen { get; set; }
    public object? UnidadDestino { get; set; }
    public string ValorTexto { get; set; }
    public string ResultadoTexto { get; set; }
    public string MensajeBanner { get; set; }
    public bool BannerVisible { get; set; }
    public bool EsMismaUnidad => Equals(UnidadOrigen, UnidadDestino);
    public ICommand ComandoConvertir { get; }
    public ICommand ComandoCerrarSesion { get; }
}
### B.4.3 Componente: ControlMensaje (banner pequeño)
Contrato:

csharp
public partial class ControlMensaje : UserControl
{
    public string Texto { get; set; }
    public TipoMensaje Tipo { get; set; }   // Error | Advertencia | Exito
    public void Mostrar(string texto, TipoMensaje tipo, TimeSpan? duracion = null);
    public void Ocultar();
}
Comportamiento:

Es un Border con CornerRadius="6", Padding="12,8" y TextBlock interno.

Colores según Tipo:

Error → fondo #FFEBEE, texto #C62828, borde #C62828.

Advertencia → fondo #FFF8E1, texto #EF6C00, borde #EF6C00.

Exito → fondo #E8F5E9, texto #2E7D32, borde #2E7D32.

Si se proporciona duracion, arranca un DispatcherTimer que oculta el banner al expirar.

Se coloca en la parte inferior o superior del panel principal según la vista.

Regla clave: cuando el mensaje proviene del servidor (RespuestaInicioSesion.Mensaje o RespuestaConversion.Mensaje), se muestra tal cual, sin reescribirlo.

### B.4.4 Componente: ClienteSoapConversion
Idéntico al del Plan A:

csharp
public class ClienteSoapConversion
{
    public Task<RespuestaInicioSesion> IniciarSesionAsync(string usuario, string contrasena);
    public Task<RespuestaConversion> ConvertirAsync(string token, CategoriaUnidad categoria,
                                                     string unidadOrigen, string unidadDestino,
                                                     double valor);
}
Captura excepciones de red y las traduce a mensajes locales amigables, pero no altera los mensajes de negocio del servidor.

## B.5 Plan de Desarrollo por Fases (Cliente Escritorio)
Fase B1: Inicialización
Crear proyecto dotnet new wpf -n ClienteEscritorioConversion.

Agregar paquetes System.ServiceModel.Http y System.ServiceModel.Primitives.

Generar proxy desde WSDL con dotnet-svcutil.

Configurar appsettings.json con la URL del endpoint.

Agregar logo.png en Recursos/ y estilos base en Recursos/Estilos.xaml.

Aceptación: la app arranca en blanco; el logo se muestra correctamente.

Fase B2: Infraestructura MVVM y Mensajería
Implementar ComandoRelay.

Implementar ControlMensaje (banner con tipo y auto-ocultado).

Implementar SesionActual (misma que Plan A).

Implementar ClienteSoapConversion.

Aceptación: en una vista de prueba, el banner muestra un mensaje de cada tipo con el color correcto.

Fase B3: Vista y ModeloVista de Inicio de Sesión
Maquetar VistaInicioSesion.xaml con logo, TextBox, PasswordBox, botón y ControlMensaje.

Implementar ModeloVistaInicioSesion con validación de campos no vacíos y CanExecute.

Conectar con ClienteSoapConversion.IniciarSesionAsync.

En éxito: guardar token y navegar a VistaConversion.

En fallo: mostrar Mensaje del servidor en el banner.

Aceptación:

Usuario/contraseña vacíos → banner local "El usuario no puede estar vacío".

Credenciales inválidas → banner con el mensaje del servidor.

Credenciales válidas → navegación a la vista de conversión.

Fase B4: Vista y ModeloVista de Conversión
Maquetar 3 ComboBox + TextBox valor + botón Convertir + resultado + ControlMensaje.

Implementar recarga dinámica de unidades según categoría.

Implementar la regla EsMismaUnidad (deshabilita botón + banner ámbar).

Implementar el flujo de conversión y la muestra de mensajes del servidor.

Aceptación:

Cambiar Tipo entre Temperatura/Longitud/Masa recarga los ComboBox correspondientes.

Seleccionar De == A deshabilita Convertir y muestra banner ámbar.

Conversión válida muestra el resultado en verde.

Conversión inválida (ej. -10 °C → °F) muestra el mensaje exacto del servidor en el banner rojo.

Fase B5: Cierre de sesión y pruebas integrales
Implementar "Cerrar Sesión" (limpia token, navega a login).

Probar todos los casos de la fase 9 del plan del servidor desde la UI.

Verificar que los mensajes del servidor aparecen sin modificación.

Aceptación: todos los casos pasan en la UI de escritorio.

Comparativa resumida
Aspecto	Plan A (Consola)	Plan B (Escritorio WPF)
Selector de tipo/unidades	Menú con flecha movible (↑/↓ + Enter)	3 ComboBox
Login	Usuario visible, contraseña enmascarada con *	Logo + TextBox + PasswordBox
Validación de campos	Reintento en bucle	Banner con mensaje local antes de llamar al servidor
Token	SesionActual en memoria	SesionActual en memoria
Misma unidad	Comprobación local previa al envío	Bloqueo de botón + banner ámbar
Mensajes del servidor	Impresos en consola en rojo, tal cual	Mostrados en banner rojo, tal cual
Valores negativos	EntradaNumerica acepta -	TextBox acepta -
Estilo visual	Colores ANSI de consola + flecha ►	Estilos XAML, banner con colores por tipo
Ambos planes comparten el mismo contrato con el servidor (IniciarSesion / Convertir), la misma política de token en memoria y la misma filosofía de propagar literalmente los mensajes amigables del servidor hacia la UI.
