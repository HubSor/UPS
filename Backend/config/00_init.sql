alter user postgres encrypted password 'example';
grant all privileges on database example to postgres;
SELECT pg_create_physical_replication_slot('ups');