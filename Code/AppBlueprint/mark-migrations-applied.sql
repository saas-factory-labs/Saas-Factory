-- Simple fix: Just mark migrations as applied in __EFMigrationsHistory
-- Run this in your PostgreSQL database (pgAdmin, DBeaver, etc.)

-- Mark both migrations as applied
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES 
    ('20260202085124_AddFileMetadataEntity', '10.0.1'),
    ('20260203071153_AddFullTextSearchVectors', '10.0.1')
ON CONFLICT DO NOTHING;

-- Verify
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";
