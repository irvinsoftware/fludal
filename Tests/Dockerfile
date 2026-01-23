FROM mcr.microsoft.com/mssql/server:2022-CU13-ubuntu-22.04

ENV ACCEPT_EULA=Y

COPY CreateDatabase.sql /tmp
COPY startup.sh /tmp

USER root
RUN apt-get update
RUN apt-get install sqlcmd
RUN chmod +x /tmp/startup.sh
USER mssql

CMD ["./tmp/startup.sh"] 
