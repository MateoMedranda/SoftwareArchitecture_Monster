# Personalizar el logo

La aplicación usa dos recursos distintos:

| Lugar donde aparece | Archivo que debes reemplazar |
| --- | --- |
| Pantalla de inicio de sesión | `Resources/Images/logo_monster.svg` |
| Icono de la aplicación (Android, Windows e iOS) | `Resources/AppIcon/appiconfg.svg` |

Reemplaza cada archivo por tu logo en formato SVG y conserva exactamente el mismo nombre. Después, detén la depuración y vuelve a ejecutar el proyecto para que MAUI genere de nuevo el icono.

Si tu logo está en PNG, colócalo en `Resources/Images/logo_login.png` y cambia en `Views/LoginPage.xaml` la propiedad `Source` a `logo_login.png`. Para usar un PNG como icono de la aplicación, colócalo en `Resources/AppIcon/appicon.png` y cambia el elemento `MauiIcon` de `MonsterConversorMovil.csproj` para que use ese archivo.
