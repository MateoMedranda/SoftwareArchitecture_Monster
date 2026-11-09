#!/bin/bash
set -e

SA_PASSWORD="${MSSQL_SA_PASSWORD:-${SA_PASSWORD:-StrongPass!2026}}"
export SQLCMDPASSWORD="$SA_PASSWORD"

SQLCMD=""
for p in /opt/mssql-tools18/bin/sqlcmd /opt/mssql-tools/bin/sqlcmd; do
    if [ -x "$p" ]; then
        SQLCMD="$p"
        break
    fi
done
if [ -z "$SQLCMD" ]; then
    echo "sqlcmd not found in image" >&2
    exit 1
fi

# Arranca SQL Server en segundo plano
/opt/mssql/bin/sqlservr &
SQLPID=$!

echo "Esperando a que SQL Server acepte conexiones..."
for i in $(seq 1 60); do
    if $SQLCMD -S localhost -U sa -C -Q "SELECT 1" >/dev/null 2>&1; then
        echo "SQL Server listo."
        break
    fi
    sleep 2
done

# Ejecuta los scripts solo si la BD/tabla USUARIO no existe (idempotente)
TABLES=$($SQLCMD -S localhost -U sa -d arquitectura_db -C -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='USUARIO';" 2>/dev/null | tr -d '[:space:]' || echo 0)
if [ "$TABLES" = "0" ] || [ -z "$TABLES" ]; then
    echo "Ejecutando 01-modelo.sql (modelo)..."
    $SQLCMD -S localhost -U sa -C -i /opt/mssql/init/01-modelo.sql
    echo "Ejecutando 02-datos.sql (datos)..."
    $SQLCMD -S localhost -U sa -C -i /opt/mssql/init/02-datos.sql
    echo "Inicializacion completada."
else
    echo "La base de datos ya esta inicializada; se omiten los scripts."
fi

wait $SQLPID
