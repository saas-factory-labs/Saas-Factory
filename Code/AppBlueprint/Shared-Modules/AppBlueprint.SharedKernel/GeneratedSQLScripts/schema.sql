CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory"
(
    "MigrationId"
    character
    varying
(
    150
) NOT NULL,
    "ProductVersion" character varying
(
    32
) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY
(
    "MigrationId"
)
    );

