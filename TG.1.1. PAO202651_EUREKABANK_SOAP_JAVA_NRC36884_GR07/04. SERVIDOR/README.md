# Endpoints
> Con Payara recién instalado use HTTP en el puerto 8080. Use HTTPS (habitualmente 8181) solo después de configurar un listener y certificado TLS. Con el contexto por defecto del WAR, las URL son:

- Login: `http://localhost:8080/EUREKABANK/LoginServicio?wsdl`
- Transacciones: `http://localhost:8080/EUREKABANK/TransaccionServicio?wsdl`
- Cuentas: `http://localhost:8080/EUREKABANK/CuentaServicio?wsdl`
- Usuarios: `http://localhost:8080/EUREKABANK/UsuarioServicio?wsdl`
- Pertenece: `http://localhost:8080/EUREKABANK/PerteneceServicio?wsdl`

# Operaciones bancarias incorporadas

- `TransaccionServicio.realizarTransferencia`: transfiere fondos entre dos cuentas en una sola transacción JDBC; crea un débito y un crédito auditables.
- `TransaccionServicio.reversarTransaccion`: compensa un depósito o retiro sin borrar el movimiento original. Las transferencias se corrigen con una nueva transferencia en sentido inverso.
- `CuentaServicio.obtenerDisponibilidadCuenta`: consulta si una cuenta está `DISPONIBLE` o `BLOQUEADA` mientras se procesa una operación.
- `CuentaServicio.esperarCambioDisponibilidad`: long polling SOAP de hasta 30 segundos para recibir el siguiente cambio de disponibilidad. El cliente debe enviar la `version` obtenida en la consulta anterior.
- `UsuarioServicio.obtenerPerfilBancario`: entrega los datos del usuario y todas sus cuentas asignadas en una sola respuesta.

Los bloqueos temporales de disponibilidad viven en el servidor y no modifican el esquema de MySQL. Las operaciones monetarias usan además bloqueos `SELECT ... FOR UPDATE` y una única conexión JDBC para proteger el saldo ante concurrencia.

# Archivo .development.env en:
- SoftwareArchitecture_Monster\TG.1.1. PAO202651_EUREKABANK_SOAP_JAVA_NRC36884_GR07\04. SERVIDOR\EUREKABANK\src\main\resources

# Levantamiento

- docker compose up -d  
