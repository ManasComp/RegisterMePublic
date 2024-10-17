#https://dev.to/danvixent/how-to-setup-postgresql-with-ssl-inside-a-docker-container-5f3

FROM golang:1.23 AS base

RUN apt-get update && apt-get upgrade -y

RUN git clone https://github.com/square/certstrap.git /go/src/certstrap

WORKDIR /go/src/certstrap
RUN go build

RUN mv certstrap /usr/local/bin/

RUN mkdir -p /certificates

COPY GoEntryPoint.sh /GoEntryPoint.sh
