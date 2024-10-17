#!/bin/ash

cd pg_dump/backups/ || { echo "Failed to change directory to pg_dump/backups/"; exit 1; }

newest_file=$(ls -t *.sql | head -n 1)

if [ -z "$newest_file" ]; then
  echo "No SQL backup file found!"
  exit 1
fi

echo "File selected: $newest_file"

DB_NAME="${1:-RegisterMeDb}"
PSQL_PATH="/Applications/Postgres.app/Contents/Versions/latest/bin/psql"

$PSQL_PATH -c "DROP DATABASE IF EXISTS \"$DB_NAME\";" || { echo "Failed to drop database $DB_NAME"; exit 1; }

$PSQL_PATH -c "CREATE DATABASE \"$DB_NAME\";" || { echo "Failed to create database $DB_NAME"; exit 1; }

for role in "neon_superuser" "cloud_admin"; do
  if $PSQL_PATH -tAc "SELECT 1 FROM pg_roles WHERE rolname='$role'" | grep -q 1; then
    echo "Role '$role' already exists."
  else
    $PSQL_PATH -c "CREATE ROLE \"$role\" WITH LOGIN SUPERUSER;" || { echo "Failed to create role $role"; exit 1; }
  fi
done

$PSQL_PATH -d "$DB_NAME" -U ondrejman -p 5432 -h localhost -f "$newest_file" || { echo "Failed to restore database from $newest_file"; exit 1; }

echo "Database restored from $newest_file to database $DB_NAME"