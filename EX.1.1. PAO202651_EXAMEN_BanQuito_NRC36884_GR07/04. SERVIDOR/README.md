## Estructura

```
04. SERVIDOR/
├── docker-compose.yml           Proyecto "sistema_banquito" (SQL Server + init)
├── Dockerfile.base_datos        Imagen dedicada a crear/cargar las 2 bases
├── base_datos/
│   └── inicializar.sh           Crea BANQUITO y FACTURACION y carga sus .sql
├── .development.env             Credenciales de SQL Server (lo lee docker-compose)
└── SISTEMA_BANQUITO/
    ├── SISTEMA_BANQUITO.slnx    Solución para abrir en Visual Studio
    └── SISTEMA_BANQUITO/
        ├── Modelo/
        │   ├── Conexion.cs      Conexión a SQL Server
        │   ├── Cliente.cs, Cuenta.cs, Movimiento.cs, Credito.cs, Amortizacion.cs
        │   ├── DAO/             Acceso a datos (CRUD + transacciones)
        │   ├── DTO/             Contratos JSON
        │   └── Mapeo/           Mapeo inverso entidad ↔ DTO
        ├── Comun/               ExcepcionNegocio y middleware de errores
        ├── Servicio/            Reglas de negocio y fórmulas
        ├── Controlador/         Endpoints REST (CRUD + específicos)
        ├── Program.cs
        └── appsettings*.json
```

## Endpoints

### Sistema de facturación

Proyecto: `Sistema Facturacion/Sistema Facturacion`. URL local por defecto: `http://localhost:5226`.

| Método | Ruta | Descripción |
| --- | --- | --- |
| GET/POST | `/api/clientes` | Listar y crear clientes locales |
| GET/PUT/DELETE | `/api/clientes/{codigo}` | Buscar, actualizar o eliminar cliente |
| GET | `/api/clientes/por-cedula/{cedula}` | Buscar cliente por cédula |
| GET/POST | `/api/productos` | Listar y crear teléfonos disponibles |
| GET/PUT/DELETE | `/api/productos/{codigo}` | Buscar, actualizar o eliminar teléfono |
| POST | `/api/facturas` | Emitir factura con ítems referenciados por código |
| GET | `/api/facturas/{numero}` | Consultar encabezado y detalle |
| GET | `/api/facturas/{numero}/items` | Consultar los ítems de una factura |
| GET | `/api/facturas?cedula={cedula}` | Consultar facturas del cliente por cédula |
| GET | `/api/facturas?codigoCliente={codigo}` | Consultar facturas por código local |

En efectivo el total es el 58% del subtotal (42% de descuento). En crédito se conserva el importe íntegro; BanQuito valida elegibilidad/tope y registra el crédito con su amortización. Ejemplos completos están en `Sistema Facturacion/Sistema Facturacion/Sistema Facturacion.http`.

Los dos proyectos leen las cadenas `ConnectionStrings__Facturacion` y `ConnectionStrings__Banquito` desde `.development.env` al ejecutar en ambiente Development. Para pruebas locales, ambas bases se sirven desde SQL Server en `localhost,1433`; BanQuito usa `http://localhost:5173/`.

### Servicios específicos

| Método | Ruta | Descripción |
| --- | --- | --- |
| POST | `/api/sujeto-credito/verificar` | Verifica si un cliente es sujeto de crédito |
| POST | `/api/monto-maximo/calcular` | Calcula el monto máximo (últimos 3 meses) |
| POST | `/api/credito/otorgar` | Otorga crédito y genera la tabla de amortización |
| POST | `/api/amortizacion/consultar` | Consulta la tabla por crédito o por cliente |

### CRUD

Base URL: http://localhost:5173

También tienes todos estos ejemplos listos en SISTEMA_BANQUITO.http.
**Diagnóstico**
| Método |	Ruta |
| --- | --- |
| GET	| /api/salud |
| GET	| /api/salud/conexion |
| GET	| /openapi/v1.json (documento OpenAPI) |
#### Servicios específicos
**Verificar sujeto de crédito** 
POST /api/sujeto-credito/verificar
{ "cedula": "0102030405" }
Con el seed: Ana true, Luis EDAD_INSUFICIENTE, María SIN_DEPOSITO, Carlos CREDITO_ACTIVO y Sofía CREDITO_ACTIVO (tiene el crédito 1002).

### Monto máximo
POST /api/monto-maximo/calcular
{ "codCliente": 106 }
Otorgar crédito (P=450, n=6 → primera cuota 78.65)

POST /api/credito/otorgar
{
  "codCliente": 106,
  "precioTelefono": 450.00,
  "numCuotas": 6
}

### Consultar tabla de amortización
POST /api/amortizacion/consultar
{ "numCredito": 1002 }
También acepta { "codCliente": 110 }.
### Clientes — /api/clientes
 Método |	Ruta | Body |
 | --- | --- | --- |
| GET	| /api/clientes |	— |
| POST |	/api/clientes/buscar-codigo	| { "valor": 106 } |
| POST |	/api/clientes/buscar-cedula	| { "valor": "0102030405" } |
| POST |	/api/clientes	| ver abajo |
| PUT	| /api/clientes/{codigo}	| mismo body |
| DELETE |	/api/clientes/{codigo}	| — |

{
  "cedula": "0607080910",
  "nombre": "Pedro Salazar",
  "genero": "M",
  "fechaNacimiento": "1990-03-15"
}

### Cuentas — /api/cuentas
 Método |	Ruta | Body |
 | --- | --- | --- |
| GET	| /api/cuentas |	—
| POST | /api/cuentas/buscar	| { "valor": "10000001" }
| POST	| /api/cuentas/por-cliente |	{ "valor": 106 }
| POST	| /api/cuentas	| ver abajo
| PUT	| /api/cuentas/{numCuenta}	| mismo body
| DELETE |	/api/cuentas/{numCuenta}	| — |

{ "codCliente": 106, "saldoCuenta": 250.00 }
### Movimientos — /api/movimientos
 Método |	Ruta | Body |
 | --- | --- | --- |
| GET	| /api/movimientos	| —
| POST	| /api/movimientos/buscar	| { "valor": 1 }
| POST	| /api/movimientos/por-cuenta	| { "valor": "10000001" }
| POST	| /api/movimientos/por-cliente	| { "valor": 106 }
| POST	| /api/movimientos	| ver abajo
| PUT	| /api/movimientos/{codigo}	| mismo body
| DELETE	| /api/movimientos/{codigo} | 	—
{
  "numCuenta": "10000001",
  "tipo": "DEP",
  "valor": 100.00
}

Tipo permitido: "DEP" o "RET". El saldo de la cuenta se actualiza en la misma transacción.

### Créditos — /api/creditos
 Método |	Ruta | Body |
 | --- | --- | --- |
| GET	| /api/creditos	| —| 
| POST	| /api/creditos/buscar	| { "valor": 1002 }| 
| POST	| /api/creditos/por-cuenta	| { "valor": "10000005" }| 
| POST	| /api/creditos/por-cliente	| { "valor": 110 }| 
| PUT	| /api/creditos/{numCredito}	| { "creditoTotal": 900.00 }| 
| DELETE	| /api/creditos/{numCredito}	| — | 

### Amortizaciones — /api/amortizaciones
 Método |	Ruta | Body |
 | --- | --- | --- |
| POST	| /api/amortizaciones/tabla	| { "valor": 1002 }
| POST	| /api/amortizaciones/tabla-por-cliente	| { "valor": 110 }
| GET	| /api/amortizaciones/1002/1	| —
| POST	| /api/amortizaciones/pagar	| ver abajo

{
  "numCredito": 1002,
  "numCuota": 1,
  "numCuenta": "10000005"
}

## Actualización del esquema

```powershell
docker compose down -v
docker compose up -d --build
```
@Sistema_Facturacion_HostAddress = http://localhost:5226

### Listar clientes
GET {{Sistema_Facturacion_HostAddress}}/api/clientes
Accept: application/json

### Buscar cliente por cédula
GET {{Sistema_Facturacion_HostAddress}}/api/clientes/por-cedula/0102030405
Accept: application/json

### Listar teléfonos disponibles
GET {{Sistema_Facturacion_HostAddress}}/api/productos
Accept: application/json

### Crear factura en efectivo (descuento del 42% aplicado al total)
POST {{Sistema_Facturacion_HostAddress}}/api/facturas
Content-Type: application/json

{
  "codCliente": 106,
  "tipoPago": "EFECTIVO",
  "items": [
    { "numInventario": 1, "cantidad": 1 }
  ]
}

### Crear factura a crédito (precio íntegro; BanQuito registra crédito y amortización)
POST {{Sistema_Facturacion_HostAddress}}/api/facturas
Content-Type: application/json

{
  "codCliente": 106,
  "tipoPago": "CREDITO",
  "numCuotas": 6,
  "items": [
    { "numInventario": 1, "cantidad": 1 }
  ]
}

### Consultar factura por número
GET {{Sistema_Facturacion_HostAddress}}/api/facturas/5001
Accept: application/json

### Consultar ítems de una factura
GET {{Sistema_Facturacion_HostAddress}}/api/facturas/5001/items
Accept: application/json

### Consultar facturas por cédula
GET {{Sistema_Facturacion_HostAddress}}/api/facturas?cedula=0102030405
Accept: application/json

### Consultar facturas por código de cliente
GET {{Sistema_Facturacion_HostAddress}}/api/facturas?codigoCliente=106
Accept: application/json
# .env muestra
## .development.env BANQUITO
MSSQL_SA_PASSWORD=Contrasena2616164

# .development.env Facturacion
FACTURACION_DB_SERVER=localhost,1433
FACTURACION_DB_NAME=FACTURACION
FACTURACION_DB_USER=sa
FACTURACION_DB_PASSWORD=Contrasena2616164

BANQUITO_DB_SERVER=localhost,1433
BANQUITO_DB_NAME=BANQUITO
BANQUITO_DB_USER=sa
BANQUITO_DB_PASSWORD=Contrasena2616164

# URL del API BanQuito para consumo desde facturacion en ejecucion local.
BANQUITO_BASE_URL=http://localhost:5173/
BANQUITO_TIMEOUT_SECONDS=10

