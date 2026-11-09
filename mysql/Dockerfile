# Dockerfile para MySQL 9 (Render / local)
FROM mysql:9

# Copia los scripts de inicializacion (se ejecutan solo si el data dir esta vacio)
COPY 01-modelo.sql /docker-entrypoint-initdb.d/01-modelo.sql
COPY 02-datos.sql  /docker-entrypoint-initdb.d/02-datos.sql

# Variables por defecto (sobrescribibles en Render)
ENV MYSQL_ROOT_PASSWORD=admin \
    MYSQL_DATABASE=arquitectura_db \
    MYSQL_USER=MONSTER \
    MYSQL_PASSWORD=MONSTER9

EXPOSE 3306

HEALTHCHECK --interval=10s --timeout=5s --retries=10 --start-period=30s \
  CMD mysqladmin ping -h 127.0.0.1 -uroot -p"$MYSQL_ROOT_PASSWORD" --silent
