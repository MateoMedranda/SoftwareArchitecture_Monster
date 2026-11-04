# CLIWEB Facturación

Cliente web de escritorio para el API de Facturación de Celulares Monster.
Está construido como Blazor Web App con renderizado interactivo del lado del
servidor (Interactive Server) y consume únicamente los endpoints REST del API
de Facturación; no accede a la base de datos ni llama directamente a BanQuito.

## Requisitos

- .NET SDK 10
- API de Facturación en ejecución (por defecto `http://localhost:5226/`)
- BanQuito en ejecución (por defecto `http://localhost:5173/`) para las compras
  a crédito, ya que el API de Facturación lo consulta internamente

## Compilar y ejecutar

Desde la carpeta del cliente:

```powershell
dotnet run --project CLIWEB_Facturacion.csproj --launch-profile http
```

También se puede usar la solución del cliente desde la carpeta superior:

```powershell
dotnet build CLIWEB_Facturacion.slnx
dotnet run --project CLIWEB_Facturacion\CLIWEB_Facturacion.csproj --launch-profile http
```

La aplicación queda disponible en `http://localhost:5227/`.

## Configuración

El cliente lee un archivo `.env` en el directorio de contenido del proyecto. Si
no existe, usa los valores por defecto que apuntan al API local.

```text
API_BASE_URL="http://localhost:5226/"
API_TIMEOUT_SECONDS=10
```

Copie `.env.example` como `.env` para cambiar la URL o el tiempo de espera del
API. El archivo `.env` no se versiona.

## Arquitectura

```text
Config/          Lectura de .env y valores por defecto
Modelos/DTO      Contratos de cliente, producto, factura y amortización
Modelos/DAO      Acceso HTTP a los endpoints del API de Facturación
Servicios/       Reglas del cliente: filtros, carrito, totales y validaciones
Controladores/   Estado y acciones de cada pantalla
Vistas/          Layout, páginas y modales reutilizables
wwwroot/         Hoja de estilos e imágenes de Celulares Monster
```

La navegación principal se muestra en un layout de escritorio con las secciones
Inicio, Facturas, Créditos, Clientes, Catálogo y Registrar compra.

## Pantallas

- **Inicio:** resumen del sistema y accesos directos a los módulos.
- **Clientes:** listado, búsqueda por cédula o nombre, alta, edición y baja.
- **Catálogo:** listado, búsqueda, alta, edición y baja de productos. El precio
  admite coma o punto como separador decimal y debe ser mayor que cero.
- **Registrar compra:** selección de cliente por cédula, armado del carrito,
  pago en efectivo o a crédito, confirmación y comprobante.
- **Facturas:** listado general, búsqueda por cédula o número, detalle de la
  factura y consulta de la tabla de amortización.
- **Créditos:** selector de los créditos de la tienda y consulta de la tabla de
  amortización con el estado de pago de cada cuota.

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
   en un modal; nunca se calcula localmente.

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

- Al ser Blazor Server, las llamadas al API salen del servidor; no se requiere
  configurar CORS en el API de Facturación.
- El descuento por efectivo mostrado antes de confirmar es solo una vista
  previa: el valor oficial es el que devuelve el API al crear la factura.
- No se modifican datos de BanQuito desde este cliente. Toda operación de
  crédito la resuelve el API de Facturación.
- La pantalla de Créditos es informativa: el estado de pago de cada cuota viene
  del API y no se puede modificar desde el cliente.
