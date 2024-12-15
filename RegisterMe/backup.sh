#!/bin/bash

cd pg_dump
docker-compose --env-file ../.env.production up --build 