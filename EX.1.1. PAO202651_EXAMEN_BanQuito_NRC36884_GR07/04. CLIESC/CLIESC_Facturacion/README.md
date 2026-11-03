# CLIESC Facturacion

Cliente de escritorio WinForms para el API de facturacion.

## Requisitos

- Windows
- .NET Framework 4.8
- SDK de .NET 10 (para compilar desde consola) o Visual Studio 2022

## Compilar y ejecutar

Desde esta carpeta:

```powershell
dotnet build CLIESC_Facturacion.slnx
dotnet run --project CLIESC_Facturacion\CLIESC_Facturacion.csproj
```

El API debe estar disponible en `http://localhost:5226/`.

## Configuracion

El cliente busca un archivo `.env` en el directorio del ejecutable, en el
directorio de trabajo o en las carpetas superiores. Si no lo encuentra, usa:

```text
FACTURACION_API_BASE_URL=http://localhost:5226/
FACTURACION_TIMEOUT_SECONDS=10
```

Copie `.env.example` como `.env` para cambiar la URL o el timeout. El archivo
`.env` no se versiona.

## Arquitectura

```text
Config/          Lectura de .env y valores por defecto
Modelos/         DTO de cliente, producto y respuesta del API
Servicios/       Cliente HTTP y acceso a los endpoints
Controladores/   Logica de la vista de clientes y catalogo
Vistas/          Pantallas de inicio, clientes, catalogo y dialogos
resources/       Logotipo de Celulares Monster
```

## Pantallas

- **Inicio:** navegacion a clientes y catalogo.
- **Clientes:** listar, buscar por cedula o codigo, crear, editar y eliminar.
- **Catalogo:** listar, buscar por codigo, crear, editar y eliminar productos.

Se consumen unicamente los endpoints de `ClienteControlador` y
`ProductoControlador` definidos en `Plantillas/Cliente/openapi_spec.json`.
La pantalla de facturas queda pendiente para una siguiente iteracion.
