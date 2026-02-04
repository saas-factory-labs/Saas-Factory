-- Auto-generated idempotent migration script from EF Core
-- This will safely apply all pending migrations without errors

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260203071153_AddFullTextSearchVectors') THEN
    ALTER TABLE "Users" ADD "SearchVector" character varying(1024) GENERATED ALWAYS AS (to_tsvector('english', coalesce("FirstName", '') || ' ' || coalesce("LastName", '') || ' ' || coalesce("UserName", '') || ' ' || coalesce("Email", ''))) STORED;
    COMMENT ON COLUMN "Users"."SearchVector" IS 'Full-text search vector for user search';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260203071153_AddFullTextSearchVectors') THEN
    ALTER TABLE "Tenants" ADD "SearchVector" character varying(1024) GENERATED ALWAYS AS (to_tsvector('english', coalesce("Name", '') || ' ' || coalesce("Description", '') || ' ' || coalesce("Email", '') || ' ' || coalesce("VatNumber", ''))) STORED;
    COMMENT ON COLUMN "Tenants"."SearchVector" IS 'Full-text search vector for tenant search';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260203071153_AddFullTextSearchVectors') THEN
    CREATE INDEX "IX_Users_SearchVector" ON "Users" USING GIN ("SearchVector");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260203071153_AddFullTextSearchVectors') THEN
    CREATE INDEX "IX_Tenants_SearchVector" ON "Tenants" USING GIN ("SearchVector");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260203071153_AddFullTextSearchVectors') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260203071153_AddFullTextSearchVectors', '10.0.1');
    END IF;
END $EF$;

-- Also mark the previous migration as applied (since tables already exist)
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260202085124_AddFileMetadataEntity') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260202085124_AddFileMetadataEntity', '10.0.1');
    END IF;
END $EF$;

COMMIT;

-- Verify SearchVector columns were added
SELECT 
    'Users' as table_name,
    column_name,
    data_type
FROM information_schema.columns
WHERE table_name = 'Users' AND column_name = 'SearchVector'
UNION ALL
SELECT 
    'Tenants' as table_name,
    column_name,
    data_type
FROM information_schema.columns
WHERE table_name = 'Tenants' AND column_name = 'SearchVector';
