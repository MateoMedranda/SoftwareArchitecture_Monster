# CLIMOV Facturación

Aplicación Android nativa para el API de Facturación de Celulares Monster.
Está construida con .NET MAUI Blazor Hybrid: conserva las pantallas Razor,
servicios, controladores y el diseño móvil, pero se ejecuta dentro de una app
Android y se distribuye como APK. Consume únicamente los endpoints REST del API
de Facturación; no accede a la base de datos ni llama directamente a BanQuito.

## Requisitos

- .NET SDK 10 con la carga de trabajo `maui-android`
- Android SDK y emulador o dispositivo Android
- API de Facturación en ejecución en el equipo anfitrión (puerto `5226`)
- BanQuito en ejecución (puerto `5173`) para las compras
  a crédito, ya que el API de Facturación lo consulta internamente

## Compilar y ejecutar

Desde la carpeta del cliente, restaura dependencias y compila el APK:

```powershell
dotnet workload install maui-android
dotnet build CLIMOV_Facturacion.csproj -f net10.0-android -c Debug
```

El APK de depuración queda en `bin\Debug\net10.0-android\`. Para crear un APK
de distribución sin firma:

```powershell
dotnet publish CLIMOV_Facturacion.csproj -f net10.0-android -c Release -p:AndroidPackageFormat=apk
```

Para instalarlo con un dispositivo conectado por ADB, ejecute
`adb install -r bin\Release\net10.0-android\publish\*.apk`.

## Configuración

En el emulador Android, el API usa por defecto
`http://10.0.2.2:5226/` (`10.0.2.2` representa el equipo anfitrión). Para usar
un teléfono físico, toque el botón discreto **Servidor** en la cabecera e
ingrese la dirección LAN del equipo que ejecuta el API, por ejemplo
`http://192.168.1.20:5226/`. La selección queda guardada en el dispositivo y
se aplica a las siguientes solicitudes sin reinstalar la aplicación. El teléfono
y el equipo deben estar en la misma red y el API debe aceptar conexiones desde
esa red.

Android permite HTTP sin TLS para esta configuración de desarrollo; para
publicar la aplicación se recomienda configurar HTTPS.

## Arquitectura

```text
Config/          URL base del API para emulador o dispositivo
Modelos/DTO      Contratos de cliente, producto, factura y amortización
Modelos/DAO      Acceso HTTP a los endpoints del API de Facturación
Servicios/       Reglas del cliente: filtros, carrito, totales y validaciones
Controladores/   Estado y acciones de cada pantalla
Vistas/          Layout móvil, páginas y modales reutilizables
Platforms/       Contenedor y permisos nativos de Android
wwwroot/         Recursos de Blazor Hybrid, estilos e imágenes
```

La interfaz usa Blazor Hybrid embebido en MAUI. No necesita navegador, servidor
Blazor ni conexión SignalR: Razor se ejecuta en la aplicación Android y las
llamadas REST se realizan desde el dispositivo.

## Pantallas

- **Inicio:** accesos rápidos a Clientes, Catálogo, Compra, Facturas y Créditos.
- **Clientes:** listado en tarjetas, búsqueda, alta, edición y baja.
- **Catálogo:** productos en tarjetas, búsqueda, alta, edición y baja. El precio
  admite coma o punto como separador decimal y debe ser mayor que cero.
- **Registrar compra:** búsqueda de cliente por cédula, carrito, pago en efectivo
  o a crédito, confirmación y comprobante.
- **Facturas:** listado, búsqueda por cédula o número, detalle y amortización.
- **Créditos:** selector de créditos en tarjetas y tabla de amortización en
  tarjetas apiladas con el estado de pago de cada cuota.

### Créditos

El selector se arma con las facturas a crédito (`tipoPago` `CREDITO` y
`numCredito` no nulo). También permite buscar por cédula: se consulta el nombre
en `GET /api/clientes/por-cedula/{cedula}` y el crédito vigente en
`GET /api/amortizaciones/por-cedula/{cedula}`. Los créditos encontrados solo en
BanQuito se agregan al selector con el origen `BanQuito` y se evitan duplicados
por número de crédito.

Al elegir un crédito con factura de la tienda se consulta
`GET /api/amortizaciones/{numFactura}`. La pantalla es de solo lectura: muestra
las cuotas pagadas y pendientes que devuelve el API, pero no registra pagos
porque el API de Facturación no expone esa operación.

### Flujo de compra

1. Se busca al cliente por cédula con `GET /api/clientes/por-cedula/{cedula}`.
2. Se agregan productos al carrito; el subtotal se calcula en el cliente.
3. **Efectivo:** se muestra una vista previa con el 42 % de descuento. El API
   recalcula el total definitivo al registrar la factura.
4. **Crédito:** se solicitan entre 3 y 18 cuotas. El API valida la elegibilidad
   del cliente y el monto máximo en BanQuito.
5. Al confirmar se envía `POST /api/facturas` con `tipoPago` `EFECTIVO` o
   `CREDITO` y los ítems del carrito.
6. Si la factura es a crédito, la amortización se consulta al API y se muestra
   en un modal a pantalla completa; nunca se calcula localmente.

Los mensajes de error del API (cliente no elegible, crédito activo, plazos
inválidos, etc.) se presentan tal como los devuelve el servidor.

## Endpoints consumidos

```text
GET    /api/clientes
GET    /api/clientes/{codigo}
GET    /api/clientes/por-cedula/{cedula}
POST   /api/clientes
PUT    /api/clientes/{codigo}
DELETE /api/clientes/{codigo}

GET    /api/productos
GET    /api/productos/{codigo}
POST   /api/productos
PUT    /api/productos/{codigo}
DELETE /api/productos/{codigo}

GET    /api/facturas
GET    /api/facturas?cedula={cedula}
GET    /api/facturas/{numero}
GET    /api/facturas/{numero}/items
POST   /api/facturas

GET    /api/amortizaciones/{numFactura}
GET    /api/amortizaciones/por-cedula/{cedula}
```

## Notas

- Al ser Blazor Hybrid, las llamadas al API salen del dispositivo Android; no
  se requiere configurar CORS en el API de Facturación.
- El descuento por efectivo mostrado antes de confirmar es solo una vista
  previa: el valor oficial es el que devuelve el API al crear la factura.
- No se modifican datos de BanQuito desde este cliente. Toda operación de
  crédito la resuelve el API de Facturación.
- La pantalla de Créditos es informativa: el estado de pago de cada cuota viene
  del API y no se puede modificar desde el cliente.
