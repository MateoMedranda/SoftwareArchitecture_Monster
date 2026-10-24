# Cliente móvil REST

Cliente .NET MAUI que conserva el diseño del cliente móvil de referencia y consume el servidor REST de este repositorio mediante JWT.

## Configuración

La URL predeterminada se encuentra en `appsettings.json`:

```json
{
  "ServicioRest": {
    "Url": "http://10.0.2.2:60980"
  }
}
```

- Emulador Android: use `http://10.0.2.2:60980` para acceder al servidor ejecutado en el equipo anfitrión.
- Aplicación Windows: use `http://localhost:60980`.
- Dispositivo físico: use la IP local del equipo, por ejemplo `http://192.168.1.20:60980`.

En la pantalla de inicio de sesión, pulse **Cambiar URL** para mostrar el campo oculto y reemplazar la dirección. La aplicación guarda una URL válida para los próximos inicios.

## Ejecución

1. Inicie `ServidorConversionesDotnet`.
2. Verifique que User Secrets del servidor tenga `CLAVE_SECRETA`, `USUARIO_DEMO` y `CONTRASENA_DEMO`.
3. Abra `ClienteMovilConversionRest.csproj` en Visual Studio y ejecute el destino Windows o Android.
4. Inicie sesión con las credenciales demo configuradas en el servidor.

El cliente realiza `POST /api/autenticacion/login`, guarda el JWT durante la sesión y lo envía como `Authorization: Bearer {token}` a las conversiones de temperatura, longitud y masa.

## Arquitectura MVC

- `Models`: contratos REST y opciones de categorías/unidades.
- `Views`: páginas MAUI de inicio de sesión y conversión.
- `Controllers`: validación de la interacción y coordinación entre vistas y servicios.
- `Services`: cliente HTTP REST, configuración de URL y estado de sesión.
