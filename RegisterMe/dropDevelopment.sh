#!/bin/bash

cd ..
/usr/local/share/dotnet/dotnet ef database drop --project RegisterMe/src/DAL.Migrations.Postgres/DAL.Migrations.Postgres.csproj --startup-project RegisterMe/src/WebGui/WebGui.csproj --context RegisterMe.Infrastructure.Data.ApplicationDbContext --configuration Release --no-build --force  -- --environment Development