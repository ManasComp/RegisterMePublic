#!/bin/bash

read -p "Are you sure you want to drop and recreate the database? (y/n): " confirm


if [[ $confirm == "y" || $confirm == "Y" ]]; then
    echo "Dropping the database..."
    
    cd ..

    /usr/local/share/dotnet/dotnet ef database drop --project RegisterMe/src/DAL.Migrations.Postgres/DAL.Migrations.Postgres.csproj --startup-project RegisterMe/src/WebGui/WebGui.csproj --context RegisterMe.Infrastructure.Data.ApplicationDbContext --configuration Release --no-build --force -- --environment Production

    echo "Creating the new database..."
    
    neonctl databases create --name RegisterMe

    echo "Database recreated successfully!"
else
    echo "Operation cancelled."
fi