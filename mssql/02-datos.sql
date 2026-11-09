/*==============================================================*/
/* EUREKABANK - 02. DATOS INICIALES (SQL Server)                */
/* Requiere haber ejecutado 01-modelo.sql                       */
/* Motor: SQL Server 2022                                       */
/*==============================================================*/

USE arquitectura_db;
GO

/* Roles (unicos) */
INSERT INTO dbo.ROL (ROL_ID, ROL_NOMBRE) VALUES
   ('a87b1b30-dede-473d-8ebd-fc397c6cd2be', 'ADMIN'),
   ('5bd2b9c5-37e3-47ea-b1f9-4fea40a5e841', 'CLIENTE'),
   ('7ffd5c6a-4903-49cc-a0c9-a64a24b5c5c7', 'CAJERO');

/* Sucursales */
INSERT INTO dbo.SUCURSAL (SUCURSAL_ID, SUCURSAL_NOMBRE) VALUES
   ('c3ff12d2-b6b9-4657-90f4-5c63d49841b3', 'Sucursal Quito Norte'),
   ('a55fbee6-fe6a-4d82-809a-381787ccaade', 'Sucursal Guayaquil Centro'),
   ('33039bda-e9a6-4e0a-9b39-823863425d3c', 'Sucursal Cuenca El Batan');

/* Usuarios: administrador de aplicacion y dos clientes */
INSERT INTO dbo.USUARIO (USUARIO_ID, ROL_ID, USUARIO_NOMBRE, USUARIO_APELLIDO,
                     USUARIO_TELEFONO, USUARIO_USUARIO, USUARIO_CONTRASEANIA) VALUES
   ('77874037-b936-41bf-9195-2eb6e4267bd6', 'a87b1b30-dede-473d-8ebd-fc397c6cd2be',
    'MONSTER', 'ADMINISTRADOR', '0999999999', 'MONSTER', 'MONSTER9'),
   ('6ab15134-c789-48dd-bd57-53bc23611d04', '5bd2b9c5-37e3-47ea-b1f9-4fea40a5e841',
    'MATIAS SEBASTIAN', 'TORRES VILLACIS', '0998123456', 'CLIENTE1', 'Cliente1*2026'),
   ('a75ead9c-db78-49ed-a41d-0b059c1af6fb', '5bd2b9c5-37e3-47ea-b1f9-4fea40a5e841',
    'VALENTINA SOFIA', 'RAMIREZ LEON', '0987456123', 'CLIENTE2', 'Cliente2*2026');

/* Cuentas: una propia por cliente y una compartida */
INSERT INTO dbo.CUENTA (CUENTA_ID, SUCURSAL_ID, CUENTA_SALDO, CUENTA_ESTADO) VALUES
   ('1861a565-3b19-4efe-baa9-e8f964578e5f', 'c3ff12d2-b6b9-4657-90f4-5c63d49841b3',
    1250.75, 'ACTIVA'),
   ('f3ec0bbd-ecb5-4fc4-95f5-aeebb328c1d2', 'a55fbee6-fe6a-4d82-809a-381787ccaade',
    2480.30, 'ACTIVA'),
   ('6a224226-10d3-4de9-b8dc-2215ce346c03', '33039bda-e9a6-4e0a-9b39-823863425d3c',
    5000.00, 'ACTIVA');

/* Propiedad de cuentas */
INSERT INTO dbo.PERTENECE (CUENTA_ID, USUARIO_ID) VALUES
   ('1861a565-3b19-4efe-baa9-e8f964578e5f', '6ab15134-c789-48dd-bd57-53bc23611d04'),
   ('f3ec0bbd-ecb5-4fc4-95f5-aeebb328c1d2', 'a75ead9c-db78-49ed-a41d-0b059c1af6fb'),
   ('6a224226-10d3-4de9-b8dc-2215ce346c03', '6ab15134-c789-48dd-bd57-53bc23611d04'),
   ('6a224226-10d3-4de9-b8dc-2215ce346c03', 'a75ead9c-db78-49ed-a41d-0b059c1af6fb');

UPDATE dbo.USUARIO SET USUARIO_CONTRASEANIA = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'MONSTER9'), 2)) WHERE USUARIO_USUARIO = 'MONSTER';
UPDATE dbo.USUARIO SET USUARIO_CONTRASEANIA = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Cliente1*2026'), 2)) WHERE USUARIO_USUARIO = 'CLIENTE1';
UPDATE dbo.USUARIO SET USUARIO_CONTRASEANIA = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Cliente2*2026'), 2)) WHERE USUARIO_USUARIO = 'CLIENTE2';

/* Asigna a cada cuenta un numero unico, consecutivo y de 10 digitos. */
;WITH cte AS (
    SELECT CUENTA_ID, ROW_NUMBER() OVER (ORDER BY CUENTA_ID) + 1000000000 AS n
    FROM dbo.CUENTA
    WHERE CUENTA_NUMERO IS NULL
)
UPDATE c
SET CUENTA_NUMERO = RIGHT('0000000000' + CAST(cte.n AS VARCHAR(10)), 10)
FROM dbo.CUENTA c
INNER JOIN cte ON c.CUENTA_ID = cte.CUENTA_ID;

/* Protege el contrato: numero obligatorio y unico. */
ALTER TABLE dbo.CUENTA ALTER COLUMN CUENTA_NUMERO VARCHAR(10) NOT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UK_CUENTA_NUMERO' AND parent_object_id = OBJECT_ID('dbo.CUENTA'))
BEGIN
    ALTER TABLE dbo.CUENTA ADD CONSTRAINT UK_CUENTA_NUMERO UNIQUE (CUENTA_NUMERO);
END
GO
