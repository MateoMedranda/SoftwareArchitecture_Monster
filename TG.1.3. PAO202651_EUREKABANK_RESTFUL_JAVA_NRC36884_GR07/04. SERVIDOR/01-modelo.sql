/*==============================================================*/
/* EUREKABANK - 01. MODELO                                      */
/* Base de datos, usuarios y esquema (MER)                      */
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
   USUARIO_CONTRASEANIA varchar(64) not null,
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
   TOKEN_CREADO_EN      datetime not null default current_timestamp,
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
   TRANSACCION_CREADO_EN datetime not null,
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
