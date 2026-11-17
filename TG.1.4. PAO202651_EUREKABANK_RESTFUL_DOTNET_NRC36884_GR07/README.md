# EUREKABANK REST .NET + SQL Server (compatible con SOAP)

Implementación .NET del proyecto EurekaBank: API REST ASP.NET Core documentada con OpenAPI/Swagger, compatibilidad SOAP WCF (CoreWCF), SQL Server, y clientes de consola, escritorio WPF, web MVC y móvil .NET MAUI. Las vistas conservan su identidad visual azul/turquesa, MVC y los mismos casos de uso.

## Inicio rápido

1. Desde `04. SERVIDOR`, ejecute `docker compose up -d`. Se crea SQL Server 2022, la base `arquitectura_db` y los datos de prueba.
2. Ejecute el servidor REST: `dotnet run --project "04. SERVIDOR/EurekaBank.Server"`.
3. Abra Swagger en `http://localhost:62558/swagger` o el documento OpenAPI en `http://localhost:62558/swagger/v1/swagger.json`. Los clientes REST usan `EUREKABANK_REST_URL` (por defecto `http://localhost:62558/api/v1`).
4. El WSDL SOAP permanece disponible en `http://localhost:62558/EurekaBankSoap.svc?wsdl` para clientes heredados.
5. Credenciales iniciales: `MONSTER / MONSTER9`, `CLIENTE1 / Cliente1*2026`, `CLIENTE2 / Cliente2*2026`.

## IIS

Instale el Hosting Bundle de .NET 9 y habilite IIS. Publique con:

```powershell
dotnet publish "04. SERVIDOR/EurekaBank.Server/EurekaBank.Server.csproj" -c Release -o C:\inetpub\EurekaBankRest
```

Cree en IIS un Application Pool **sin código administrado** y un sitio o aplicación nuevos (por ejemplo, `EurekaBankRest`) con un enlace distinto al SOAP existente; asigne `C:\inetpub\EurekaBankRest` y dé permiso de lectura/ejecución a `IIS_IUSRS`. Configure en producción la cadena `ConnectionStrings__EurekaBank` como variable de entorno del sitio; no conserve la clave de desarrollo. También puede usar el perfil `IIS.Rest` incluido.

## Proyectos

- `04. SERVIDOR/EurekaBank.Server`: REST v1, Swagger/OpenAPI, compatibilidad WCF/CoreWCF y EF Core.
- `04. CLICON/EurekaBank.Console`, `04. CLIESC/EurekaBank.Desktop`, `04. CLIWEB/EurekaBank.Web` y `04. CLIMOV/EurekaBank.Mobile`: clientes REST que conservan MVC y las vistas existentes.

Cada componente incluye su propio archivo `.sln`, por lo que puede abrirse de forma independiente en Visual Studio. La aplicación móvil REST usa el identificador Android `ec.edu.eurekabank.mobile.rest`, distinto al de SOAP, por lo que ambas APK se pueden instalar en paralelo.

Los scripts T-SQL están en `04. SERVIDOR/01-modelo.sql` y `04. SERVIDOR/02-datos.sql`; la migración compatible está en `03. BDD/MIGRACION_01_AGREGAR_NUMERO_CUENTA.sql`.
