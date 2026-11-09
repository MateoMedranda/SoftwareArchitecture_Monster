# Dockerfile para SQL Server 2022 (Render tier pago / local)
FROM mcr.microsoft.com/mssql/server:2022-latest

ENV ACCEPT_EULA=Y \
    MSSQL_PID=Developer

USER root
RUN mkdir -p /opt/mssql/init && chown -R mssql:mssql /opt/mssql/init
COPY 01-modelo.sql /opt/mssql/init/01-modelo.sql
COPY 02-datos.sql  /opt/mssql/init/02-datos.sql
COPY entrypoint.sh /opt/mssql/init/entrypoint.sh
RUN chmod +x /opt/mssql/init/entrypoint.sh && chown -R mssql:mssql /opt/mssql/init

USER mssql
EXPOSE 1433

ENTRYPOINT ["/opt/mssql/init/entrypoint.sh"]
CMD ["/opt/mssql/bin/sqlservr"]
