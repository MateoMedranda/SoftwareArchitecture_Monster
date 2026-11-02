#!/usr/bin/env bash
# ============================================================
# Inicializa las bases de datos BANQUITO y FACTURACION
# en el contenedor SQL Server "sistema_banquito".
# Se ejecuta una sola vez (contenedor de inicializacion).
# ============================================================
set -euo pipefail

SQLCMD=/opt/mssql-tools18/bin/sqlcmd
if [ ! -x "$SQLCMD" ]; then
    SQLCMD=sqlcmd
fi

SERVIDOR="${SQL_SERVER_HOST:-sistema_banquito}"
USUARIO="${SQL_USER:-sa}"
CLAVE="${MSSQL_SA_PASSWORD:?Falta la variable MSSQL_SA_PASSWORD}"

esperar_servidor() {
    echo "Esperando a que SQL Server ($SERVIDOR) acepte conexiones..."
    until "$SQLCMD" -C -S "$SERVIDOR" -U "$USUARIO" -P "$CLAVE" -Q "SELECT 1" -b -o /dev/null; do
        sleep 3
    done
}

crear_base() {
    local nombre="$1"
    echo "Creando la base de datos $nombre si no existe..."
    "$SQLCMD" -C -S "$SERVIDOR" -U "$USUARIO" -P "$CLAVE" -b \
        -Q "IF DB_ID('$nombre') IS NULL EXEC('CREATE DATABASE [$nombre]')"
}

inicializar_base() {
    local nombre="$1"
    local esquema="$2"
    local datos="$3"

    crear_base "$nombre"

    local tablas
    tablas=$("$SQLCMD" -C -S "$SERVIDOR" -U "$USUARIO" -P "$CLAVE" \
        -d "$nombre" -h -1 -W -f 65001 \
        -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.tables")

    if [ "$tablas" = "0" ]; then
        echo "Cargando esquema de $nombre ($esquema)..."
        "$SQLCMD" -C -S "$SERVIDOR" -U "$USUARIO" -P "$CLAVE" \
            -d "$nombre" -b -f 65001 -i "/scripts/$esquema"

        echo "Cargando datos de prueba de $nombre ($datos)..."
        "$SQLCMD" -C -S "$SERVIDOR" -U "$USUARIO" -P "$CLAVE" \
            -d "$nombre" -b -f 65001 -i "/scripts/$datos"

        echo "Base de datos $nombre inicializada correctamente."
    else
        echo "La base de datos $nombre ya contiene tablas; se omite la inicializacion."
    fi
}

esperar_servidor
inicializar_base BANQUITO BANQUITO.sql BANQUITO_DATA.sql
inicializar_base FACTURACION Facturacion.sql Facturacion_DATA.sql
