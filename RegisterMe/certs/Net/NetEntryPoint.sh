#!/bin/bash

echo ${APP_CERTIFICATE_PFX}

cd /certificates
set -e
dotnet dev-certs https -ep "${APP_CERTIFICATE_PFX}" -p "${PASSWORD}"
openssl genrsa -aes256 -passout pass:"${PASSWORD}" -out "${POSTGRESDB_KEY}" 2048
openssl rsa -in "${POSTGRESDB_KEY}" -out "${POSTGRESDB_KEY}" -passout pass:"${PASSWORD}" -passin pass:"${PASSWORD}"
openssl req -new -key "${POSTGRESDB_KEY}" -days 365 -out "${POSTGRESDB_CRT}" -x509 -config ../openssl.cnf
cp "${POSTGRESDB_CRT}" "${ROOT_CRT}"