-- Create the database if it doesn't exist
SELECT 'CREATE DATABASE appblueprint'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'appblueprint');

\c appblueprint;

-- Create extension if it doesn't exist
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create schemas
CREATE SCHEMA IF NOT EXISTS auth;
CREATE SCHEMA IF NOT EXISTS public;

-- Grant privileges
GRANT ALL ON SCHEMA public TO dev_user;
GRANT ALL ON SCHEMA auth TO dev_user;

-- Set search path
ALTER DATABASE appblueprint SET search_path TO public, auth;