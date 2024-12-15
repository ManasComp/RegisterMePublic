#!/bin/bash

POSTGRES_USER="RegisterMeAdmin"
POSTGRES_PASSWORD="2024.REgisterM3"
POSTGRES_DB="RegisterMe"
POSTGRES_PORT=5434
POSTGRES_IMAGE="postgres:16"
IS_DEVELOPMENT=true

docker run -d \
  -e POSTGRES_USER=$POSTGRES_USER \
  -e POSTGRES_PASSWORD=$POSTGRES_PASSWORD \
  -e POSTGRES_DB=$POSTGRES_DB \
  -p $POSTGRES_PORT:5432 \
  $POSTGRES_IMAGE \

CONNECTION_STRING="User ID=$POSTGRES_USER;Password=$POSTGRES_PASSWORD;Host=localhost;Port=$POSTGRES_PORT;Database=$POSTGRES_DB;TrustServerCertificate=true;Include Error Detail=$IS_DEVELOPMENT;"

echo "ConnectionString: \"$CONNECTION_STRING\""