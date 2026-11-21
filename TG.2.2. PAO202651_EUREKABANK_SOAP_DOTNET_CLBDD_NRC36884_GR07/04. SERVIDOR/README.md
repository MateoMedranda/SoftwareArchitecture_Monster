# Servidor EurekaBank .NET

`EurekaBank.Server` es un servicio SOAP WCF construido con CoreWCF y EF Core para SQL Server.

## Servicio

- WSDL: `http://localhost:5000/EurekaBankSoap.svc?wsdl`
- Endpoint SOAP: `http://localhost:5000/EurekaBankSoap.svc`
- Operaciones: login/logout, cuentas, perfil, clientes, movimientos, depósitos, retiros, transferencias, reversas y creación de cuentas.

## Base de datos

Ejecute `docker compose up -d` en esta carpeta. El contenedor `sql-init` aplica el modelo y los datos al SQL Server 2022. La cadena usada por desarrollo está en `EurekaBank.Server/appsettings.json`.

## IIS

Publique `EurekaBank.Server` en una carpeta de IIS con el Hosting Bundle de .NET 9 instalado. El proyecto contiene `web.config` y el perfil `Properties/PublishProfiles/IIS.pubxml`. Use un Application Pool sin código administrado y configure `ConnectionStrings__EurekaBank` fuera del repositorio para producción.
