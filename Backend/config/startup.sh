#!/bin/bash
set -e

echo "Starting PostgreSQL..."
docker-entrypoint.sh postgres \
  -c hba_file=/var/etc/postgresql/custom_hba.conf \
  -c wal_level=replica \
  -c hot_standby=on \
  -c max_wal_senders=10 \
  -c max_replication_slots=10 \
  -c hot_standby_feedback=on &

# Wait for Postgres to be ready
echo "Waiting for PostgreSQL to accept connections..."
until pg_isready -U postgres -h localhost; do
  sleep 1
done

echo "Running startup SQL..."

psql -U postgres -d postgres <<'EOSQL'
ALTER USER postgres ENCRYPTED PASSWORD 'example';

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_replication_slots WHERE slot_name = 'ups'
  ) THEN
    PERFORM pg_create_physical_replication_slot('ups');
  END IF;
END
$$;
EOSQL

echo "Startup SQL completed."

# Bring Postgres back to foreground
wait
