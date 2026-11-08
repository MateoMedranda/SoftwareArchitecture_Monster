# API REST de EUREKABANK

Base URL: `http://localhost:8080/EUREKABANK-RESTFUL/api`. Todas las respuestas son JSON. Las rutas protegidas reciben `Authorization: Bearer <JWT>`.

- `POST /auth/login`, `POST /auth/logout`, `GET /auth/verificar`
- CRUD y perfil de usuarios: `/usuarios`, `/usuarios/{id}/perfil`
- CRUD de cuentas: `/cuentas`; creación/asignación: `POST /cuentas/para-usuario/{usuarioId}`
- Disponibilidad y long polling: `/cuentas/{id}/disponibilidad`
- Movimientos, transferencias y reversas: `/transacciones`
- Asignaciones cuenta/usuario: `/asignaciones`

Se conservan las reglas de autorización, validaciones, transacciones JDBC, auditoría, transferencias atómicas, reversas y bloqueos temporales. Los errores de negocio se devuelven como JSON (`codigo`, `mensaje`, `detalle`) y código HTTP apropiado.

Configure `EUREKABANK_URL` con la URL base de la API. Para iniciar la base: `docker compose up -d`.

## OpenAPI y Swagger UI

Después de desplegar el WAR, abra `http://localhost:8080/EUREKABANK-RESTFUL/swagger-ui.html`.
La especificación reutilizable está en `src/main/webapp/openapi.yaml` y se sirve como `http://localhost:8080/EUREKABANK-RESTFUL/openapi.yaml`.
