/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     31/10/2026 22:05:39                          */
/*==============================================================*/


if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('AMORTIZACION') and o.name = 'FK_AMORTIZA_PAGA_CREDITO')
alter table AMORTIZACION
   drop constraint FK_AMORTIZA_PAGA_CREDITO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CREDITO') and o.name = 'FK_CREDITO_PIDE_CUENTA')
alter table CREDITO
   drop constraint FK_CREDITO_PIDE_CUENTA
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CUENTA') and o.name = 'FK_CUENTA_TIENE_CLIENTE')
alter table CUENTA
   drop constraint FK_CUENTA_TIENE_CLIENTE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('MOVIMIENTO') and o.name = 'FK_MOVIMIEN_REALIZA_CUENTA')
alter table MOVIMIENTO
   drop constraint FK_MOVIMIEN_REALIZA_CUENTA
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('AMORTIZACION')
            and   name  = 'PAGA_FK'
            and   indid > 0
            and   indid < 255)
   drop index AMORTIZACION.PAGA_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('AMORTIZACION')
            and   type = 'U')
   drop table AMORTIZACION
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CLIENTE')
            and   type = 'U')
   drop table CLIENTE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('CREDITO')
            and   name  = 'PIDE_FK'
            and   indid > 0
            and   indid < 255)
   drop index CREDITO.PIDE_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CREDITO')
            and   type = 'U')
   drop table CREDITO
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('CUENTA')
            and   name  = 'TIENE_FK'
            and   indid > 0
            and   indid < 255)
   drop index CUENTA.TIENE_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CUENTA')
            and   type = 'U')
   drop table CUENTA
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('MOVIMIENTO')
            and   name  = 'REALIZA_FK'
            and   indid > 0
            and   indid < 255)
   drop index MOVIMIENTO.REALIZA_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('MOVIMIENTO')
            and   type = 'U')
   drop table MOVIMIENTO
go

/*==============================================================*/
/* Table: AMORTIZACION                                          */
/*==============================================================*/
create table AMORTIZACION (
   NUM_CUOTA            int                  not null,
   NUM_CREDITO          int                  not null,
   VALOR_CUOTA          decimal(10,2)        not null,
   INTERES_PAGADO       decimal(4,2)         not null,
   CAPITAL_PAGADO       decimal(10,2)        not null,
   SALDO                decimal(10,2)        not null,
   FECHA_PAGO           datetime             not null,
   PAGADA               bit                  not null
      constraint DF_AMORTIZACION_PAGADA default 0,
   constraint PK_AMORTIZACION primary key nonclustered (NUM_CUOTA, NUM_CREDITO)
)
go

/*==============================================================*/
/* Index: PAGA_FK                                               */
/*==============================================================*/
create index PAGA_FK on AMORTIZACION (
NUM_CREDITO ASC
)
go

/*==============================================================*/
/* Table: CLIENTE                                               */
/*==============================================================*/
create table CLIENTE (
   COD_CLIENTE          int                  not null,
   CEDULA               varchar(100)         not null,
   NOMBRE               varchar(100)         not null,
   GENERO               varchar(100)         not null,
   FECHA_NACIMIENTO     datetime             not null,
   constraint PK_CLIENTE primary key nonclustered (COD_CLIENTE)
)
go

/*==============================================================*/
/* Table: CREDITO                                               */
/*==============================================================*/
create table CREDITO (
   NUM_CREDITO          int                  not null,
   NUM_CUENTA           varchar(8)           not null,
   CREDITO_TOTAL        decimal(10,2)        not null,
   FECHA_CREACION       datetime             not null,
   FECHA_ACTUALIZACION  datetime             not null,
   constraint PK_CREDITO primary key nonclustered (NUM_CREDITO)
)
go

/*==============================================================*/
/* Index: PIDE_FK                                               */
/*==============================================================*/
create index PIDE_FK on CREDITO (
NUM_CUENTA ASC
)
go

/*==============================================================*/
/* Table: CUENTA                                                */
/*==============================================================*/
create table CUENTA (
   NUM_CUENTA           varchar(8)           not null,
   COD_CLIENTE          int                  not null,
   SALDO_CUENTA         decimal(10,2)        not null,
   constraint PK_CUENTA primary key nonclustered (NUM_CUENTA)
)
go

/*==============================================================*/
/* Index: TIENE_FK                                              */
/*==============================================================*/
create index TIENE_FK on CUENTA (
COD_CLIENTE ASC
)
go

/*==============================================================*/
/* Table: MOVIMIENTO                                            */
/*==============================================================*/
create table MOVIMIENTO (
   COD_MOVIMIENTO       int                  not null,
   NUM_CUENTA           varchar(8)           not null,
   TIPO                 varchar(3)           not null,
   VALOR                decimal(10,2)        not null,
   FECHA                datetime             not null,
   constraint PK_MOVIMIENTO primary key nonclustered (COD_MOVIMIENTO)
)
go

/*==============================================================*/
/* Index: REALIZA_FK                                            */
/*==============================================================*/
create index REALIZA_FK on MOVIMIENTO (
NUM_CUENTA ASC
)
go

alter table AMORTIZACION
   add constraint FK_AMORTIZA_PAGA_CREDITO foreign key (NUM_CREDITO)
      references CREDITO (NUM_CREDITO)
go

alter table CREDITO
   add constraint FK_CREDITO_PIDE_CUENTA foreign key (NUM_CUENTA)
      references CUENTA (NUM_CUENTA)
go

alter table CUENTA
   add constraint FK_CUENTA_TIENE_CLIENTE foreign key (COD_CLIENTE)
      references CLIENTE (COD_CLIENTE)
go

alter table MOVIMIENTO
   add constraint FK_MOVIMIEN_REALIZA_CUENTA foreign key (NUM_CUENTA)
      references CUENTA (NUM_CUENTA)
go

