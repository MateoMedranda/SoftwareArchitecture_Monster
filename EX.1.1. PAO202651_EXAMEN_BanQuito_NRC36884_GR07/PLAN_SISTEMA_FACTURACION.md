# Plan inicial: Sistema de facturación e integración con BanQuito

## 1. Objetivo y alcance

Completar la aplicación ASP.NET Core **Sistema Facturación** para administrar clientes propios, catálogo de teléfonos y facturas con sus detalles. La aplicación se conecta únicamente a la base `FACTURACION` para sus operaciones y consume por HTTP los servicios del módulo **SISTEMA_BANQUITO** cuando una compra se paga con crédito directo. BanQuito sigue siendo dueño de sus clientes bancarios, cuentas, movimientos, créditos y amortizaciones. Mantener sin ampliaciones las columnas actuales de las tablas y no añadir datos de crédito a `FACTURA`.

Incluye CRUD básico y búsqueda de clientes/productos/facturas, cálculo de totales, emisión transaccional de facturas y consulta por número de factura, cédula o código de cliente. No incluye control de existencias ni modificación directa de tablas de BanQuito.

## 2. Estado encontrado en el repositorio

- `03. BDD/Facturacion.sql` define `CLIENTE(COD_CLIENTE, CEDULA, NOMBRE)`, `ITEM_INVENTARIO(NUM_INVENTARIO, NOMBRE, PRECIO)`, `FACTURA(NUM_FACTURA, COD_CLIENTE, FECHA, TIPO_PAGO, VALOR)` e `ITEM_FACTURA(NUM_ITEM, NUM_FACTURA, NUM_INVENTARIO, VALOR, CANTIDAD)`.
- El script crea índices para las relaciones, pero no declara claves foráneas. Añadir únicamente las tres claves foráneas correspondientes en SQL y reflejarlas en los modelos UML lógico/físico, sin agregar o eliminar atributos de negocio.
- `Facturacion_DATA.sql` contiene cinco clientes y cinco teléfonos. Sus dos facturas seed referencian códigos `1` y `5`, incompatibles con los códigos existentes `106`–`110`; corregir los datos seed (p.ej. usar `106` y `110`) antes de reconstruir la base.
- El proyecto de facturación actualmente es una plantilla: solo tiene conexión base y `WeatherForecast`; faltan modelos de negocio, DAO/repositorios, servicios y controladores.
- BanQuito ya expone `POST /api/sujeto-credito/verificar`, `POST /api/monto-maximo/calcular` y `POST /api/credito/otorgar`. El último recibe `codCliente`, `precioTelefono` y `numCuotas`; crea el crédito y la tabla de amortización.
- Los clientes de las dos BDD tienen cédula/código comunes en los seed, pero son registros independientes. La aplicación de facturación no debe asumir que un código local equivale al código BanQuito. Resolver primero al cliente bancario por cédula mediante la API de BanQuito; si el contrato exige usar código, obtenerlo del servicio de búsqueda de cliente del banco.

## 3. Modelo de datos y reglas de valores

Conservar exactamente las tablas y columnas actuales. No agregar subtotal, descuento, datos de crédito, columnas ni tablas nuevas. `FACTURA.VALOR` almacena el importe final cobrado: para efectivo el precio menos 42%; para crédito, el precio íntegro. `ITEM_FACTURA.VALOR` conserva el precio unitario del producto al momento de la venta y `CANTIDAD` no implica administrar stock. No se envía a la BDD una fila adicional para representar el descuento.

Añadir al script las relaciones: `FACTURA.COD_CLIENTE → CLIENTE.COD_CLIENTE`, `ITEM_FACTURA.NUM_FACTURA → FACTURA.NUM_FACTURA`, `ITEM_FACTURA.NUM_INVENTARIO → ITEM_INVENTARIO.NUM_INVENTARIO`. Reflejar las mismas relaciones en los modelos UML lógico y físico; revisar tanto los `.ldm` como los `.pdm` y entregar/guardar desde la herramienta de modelado en vez de editar a mano archivos binarios. Antes de activar las restricciones, corregir las referencias inválidas de facturas seed (códigos 1 y 5 deben corresponder a clientes reales 106–110), ya que las claves foráneas impedirán cargarlas.

## 4. Reglas y flujo de emisión

1. Validar cliente, método de pago, plazo si aplica y una lista no vacía de productos con cantidades enteras positivas. Cargar productos y precios desde `FACTURACION`; ignorar cualquier precio enviado por el cliente HTTP/UI.
2. Para `EFECTIVO`, aplicar descuento del 42% al importe final de la factura; guardar factura y detalle en una transacción SQL local, sin línea extra de descuento ni llamada a BanQuito.
3. Para `CREDITO`, dejar el importe íntegro en `FACTURA.VALOR`; obtener la identidad bancaria del cliente por cédula, comprobar que sea sujeto y que el importe no supere su máximo autorizado; validar plazo inclusivo de 3–18 meses.
4. Enviar a BanQuito los datos mínimos para otorgar el crédito y generar su amortización. BanQuito guarda crédito y cuotas. Facturación guarda solamente la factura y sus ítems en su propia BDD; no replica ni persiste el plan de amortización ni agrega columnas a `FACTURA`.
5. Emitir la factura local solo cuando BanQuito confirme el otorgamiento. Si falla la escritura local luego del otorgamiento, informar claramente el resultado bancario para conciliación manual. No hay transacción distribuida entre las dos bases.

**Aclaración de contrato necesaria para la integración:** el endpoint `otorgar` ya valida sujeto, monto y plazo según README, por lo que es la decisión final de aprobación. Se puede consultar primero `verificar` y `monto-maximo` para explicar al usuario el resultado, pero `otorgar` debe repetir/garantizar esas reglas para evitar cambios entre consulta y emisión. Evitar crear el crédito solo para luego descubrir que la factura no se pudo guardar.

## 5. Servicios REST de facturación

Base propuesta `/api` y JSON camelCase. Mantener controladores delgados; validación y reglas en servicios; SQL parametrizado en DAO/repositorios.

| Método y ruta | Uso |
|---|---|
| `GET /api/clientes` | Listar clientes de facturación |
| `GET /api/clientes/{codigo}` / `GET /api/clientes/por-cedula/{cedula}` | Consultar cliente local |
| `POST /api/clientes`, `PUT /api/clientes/{codigo}`, `DELETE /api/clientes/{codigo}` | CRUD básico; impedir borrar si tiene facturas |
| `GET /api/productos` / `GET /api/productos/{codigo}` | Catálogo disponible para venta (sin inventario) |
| `POST /api/productos`, `PUT /api/productos/{codigo}`, `DELETE /api/productos/{codigo}` | CRUD básico; impedir borrar si aparece en una factura |
| `POST /api/facturas` | Generar factura a partir de cliente, método de pago, plazo si es crédito e ítems `{productoId, cantidad}`; cargar precios desde BDD, calcular total y persistir encabezado/detalle |
| `GET /api/facturas/{numero}` | Consultar factura por número, incluyendo ítems |
| `GET /api/facturas?cedula=...` o `?codigoCliente=...` | Consultar facturas del cliente; aceptar exactamente un criterio o documentar su precedencia |

Para evitar enviar repetidamente datos calculados: el POST acepta identificadores y cantidades mínimos; la lectura de la factura devuelve los ítems relacionados. También puede ofrecerse `GET /api/facturas/{numero}/items` si se requiere una URI explícita para consultar los ítems. La URI es recurso de consulta; no sustituye persistir `ITEM_FACTURA` en la base.

## 6. Integración SOA y contratos

- Registrar `HttpClient` tipado para BanQuito, con URL base configurable, timeout, cancelación, manejo de errores y respuestas DTO versionadas; nunca poner URL en el controlador.
- Contratos actuales documentados por BanQuito: `POST /api/sujeto-credito/verificar` con `{cedula}`, `POST /api/monto-maximo/calcular` con `{codCliente}` y `POST /api/credito/otorgar` con `{codCliente, precioTelefono, numCuotas}`. Confirmar en DTOs reales cómo se resuelve cédula→código y cómo se representa rechazo/errores antes de implementar el cliente. El endpoint de otorgamiento es el que crea crédito y amortización en BanQuito.
- No acoplar bases de datos ni hacer joins entre `FACTURACION` y `BANQUITO`; toda consulta bancaria va por servicio.
- Usar respuestas HTTP coherentes: `201 Created` al emitir, `400` para datos inválidos, `404` para cliente/producto inexistente, `409` para no elegible/sin cupo/conflicto, `502/503` si BanQuito falla o no está disponible. Incluir código de motivo y mensaje claro.

## 7. Configuración por ambiente

La URL de BanQuito y la conexión de SQL deben poder cambiarse sin modificar el código. Mantener claves de configuración en `appsettings.json` sin secretos y permitir sobrescribirlas con variables de entorno (ASP.NET Core `__`):

```json
{
  "ConnectionStrings": {
    "Facturacion": "Server=localhost,1433;Database=FACTURACION;User Id=sa;Password=CAMBIAR;TrustServerCertificate=True;Encrypt=False"
  },
  "Servicios": {
    "Banquito": {
      "BaseUrl": "http://localhost:5173/",
      "TimeoutSegundos": 10
    }
  }
}
```

Crear `.env.example` configurable para desarrollo local/Docker, no versionar `.env` con contraseñas reales. Ejemplo (ajustar host/puerto según la red de contenedores):

```dotenv
FACTURACION_DB_SERVER=localhost,1433
FACTURACION_DB_NAME=FACTURACION
FACTURACION_DB_USER=sa
FACTURACION_DB_PASSWORD=cambiar_localmente
BANQUITO_BASE_URL=http://localhost:5173/
BANQUITO_TIMEOUT_SECONDS=10
```

El proyecto puede mapear estas variables a `ConnectionStrings__Facturacion` y `Servicios__Banquito__BaseUrl`/`TimeoutSegundos` o construir la cadena de conexión desde sus componentes. En Docker, `localhost` dentro del contenedor apunta al propio contenedor: utilizar el nombre de servicio de Docker para SQL Server y BanQuito. La reestructuración de contenedores queda para una tarea posterior.

## 8. Endpoints para pruebas manuales

Base BanQuito local documentada: `http://localhost:5173`. La base de facturación debe usar la URL/puerto que indique su perfil de lanzamiento.

| Servicio | Método/ruta | Ejemplo de body |
|---|---|---|
| Salud BanQuito | `GET /api/salud` | — |
| Buscar cliente banco por cédula | `POST /api/clientes/buscar-cedula` | `{"valor":"0102030405"}` |
| Verificar sujeto | `POST /api/sujeto-credito/verificar` | `{"cedula":"0102030405"}` |
| Calcular monto máximo | `POST /api/monto-maximo/calcular` | `{"codCliente":106}` |
| Otorgar crédito y generar amortización | `POST /api/credito/otorgar` | `{"codCliente":106,"precioTelefono":450.00,"numCuotas":6}` |
| Consultar amortización | `POST /api/amortizacion/consultar` | `{"numCredito":1002}` |
| Emitir factura local | `POST /api/facturas` | `{"codCliente":106,"tipoPago":"EFECTIVO","items":[{"numInventario":1,"cantidad":1}]}` |
| Emitir factura crédito local | `POST /api/facturas` | `{"codCliente":106,"tipoPago":"CREDITO","numCuotas":6,"items":[{"numInventario":1,"cantidad":1}]}` |
| Consultar factura e ítems | `GET /api/facturas/5001` | — |
| Consultar facturas por cédula | `GET /api/facturas?cedula=0102030405` | — |
| Consultar facturas por código local | `GET /api/facturas?codigoCliente=106` | — |

La API de facturación está planificada y esas rutas aún deben implementarse; son propuestas para la prueba manual posterior. Las llamadas BanQuito anteriores ya están documentadas en `04. SERVIDOR/README.md` y el archivo `.http` del proyecto bancario.

## 9. Fases de implementación

1. **Preparación de esquema:** corregir seed de códigos; agregar únicamente claves foráneas al script de facturación y relaciones a los UML lógico/físico.
2. **Dominio y persistencia local:** modelos `Cliente`, `Producto`, `Factura`, `ItemFactura`; DTOs de entrada/salida; DAO con SQL parametrizado y transacciones; servicios de CRUD y búsquedas.
3. **Facturación de contado:** endpoint de emisión, cálculo de descuento, snapshots de precio y lectura completa por número/cédula/código.
4. **Integración BanQuito:** `HttpClient` tipado, resolución del cliente por cédula, validación de cupo/plazo, otorgamiento y captura de referencia/tabla de amortización donde aplique.
5. **Operación y documentación:** configuración por ambiente, ejemplos HTTP/OpenAPI, tratamiento de indisponibilidad bancaria y reconciliación de operaciones parcialmente completadas. La reestructuración de contenedores y ejecución de pruebas manuales se realizará después, fuera de este plan inicial.

## 8. Criterios de aceptación

- El catálogo presenta productos y precios sin gestionar existencias.
- Efectivo aplica exactamente 42% de descuento en el valor final de la factura, persiste factura y todos los ítems sin guardar una fila/campo de descuento y no llama a BanQuito.
- Crédito conserva el importe íntegro en factura, rechaza clientes no elegibles o importes mayores al máximo, valida 3–18 cuotas y factura únicamente tras aprobación del banco; la amortización se almacena solo en BanQuito.
- La consulta por número devuelve encabezado e ítems; las consultas por cédula y código devuelven las facturas del cliente correcto.
- La modificación futura del precio de catálogo no cambia importes de facturas ya emitidas.
- Código/cédula de facturación no se confunden con identidad bancaria; la conexión bancaria usa HTTP y maneja fallos sin emitir una factura de crédito no aprobada.

## 9. Decisiones que se deben confirmar al implementar

- Si se permite más de un teléfono por factura y cantidades mayores a uno (el modelo actual lo admite; el caso de estudio habla de un teléfono).
- El formato exacto de la fórmula de amortización está en las imágenes que no aparecen como texto en el enunciado; mantener la tasa mensual `0.165/12` y usar la fórmula/algoritmo ya implementado en BanQuito como fuente de verdad.
- Confirmar si la factura debe conservar número de crédito y plan completo de cuotas o solo el identificador y consultar las cuotas al banco.
- Resolver la atomicidad distribuida entre el guardado local y el otorgamiento del crédito mediante idempotencia o compensación bancaria si se requiere garantía ante fallos.
