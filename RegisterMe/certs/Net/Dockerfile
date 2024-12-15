#https://dev.to/danvixent/how-to-setup-postgresql-with-ssl-inside-a-docker-container-5f3

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

RUN apt-get update && apt-get upgrade -y

RUN mkdir -p /certificates

WORKDIR /certificates

COPY NetEntryPoint.sh /NetEntryPoint.sh
COPY openssl.cnf /openssl.cnf
