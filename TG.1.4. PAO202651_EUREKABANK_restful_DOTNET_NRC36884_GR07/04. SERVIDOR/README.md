# Servidor EurekaBank .NET

`EurekaBank.Server` expone la API REST v1 y conserva el servicio SOAP WCF (CoreWCF) como compatibilidad para los clientes ya instalados. Ambos usan la misma lógica de negocio y la misma base de datos.

## Servicio

- Swagger UI: `http://localhost:62558/swagger`
- Documento OpenAPI: `http://localhost:62558/swagger/v1/swagger.json`
- Base REST: `http://localhost:62558/api/v1`
- WSDL SOAP de compatibilidad: `http://localhost:62558/EurekaBankSoap.svc?wsdl`

La API REST recibe el token con `Authorization: Bearer {token}`. Cree la sesión con `POST /api/v1/auth/sessions`; Swagger permite probar cada operación desde el navegador.

## Convivencia con SOAP

El perfil REST usa los puertos `62557` (HTTPS) y `62558` (HTTP), distintos de la instalación SOAP anterior (`62555`/`62556`). Por ello pueden ejecutarse lado a lado durante la transición. Los artefactos SOAP se mantienen en el código y el endpoint `/EurekaBankSoap.svc` sigue publicado.

## Base de datos

Ejecute `docker compose up -d` en esta carpeta. El contenedor `sql-init` aplica el modelo y los datos al SQL Server 2022. La cadena usada por desarrollo está en `EurekaBank.Server/appsettings.json`.

## IIS

Publique `EurekaBank.Server` en una carpeta de IIS con el Hosting Bundle de .NET 9 instalado. Para convivir con SOAP, use el perfil `Properties/PublishProfiles/IIS.Rest.pubxml`, que publica en `C:\inetpub\EurekaBankRest`, y cree un sitio o aplicación IIS con enlace distinto. Use un Application Pool sin código administrado y configure `ConnectionStrings__EurekaBank` fuera del repositorio para producción.
