#!/bin/sh

/opt/mssql/bin/sqlservr &
sqlpid=$!

while ! cat /var/opt/mssql/log/errorlog | grep "The tempdb database has" > /dev/null
do
  echo Waiting for DB startup
  sleep 5
done

sqlcmd -U sa -P $MSSQL_SA_PASSWORD -i /tmp/CreateDatabase.sql

wait $sqlpid
