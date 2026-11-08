# WSDL generados — EUREKABANK

Estos archivos fueron extraídos del servidor Payara con el WAR
`target/EUREKABANK-1.0.war` desplegado en el contexto `/EUREKABANK`.

## Servicios

| Servicio | WSDL | XSD | Endpoint |
|---|---|---|---|
| Login | `LoginControlador.wsdl` | `LoginControlador.xsd` | `https://localhost:8181/LoginServicio/LoginControlador?wsdl` |
| Usuarios | `UsuarioControlador.wsdl` | `UsuarioControlador.xsd` | `https://localhost:8181/UsuarioServicio/UsuarioControlador?wsdl` |
| Cuentas | `CuentaControlador.wsdl` | `CuentaControlador.xsd` | `https://localhost:8181/CuentaServicio/CuentaControlador?wsdl` |
| Transacciones | `TransaccionControlador.wsdl` | `TransaccionControlador.xsd` | `https://localhost:8181/TransaccionServicio/TransaccionControlador?wsdl` |
| Pertenece | `PerteneceControlador.wsdl` | `PerteneceControlador.xsd` | `https://localhost:8181/PerteneceServicio/PerteneceControlador?wsdl` |

Cada WSDL importa su esquema mediante:

```text
<servicio>/<controlador>?xsd=1
```

El archivo `.xsd` guardado aquí es exactamente la respuesta de esa URL.

## Regenerar

Con el WAR desplegado:

```powershell
$base = "https://localhost:8181/UsuarioServicio/UsuarioControlador"
curl.exe -k "$base?wsdl"  -o UsuarioControlador.wsdl
curl.exe -k "$base?xsd=1" -o UsuarioControlador.xsd
```

Repita cambiando el servicio y el controlador según la tabla.

## Nota sobre compatibilidad

- El contrato SOAP (`targetNamespace`, `service`, `port`, `operationName`,
  `@WebResult` y `token` como header) se conserva.
- El `portType` del WSDL ahora se llama como el controlador
  (`LoginControlador`, `UsuarioControlador`, etc.).
- La URL del endpoint cambió el segundo segmento: antes era la clase del
  servicio (`/LoginServicio/LoginServicio`) y ahora es la clase del
  controlador (`/LoginServicio/LoginControlador`).
- En `TransaccionControlador` y `PerteneceControlador` se renombraron los
  métodos Java `listarPorCuenta` y `listarPorUsuario` para evitar la colisión
  de clases wrapper de JAX-WS. Los `operationName` SOAP siguen siendo
  `listarPorCuenta` y `listarPorUsuario`.
