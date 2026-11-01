/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     31/10/2026 22:06:08                          */
/*==============================================================*/


if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FACTURA') and o.name = 'FK_FACTURA_GENERA_CLIENTE')
alter table FACTURA
   drop constraint FK_FACTURA_GENERA_CLIENTE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ITEM_FACTURA') and o.name = 'FK_ITEM_FAC_CONTIENE_ITEM_INV')
alter table ITEM_FACTURA
   drop constraint FK_ITEM_FAC_CONTIENE_ITEM_INV
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ITEM_FACTURA') and o.name = 'FK_ITEM_FAC_MANEJA_FACTURA')
alter table ITEM_FACTURA
   drop constraint FK_ITEM_FAC_MANEJA_FACTURA
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CLIENTE')
            and   type = 'U')
   drop table CLIENTE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('FACTURA')
            and   name  = 'GENERA_FK'
            and   indid > 0
            and   indid < 255)
   drop index FACTURA.GENERA_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('FACTURA')
            and   type = 'U')
   drop table FACTURA
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ITEM_FACTURA')
            and   name  = 'CONTIENE_FK'
            and   indid > 0
            and   indid < 255)
   drop index ITEM_FACTURA.CONTIENE_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ITEM_FACTURA')
            and   name  = 'MANEJA_FK'
            and   indid > 0
            and   indid < 255)
   drop index ITEM_FACTURA.MANEJA_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ITEM_FACTURA')
            and   type = 'U')
   drop table ITEM_FACTURA
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ITEM_INVENTARIO')
            and   type = 'U')
   drop table ITEM_INVENTARIO
go

/*==============================================================*/
/* Table: CLIENTE                                               */
/*==============================================================*/
create table CLIENTE (
   COD_CLIENTE          int                  not null,
   CEDULA               varchar(100)         not null,
   NOMBRE               varchar(100)         not null,
   constraint PK_CLIENTE primary key nonclustered (COD_CLIENTE)
)
go

/*==============================================================*/
/* Table: FACTURA                                               */
/*==============================================================*/
create table FACTURA (
   NUM_FACTURA          int                  not null,
   COD_CLIENTE          int                  not null,
   FECHA                datetime             not null,
   TIPO_PAGO            varchar(16)          not null,
   VALOR                decimal(10,2)        not null,
   constraint PK_FACTURA primary key nonclustered (NUM_FACTURA)
)
go

/*==============================================================*/
/* Index: GENERA_FK                                             */
/*==============================================================*/
create index GENERA_FK on FACTURA (
COD_CLIENTE ASC
)
go

/*==============================================================*/
/* Table: ITEM_FACTURA                                          */
/*==============================================================*/
create table ITEM_FACTURA (
   NUM_ITEM             int                  not null,
   NUM_FACTURA          int                  not null,
   NUM_INVENTARIO       int                  not null,
   VALOR                decimal(10,2)        not null,
   CANTIDAD             int                  not null default 1,
   constraint PK_ITEM_FACTURA primary key nonclustered (NUM_ITEM)
)
go

/*==============================================================*/
/* Index: MANEJA_FK                                             */
/*==============================================================*/
create index MANEJA_FK on ITEM_FACTURA (
NUM_FACTURA ASC
)
go

/*==============================================================*/
/* Index: CONTIENE_FK                                           */
/*==============================================================*/
create index CONTIENE_FK on ITEM_FACTURA (
NUM_INVENTARIO ASC
)
go

/*==============================================================*/
/* Table: ITEM_INVENTARIO                                       */
/*==============================================================*/
create table ITEM_INVENTARIO (
   NUM_INVENTARIO       int                  not null,
   NOMBRE               varchar(100)         not null,
   PRECIO               decimal(10,2)        not null,
   constraint PK_ITEM_INVENTARIO primary key nonclustered (NUM_INVENTARIO)
)
go

alter table FACTURA
   add constraint FK_FACTURA_GENERA_CLIENTE foreign key (COD_CLIENTE)
      references CLIENTE (COD_CLIENTE)
go

alter table ITEM_FACTURA
   add constraint FK_ITEM_FAC_CONTIENE_ITEM_INV foreign key (NUM_INVENTARIO)
      references ITEM_INVENTARIO (NUM_INVENTARIO)
go

alter table ITEM_FACTURA
   add constraint FK_ITEM_FAC_MANEJA_FACTURA foreign key (NUM_FACTURA)
      references FACTURA (NUM_FACTURA)
go

