MONSTER CONVERSOR MÓVIL (.NET MAUI)

Proyecto: MonsterConversorMovil/MonsterConversorMovil.csproj

Cliente móvil Android construido con .NET MAUI y organización MVC:
- Models: contratos de inicio de sesión y conversión.
- Views: LoginPage y ConverterPage en XAML.
- Controllers: AccountController y ConversionController.
- Services: SoapConversionService consume SOAP 1.1 sobre basicHttpBinding.

ANTES DE EJECUTAR
1. Inicie el proyecto WCF CONV_SOAP_DOTNET mediante IIS Express.
2. Abra MonsterConversorMovil.csproj con Visual Studio 2022 y la carga de trabajo .NET MAUI.
3. Seleccione un emulador Android y ejecute el proyecto.

DIRECCIÓN DEL SERVICIO
- Emulador Android: http://10.0.2.2:49159/Servicios/ServicioConversionUnidades.svc
- Windows: http://localhost:49159/Servicios/ServicioConversionUnidades.svc
- Teléfono físico: reemplace 10.0.2.2 por la IP local del computador (por ejemplo,
  http://192.168.1.25:49159/Servicios/ServicioConversionUnidades.svc) y permita el puerto
  en Firewall/IIS Express.

El usuario puede editar la dirección desde la pantalla de ingreso. No use ?wsdl en la
dirección de la aplicación: la app necesita el endpoint .svc para enviar mensajes SOAP.

CREDENCIALES DE PRUEBA
Usuario: MONSTER
Contraseña: MONSTER9

ALCANCE ACTUAL
El servicio .NET implementa solamente la categoría Temperatura: Celsius, Fahrenheit,
Kelvin y Rankine. Longitud y Masa todavía devuelven "no implementada" desde el servidor,
por eso no se muestran en este cliente.

COMPILACIÓN VERIFICADA
dotnet build MonsterConversorMovil.csproj -f net10.0-android

LOGOS
Consulta MonsterConversorMovil/BRANDING.md para reemplazar el logo de inicio de sesión y el icono de la aplicación.
