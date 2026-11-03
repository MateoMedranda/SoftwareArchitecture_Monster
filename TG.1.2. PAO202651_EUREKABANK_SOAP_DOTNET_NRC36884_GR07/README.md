# EUREKABANK SOAP .NET + SQL Server

Implementación .NET del proyecto EurekaBank: servidor SOAP WCF (CoreWCF), SQL Server, y clientes de consola, escritorio WPF, web MVC y móvil .NET MAUI. Las vistas aplican la misma identidad visual azul/turquesa y los mismos casos de uso del proyecto Java de referencia.

## Inicio rápido

1. Desde `04. SERVIDOR`, ejecute `docker compose up -d`. Se crea SQL Server 2022, la base `arquitectura_db` y los datos de prueba.
2. Ejecute el servidor: `dotnet run --project "04. SERVIDOR/EurekaBank.Server"`.
3. Consulte el WSDL en `http://localhost:5000/EurekaBankSoap.svc?wsdl`. Si el perfil local usa otro puerto, configure `EUREKABANK_URL` con la URL base terminada en `/EurekaBankSoap.svc` antes de iniciar los clientes.
4. Credenciales iniciales: `MONSTER / MONSTER9`, `CLIENTE1 / Cliente1*2026`, `CLIENTE2 / Cliente2*2026`.

## IIS

Instale el Hosting Bundle de .NET 9 y habilite IIS. Publique con:

```powershell
dotnet publish "04. SERVIDOR/EurekaBank.Server/EurekaBank.Server.csproj" -c Release -o C:\inetpub\EurekaBank
```

Cree en IIS un Application Pool **sin código administrado**, asigne la carpeta publicada y dé permiso de lectura/ejecución a `IIS_IUSRS`. Configure en producción la cadena `ConnectionStrings__EurekaBank` como variable de entorno del sitio; no conserve la clave de desarrollo.

## Proyectos

- `04. SERVIDOR/EurekaBank.Server`: WCF/CoreWCF, EF Core y las capas `Contratos`, `Modelos`, `Servicios` y `Properties`.
- `04. CLICON/EurekaBank.Console`: cliente de consola con sus capas `Modelos` y `Servicios`.
- `04. CLIESC/EurekaBank.Desktop`: cliente WPF con sus capas `Modelos`, `Servicios` y `Vistas`.
- `04. CLIWEB/EurekaBank.Web`: sitio ASP.NET Core MVC con `Controllers`, `Views`, `Modelos` y `Servicios`.
- `04. CLIMOV/EurekaBank.Mobile`: aplicación .NET MAUI con sus capas `Modelos`, `Servicios` y `Vistas`.

Cada componente incluye su propio archivo `.sln`, por lo que puede abrirse de forma independiente en Visual Studio.

Los scripts T-SQL están en `04. SERVIDOR/01-modelo.sql` y `04. SERVIDOR/02-datos.sql`; la migración compatible está en `03. BDD/MIGRACION_01_AGREGAR_NUMERO_CUENTA.sql`.
