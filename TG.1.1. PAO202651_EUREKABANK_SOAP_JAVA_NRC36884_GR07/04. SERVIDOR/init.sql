/*==============================================================*/
/* EUREKABANK - Script de inicializacion (esquema + datos)      */
/* Motor: MySQL 9                                               */
/*                                                              */
/* Usuarios creados:                                            */
/*   MONSTER / MONSTER9  -> usuario de aplicacion (Payara)      */
/*   admin   / admin     -> usuario administrador               */
/*   root    / admin     -> creado por la imagen de Docker      */
/*==============================================================*/

/*--------------------------------------------------------------*/
/* 1. Base de datos                                             */
/*--------------------------------------------------------------*/
create database if not exists arquitectura_db
   default character set utf8mb4
   collate utf8mb4_unicode_ci;

use arquitectura_db;

/*--------------------------------------------------------------*/
/* 2. Usuarios y privilegios                                    */
/*--------------------------------------------------------------*/
create user if not exists 'MONSTER'@'%' identified by 'MONSTER9';
alter user 'MONSTER'@'%' identified by 'MONSTER9';
grant all privileges on `arquitectura_db`.* to 'MONSTER'@'%';

create user if not exists 'admin'@'%' identified by 'admin';
alter user 'admin'@'%' identified by 'admin';
grant all privileges on *.* to 'admin'@'%' with grant option;

flush privileges;

/*--------------------------------------------------------------*/
/* 3. Esquema (MER EUREKABANK)                                  */
/*--------------------------------------------------------------*/
set foreign_key_checks = 0;

drop table if exists PERTENECE;
drop table if exists TRANSACCION;
drop table if exists TOKEN;
drop table if exists CUENTA;
drop table if exists USUARIO;
drop table if exists SUCURSAL;
drop table if exists ROL;

set foreign_key_checks = 1;

/*==============================================================*/
/* Table: ROL                                                   */
/*==============================================================*/
create table ROL
(
   ROL_ID               varchar(36) not null,
   ROL_NOMBRE           varchar(36) not null,
   primary key (ROL_ID),
   unique key UK_ROL_NOMBRE (ROL_NOMBRE)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_unicode_ci;

/*==============================================================*/
/* Table: SUCURSAL                                              */
/*==============================================================*/
create table SUCURSAL
(
   SUCURSAL_ID          varchar(36) not null,
   SUCURSAL_NOMBRE      varchar(64) not null,
   primary key (SUCURSAL_ID)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_unicode_ci;

/*==============================================================*/
/* Table: USUARIO                                               */
/*==============================================================*/
create table USUARIO
(
   USUARIO_NOMBRE       varchar(36) not null,
   USUARIO_ID           varchar(36) not null,
   ROL_ID               varchar(36),
   USUARIO_APELLIDO     varchar(36) not null,
   USUARIO_TELEFONO     varchar(36) not null,
   USUARIO_USUARIO      varchar(36) not null,
   USUARIO_CONTRASEANIA varchar(36) not null,
   primary key (USUARIO_ID),
   unique key UK_USUARIO_USUARIO (USUARIO_USUARIO)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_unicode_ci;

/*==============================================================*/
/* Table: CUENTA                                                */
/*==============================================================*/
create table CUENTA
(
   CUENTA_SALDO         decimal(15,2) not null,
   CUENTA_ESTADO        varchar(24) not null,
   CUENTA_ID            varchar(36) not null,
   SUCURSAL_ID          varchar(36),
   primary key (CUENTA_ID)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_unicode_ci;

/*==============================================================*/
/* Table: PERTENECE                                             */
/*==============================================================*/
create table PERTENECE
(
   CUENTA_ID            varchar(36) not null,
   USUARIO_ID           varchar(36) not null,
   primary key (CUENTA_ID, USUARIO_ID)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_unicode_ci;

/*==============================================================*/
/* Table: TOKEN                                                 */
/*==============================================================*/
create table TOKEN
(
   TOKEN_TOKEN          varchar(124) not null,
   TOKEN_ESTADO         varchar(24) not null,
   TOKEN_ID             varchar(36) not null,
   USUARIO_ID           varchar(36) not null,
   primary key (TOKEN_ID)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_unicode_ci;

/*==============================================================*/
/* Table: TRANSACCION                                           */
/*==============================================================*/
create table TRANSACCION
(
   TRANSACCION_ID       varchar(36) not null,
   CUENTA_ID            varchar(36) not null,
   TRANSACCION_SALDO    decimal(10,2) not null,
   TRANSACCION_TIPO     varchar(36) not null,
   TRANSACCION_SALDO_ANTERIOR decimal(12,2) not null,
   TRANSACCION_SALDO_POSTERIOR decimal(12,2) not null,
   TRANSACCION_CREADO_EN_ datetime not null,
   TRANSACCION_CREADO_POR varchar(36) not null,
   primary key (TRANSACCION_ID)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_unicode_ci;

/*==============================================================*/
/* Foreign keys                                                 */
/*==============================================================*/
alter table CUENTA add constraint FK_TIENE foreign key (SUCURSAL_ID)
      references SUCURSAL (SUCURSAL_ID) on delete restrict on update restrict;

alter table PERTENECE add constraint FK_PERTENECE foreign key (CUENTA_ID)
      references CUENTA (CUENTA_ID) on delete restrict on update restrict;

alter table PERTENECE add constraint FK_PERTENECE2 foreign key (USUARIO_ID)
      references USUARIO (USUARIO_ID) on delete restrict on update restrict;

alter table TOKEN add constraint FK_TIENE_ASIGNADO foreign key (USUARIO_ID)
      references USUARIO (USUARIO_ID) on delete restrict on update restrict;

alter table TRANSACCION add constraint FK_GENERA foreign key (CUENTA_ID)
      references CUENTA (CUENTA_ID) on delete restrict on update restrict;

alter table USUARIO add constraint FK_PUEDE_SER foreign key (ROL_ID)
      references ROL (ROL_ID) on delete restrict on update restrict;

/*--------------------------------------------------------------*/
/* 4. Datos iniciales                                           */
/*--------------------------------------------------------------*/

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
