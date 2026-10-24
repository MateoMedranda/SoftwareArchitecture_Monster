/*==============================================================*/
/* DBMS name:      MySQL 5.0                                    */
/* Created on:     23/10/2026 20:36:16                          */
/*==============================================================*/


drop table if exists CUENTA;

drop table if exists PERTENECE;

drop table if exists ROL;

drop table if exists SUCURSAL;

drop table if exists TOKEN;

drop table if exists TRANSACCION;

drop table if exists USUARIO;

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
);

/*==============================================================*/
/* Table: PERTENECE                                             */
/*==============================================================*/
create table PERTENECE
(
   CUENTA_ID            varchar(36) not null,
   USUARIO_ID           varchar(36) not null,
   primary key (CUENTA_ID, USUARIO_ID)
);

/*==============================================================*/
/* Table: ROL                                                   */
/*==============================================================*/
create table ROL
(
   ROL_ID               varchar(36) not null,
   ROL_NOMBRE           varchar(36) not null,
   primary key (ROL_ID)
);

/*==============================================================*/
/* Table: SUCURSAL                                              */
/*==============================================================*/
create table SUCURSAL
(
   SUCURSAL_ID          varchar(36) not null,
   SUCURSAL_NOMBRE      varchar(64) not null,
   primary key (SUCURSAL_ID)
);

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
);

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
);

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
   primary key (USUARIO_ID)
);

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

