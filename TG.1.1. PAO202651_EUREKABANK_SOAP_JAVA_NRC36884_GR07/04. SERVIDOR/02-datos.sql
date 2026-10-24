/*==============================================================*/
/* EUREKABANK - 02. DATOS INICIALES                             */
/* Requiere haber ejecutado 01-modelo.sql                       */
/* Motor: MySQL 9                                               */
/*==============================================================*/

use arquitectura_db;

/* Roles (unicos) */
insert into ROL (ROL_ID, ROL_NOMBRE) values
   ('a87b1b30-dede-473d-8ebd-fc397c6cd2be', 'ADMIN'),
   ('5bd2b9c5-37e3-47ea-b1f9-4fea40a5e841', 'CLIENTE'),
   ('7ffd5c6a-4903-49cc-a0c9-a64a24b5c5c7', 'CAJERO');

/* Sucursales */
insert into SUCURSAL (SUCURSAL_ID, SUCURSAL_NOMBRE) values
   ('c3ff12d2-b6b9-4657-90f4-5c63d49841b3', 'Sucursal Quito Norte'),
   ('a55fbee6-fe6a-4d82-809a-381787ccaade', 'Sucursal Guayaquil Centro'),
   ('33039bda-e9a6-4e0a-9b39-823863425d3c', 'Sucursal Cuenca El Batan');

/* Usuarios: administrador de aplicacion y dos clientes */
insert into USUARIO (USUARIO_ID, ROL_ID, USUARIO_NOMBRE, USUARIO_APELLIDO,
                     USUARIO_TELEFONO, USUARIO_USUARIO, USUARIO_CONTRASEANIA) values
   ('77874037-b936-41bf-9195-2eb6e4267bd6', 'a87b1b30-dede-473d-8ebd-fc397c6cd2be',
    'MONSTER', 'ADMINISTRADOR', '0999999999', 'MONSTER', 'MONSTER9'),
   ('6ab15134-c789-48dd-bd57-53bc23611d04', '5bd2b9c5-37e3-47ea-b1f9-4fea40a5e841',
    'MATIAS SEBASTIAN', 'TORRES VILLACIS', '0998123456', 'CLIENTE1', 'Cliente1*2026'),
   ('a75ead9c-db78-49ed-a41d-0b059c1af6fb', '5bd2b9c5-37e3-47ea-b1f9-4fea40a5e841',
    'VALENTINA SOFIA', 'RAMIREZ LEON', '0987456123', 'CLIENTE2', 'Cliente2*2026');

/* Cuentas: una propia por cliente y una compartida */
insert into CUENTA (CUENTA_ID, SUCURSAL_ID, CUENTA_SALDO, CUENTA_ESTADO) values
   ('1861a565-3b19-4efe-baa9-e8f964578e5f', 'c3ff12d2-b6b9-4657-90f4-5c63d49841b3',
    1250.75, 'ACTIVA'),
   ('f3ec0bbd-ecb5-4fc4-95f5-aeebb328c1d2', 'a55fbee6-fe6a-4d82-809a-381787ccaade',
    2480.30, 'ACTIVA'),
   ('6a224226-10d3-4de9-b8dc-2215ce346c03', '33039bda-e9a6-4e0a-9b39-823863425d3c',
    5000.00, 'ACTIVA');

/* Propiedad de cuentas: cliente1 -> cuenta propia, cliente2 -> cuenta propia,
   y ambos clientes -> cuenta compartida */
insert into PERTENECE (CUENTA_ID, USUARIO_ID) values
   ('1861a565-3b19-4efe-baa9-e8f964578e5f', '6ab15134-c789-48dd-bd57-53bc23611d04'),
   ('f3ec0bbd-ecb5-4fc4-95f5-aeebb328c1d2', 'a75ead9c-db78-49ed-a41d-0b059c1af6fb'),
   ('6a224226-10d3-4de9-b8dc-2215ce346c03', '6ab15134-c789-48dd-bd57-53bc23611d04'),
   ('6a224226-10d3-4de9-b8dc-2215ce346c03', 'a75ead9c-db78-49ed-a41d-0b059c1af6fb');
