Arquitectura Monster

Cliente móvil nativo Android (APK) con .NET MAUI Blazor Hybrid:
CLIMOV_Facturacion\CLIMOV_Facturacion\CLIMOV_Facturacion.csproj

Consulte CLIMOV_Facturacion\CLIMOV_Facturacion\README.md para requisitos,
configuración del API y comandos de compilación del APK.

Para conexión
adb pair ip:puerto -> pide a pairing code
adb connect ip:puerto
compilacion y despliegue
dotnet publish .\CLIMOV_Facturacion.csproj -f net10.0-android -c Release -p:AndroidPackageFormat=apk -p:EmbedAssembliesIntoApk=true
adb install -r .\bin\Release\net10.0-android\publish\ec.edu.espe.climov.facturacion-Signed.apk
adb shell monkey -p ec.edu.espe.climov.facturacion 1