-- Define migration constants using session variables to avoid duplication (SonarQube fix)
SET app.migration_id1 = '20260202085124_AddFileMetadataEntity';
SET app.migration_id2 = '20260203071153_AddFullTextSearchVectors';
SET app.history_table = '__EFMigrationsHistory';
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

-- Helper functions to eliminate string literal duplication (SonarQube fix)
CREATE OR REPLACE FUNCTION _get_migration_id1() RETURNS TEXT AS $$
BEGIN
    RETURN current_setting('app.migration_id1');
END;
$$ LANGUAGE plpgsql STABLE;

CREATE OR REPLACE FUNCTION _get_migration_id2() RETURNS TEXT AS $$
BEGIN
    RETURN current_setting('app.migration_id2');
END;
$$ LANGUAGE plpgsql STABLE;

CREATE OR REPLACE FUNCTION _get_history_table() RETURNS TEXT AS $$
BEGIN
    RETURN current_setting('app.history_table');
END;
$$ LANGUAGE plpgsql STABLE;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Admins" (
        "Id" character varying(1024) NOT NULL,
        "Email" character varying(255) NOT NULL,
        "Role" character varying(50) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Admins" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Credits" (
        "Id" character varying(40) NOT NULL,
        "CreditRemaining" numeric(18,2) NOT NULL,
        "TenantId" character varying(40) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (NOW()),
        "LastUpdatedAt" timestamp with time zone NOT NULL DEFAULT (NOW()),
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_Credits" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Customers" (
        "Id" character varying(40) NOT NULL,
        "CustomerType" integer NOT NULL,
        "CurrentlyAtOnboardingFlowStep" integer NOT NULL,
        "Type" character varying(50),
        "VatNumber" character varying(50),
        "Country" character varying(100),
        "StripeCustomerId" character varying(100),
        "StripeSubscriptionId" character varying(100),
        "VatId" character varying(50),
        "OrganizationId" character varying(40),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_Customers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "DataExports" (
        "Id" character varying(1024) NOT NULL,
        "DownloadUrl" text,
        "FileName" character varying(1024) NOT NULL,
        "FileSize" double precision NOT NULL,
        "TenantId" character varying(1024) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_DataExports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "FileMetadata" (
        "Id" character varying(1024) NOT NULL,
        "FileKey" character varying(500) NOT NULL,
        "OriginalFileName" character varying(255) NOT NULL,
        "ContentType" character varying(100) NOT NULL,
        "SizeInBytes" bigint NOT NULL,
        "UploadedBy" character varying(50) NOT NULL,
        "Folder" character varying(200),
        "IsPublic" boolean NOT NULL DEFAULT FALSE,
        "PublicUrl" character varying(1000),
        "CustomMetadata" jsonb DEFAULT ('{}'),
        "TenantId" character varying(50) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_FileMetadata" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Files" (
        "Id" character varying(1024) NOT NULL,
        "OwnerId" character varying(1024) NOT NULL,
        "FileName" character varying(1024) NOT NULL,
        "FileSize" bigint NOT NULL,
        "FileExtension" character varying(1024) NOT NULL,
        "FilePath" character varying(1024) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Files" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "GlobalRegions" (
        "Id" character varying(1024) NOT NULL,
        "Name" character varying(200) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_GlobalRegions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Languages" (
        "Id" character varying(1024) NOT NULL,
        "Name" character varying(200) NOT NULL,
        "Code" character varying(20) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Languages" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "PaymentProviders" (
        "Id" character varying(40) NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Description" character varying(500),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_PaymentProviders" PRIMARY KEY ("Id")
    );
    COMMENT ON COLUMN "PaymentProviders"."Name" IS 'Name of the payment provider (e.g., Stripe, PayPal, Square)';
    COMMENT ON COLUMN "PaymentProviders"."Description" IS 'Optional description of the payment provider and its capabilities';
    COMMENT ON COLUMN "PaymentProviders"."IsActive" IS 'Indicates if this payment provider is currently active and available for use';
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Permissions" (
        "Id" character varying(40) NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Description" character varying(500),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_Permissions" PRIMARY KEY ("Id")
    );
    COMMENT ON COLUMN "Permissions"."Name" IS 'The permission name (e.g., ''read:users'', ''write:documents'')';
    COMMENT ON COLUMN "Permissions"."Description" IS 'Optional description of what this permission allows';
    COMMENT ON COLUMN "Permissions"."CreatedAt" IS 'Timestamp when the permission was created';
    COMMENT ON COLUMN "Permissions"."LastUpdatedAt" IS 'Timestamp when the permission was last modified';
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Searches" (
        "Id" character varying(1024) NOT NULL,
        "Name" character varying(1024) NOT NULL,
        "Description" character varying(1024) NOT NULL,
        "Url" text NOT NULL,
        "SearchType" character varying(1024) NOT NULL,
        "SearchCriteria" character varying(1024) NOT NULL,
        "SearchResults" character varying(1024) NOT NULL,
        "SearchStatus" character varying(1024) NOT NULL,
        "SearchError" character varying(1024) NOT NULL,
        "SearchErrorMessage" character varying(1024) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Searches" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Sessions" (
        "Id" character varying(1024) NOT NULL,
        "SessionKey" character varying(100) NOT NULL,
        "SessionData" character varying(1024) NOT NULL,
        "ExpireDate" timestamp with time zone NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Sessions" PRIMARY KEY ("Id"),
        CONSTRAINT "AK_Sessions_SessionKey" UNIQUE ("SessionKey")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Webhooks" (
        "Id" character varying(1024) NOT NULL,
        "Url" text NOT NULL,
        "Secret" character varying(1024) NOT NULL,
        "Description" character varying(1024),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Webhooks" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Tenants" (
        "Id" character varying(40) NOT NULL,
        "TenantType" integer NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Description" character varying(500),
        "IsActive" boolean NOT NULL,
        "IsPrimary" boolean NOT NULL,
        "Email" character varying(100),
        "Phone" character varying(20),
        "VatNumber" character varying(50),
        "Country" character varying(100),
        "CustomerId" character varying(1024),
        "CustomerEntityId" character varying(1024),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_Tenants" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Tenants_Customers_CustomerEntityId" FOREIGN KEY ("CustomerEntityId") REFERENCES "Customers" ("Id"),
        CONSTRAINT "FK_Tenants_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE RESTRICT
    );
    COMMENT ON COLUMN "Tenants"."Id" IS 'Unique tenant identifier with prefix (e.g., tenant_01ABCD...)';
    COMMENT ON COLUMN "Tenants"."TenantType" IS '0 = Personal (B2C), 1 = Organization (B2B)';
    COMMENT ON COLUMN "Tenants"."Name" IS 'Tenant name: Full name for Personal, Company name for Organization';
    COMMENT ON COLUMN "Tenants"."Description" IS 'Optional description, typically used for Organization tenants';
    COMMENT ON COLUMN "Tenants"."IsActive" IS 'Whether tenant can access the system';
    COMMENT ON COLUMN "Tenants"."IsPrimary" IS 'Indicates primary tenant for multi-tenant B2C users';
    COMMENT ON COLUMN "Tenants"."Email" IS 'Contact email for the tenant';
    COMMENT ON COLUMN "Tenants"."Phone" IS 'Contact phone for the tenant';
    COMMENT ON COLUMN "Tenants"."VatNumber" IS 'VAT/Tax number (B2B only)';
    COMMENT ON COLUMN "Tenants"."Country" IS 'Country code (B2B only)';
    COMMENT ON COLUMN "Tenants"."CreatedAt" IS 'Timestamp when tenant was created';
    COMMENT ON COLUMN "Tenants"."LastUpdatedAt" IS 'Timestamp when tenant was last modified';
    COMMENT ON COLUMN "Tenants"."IsSoftDeleted" IS 'Soft delete flag';
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Countries" (
        "Id" character varying(40) NOT NULL,
        "Name" character varying(200) NOT NULL,
        "IsoCode" integer NOT NULL,
        "CityId" character varying(1024) NOT NULL,
        "GlobalRegionId" character varying(40) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_Countries" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Countries_GlobalRegions_GlobalRegionId" FOREIGN KEY ("GlobalRegionId") REFERENCES "GlobalRegions" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "ApiLogs" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "ApiKeyId" character varying(450) NOT NULL,
        "SessionId" character varying(450) NOT NULL,
        "RequestPath" character varying(2000) NOT NULL,
        "StatusCode" integer NOT NULL,
        "StatusMessage" character varying(500) NOT NULL,
        "RequestMethod" character varying(10) NOT NULL,
        "SourceIp" character varying(45) NOT NULL,
        "ResponseLatency" integer NOT NULL,
        CONSTRAINT "PK_ApiLogs" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ApiLogs_Sessions_SessionId" FOREIGN KEY ("SessionId") REFERENCES "Sessions" ("SessionKey") ON DELETE RESTRICT
    );
    COMMENT ON COLUMN "ApiLogs"."Id" IS 'Primary key for the API log entry';
    COMMENT ON COLUMN "ApiLogs"."ApiKeyId" IS 'Foreign key reference to the API key used for the request';
    COMMENT ON COLUMN "ApiLogs"."SessionId" IS 'Session identifier for tracking user sessions';
    COMMENT ON COLUMN "ApiLogs"."RequestPath" IS 'The API endpoint path that was requested';
    COMMENT ON COLUMN "ApiLogs"."StatusCode" IS 'HTTP status code returned by the API';
    COMMENT ON COLUMN "ApiLogs"."StatusMessage" IS 'HTTP status message or custom response message';
    COMMENT ON COLUMN "ApiLogs"."RequestMethod" IS 'HTTP method used for the request (GET, POST, PUT, PATCH, DELETE, etc.)';
    COMMENT ON COLUMN "ApiLogs"."SourceIp" IS 'Source IP address of the client making the request';
    COMMENT ON COLUMN "ApiLogs"."ResponseLatency" IS 'Response time in milliseconds for performance monitoring';
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "ContactPersons" (
        "Id" character varying(1024) NOT NULL,
        "FirstName" character varying(50) NOT NULL,
        "LastName" character varying(200) NOT NULL,
        "TenantId" character varying(1024) NOT NULL,
        "TenantId1" character varying(1024),
        "CustomerId" character varying(1024) NOT NULL,
        "CustomerId1" character varying(1024),
        "IsActive" boolean NOT NULL,
        "IsPrimary" boolean NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_ContactPersons" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ContactPersons_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ContactPersons_Customers_CustomerId1" FOREIGN KEY ("CustomerId1") REFERENCES "Customers" ("Id"),
        CONSTRAINT "FK_ContactPersons_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenants" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ContactPersons_Tenants_TenantId1" FOREIGN KEY ("TenantId1") REFERENCES "Tenants" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Subscriptions" (
        "Id" character varying(40) NOT NULL,
        "Name" character varying(200) NOT NULL,
        "Description" character varying(1000),
        "Code" character varying(50) NOT NULL,
        "Status" character varying(50) NOT NULL,
        "CreatedBy" character varying(40) NOT NULL,
        "UpdatedBy" character varying(40) NOT NULL,
        "TenantId" character varying(40) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_Subscriptions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Subscriptions_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenants" ("Id") ON DELETE CASCADE
    );
    COMMENT ON COLUMN "Subscriptions"."Id" IS 'Unique identifier for the subscription plan';
    COMMENT ON COLUMN "Subscriptions"."Name" IS 'Name of the subscription plan (e.g., Basic, Pro, Enterprise)';
    COMMENT ON COLUMN "Subscriptions"."Description" IS 'Detailed description of the subscription plan features and benefits';
    COMMENT ON COLUMN "Subscriptions"."Code" IS 'Unique code identifier for the subscription plan';
    COMMENT ON COLUMN "Subscriptions"."Status" IS 'Current status of the subscription (Active, Inactive, Discontinued, etc.)';
    COMMENT ON COLUMN "Subscriptions"."CreatedBy" IS 'User ID who created this subscription';
    COMMENT ON COLUMN "Subscriptions"."UpdatedBy" IS 'User ID who last updated this subscription';
    COMMENT ON COLUMN "Subscriptions"."CreatedAt" IS 'Timestamp when the subscription was created';
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Users" (
        "Id" character varying(40) NOT NULL,
        "FirstName" character varying(100) NOT NULL,
        "LastName" character varying(100) NOT NULL,
        "UserName" character varying(100) NOT NULL,
        "IsActive" boolean NOT NULL,
        "Email" character varying(100) NOT NULL,
        "ExternalAuthId" character varying(1024),
        "LastLogin" timestamp with time zone NOT NULL,
        "TenantId" character varying(40) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_Users" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Users_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenants" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "CountryRegions" (
        "Id" character varying(40) NOT NULL,
        "Name" character varying(100) NOT NULL,
        "CountryId" character varying(40) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_CountryRegions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_CountryRegions_Countries_CountryId" FOREIGN KEY ("CountryId") REFERENCES "Countries" ("Id") ON DELETE CASCADE
    );
    COMMENT ON COLUMN "CountryRegions"."Id" IS 'Primary key for country region using ULID format';
    COMMENT ON COLUMN "CountryRegions"."Name" IS 'Name of the region within the country (e.g., Syddanmark, Midtjylland)';
    COMMENT ON COLUMN "CountryRegions"."CountryId" IS 'Foreign key to the country this region belongs to';
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "StateEntity" (
        "Id" character varying(1024) NOT NULL,
        "Name" character varying(1024) NOT NULL,
        "IsoCode" character varying(1024),
        "CountryId" character varying(1024),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_StateEntity" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_StateEntity_Countries_CountryId" FOREIGN KEY ("CountryId") REFERENCES "Countries" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Accounts" (
        "Id" character varying(1024) NOT NULL,
        "CustomerType" integer NOT NULL,
        "Name" character varying(1024) NOT NULL,
        "Email" character varying(255) NOT NULL,
        "Role" character varying(100) NOT NULL,
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        "OwnerId" character varying(1024),
        "UserId" character varying(1024) NOT NULL,
        "TenantId" character varying(1024) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "LastUpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Accounts" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Accounts_Users_OwnerId" FOREIGN KEY ("OwnerId") REFERENCES "Users" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "AuditLogs" (
        "Id" character varying(40) NOT NULL,
        "Action" character varying(200) NOT NULL,
        "Category" character varying(100),
        "NewValue" text NOT NULL,
        "OldValue" text NOT NULL,
        "ModifiedByUserId" character varying(1024),
        "ModifiedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "TenantId" character varying(40) NOT NULL,
        "UserId" character varying(40) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (NOW()),
        "LastUpdatedAt" timestamp with time zone NOT NULL DEFAULT (NOW()),
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_AuditLogs" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_AuditLogs_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenants" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_AuditLogs_Users_ModifiedByUserId" FOREIGN KEY ("ModifiedByUserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_AuditLogs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
    );
    COMMENT ON COLUMN "AuditLogs"."Id" IS 'Primary key for audit log entry';
    COMMENT ON COLUMN "AuditLogs"."Action" IS 'Description of the action performed (GDPR sensitive)';
    COMMENT ON COLUMN "AuditLogs"."Category" IS 'Category classification for the audit action';
    COMMENT ON COLUMN "AuditLogs"."NewValue" IS 'New value after the change (JSON format)';
    COMMENT ON COLUMN "AuditLogs"."OldValue" IS 'Previous value before the change (JSON format)';
    COMMENT ON COLUMN "AuditLogs"."ModifiedAt" IS 'Timestamp when the action was performed';
    COMMENT ON COLUMN "AuditLogs"."TenantId" IS 'Foreign key to the tenant where the action occurred';
    COMMENT ON COLUMN "AuditLogs"."UserId" IS 'Foreign key to the user who performed the action';
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "EmailAddresses" (
        "Id" character varying(1024) NOT NULL,
        "Address" character varying(320) NOT NULL,
        "UserId" character varying(1024) NOT NULL,
        "CustomerId" character varying(1024),
        "ContactPersonId" character varying(1024),
        "TenantId" character varying(1024),
        "ContactPersonEntityId" character varying(1024),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Emails" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_EmailAddresses_ContactPersons_ContactPersonEntityId" FOREIGN KEY ("ContactPersonEntityId") REFERENCES "ContactPersons" ("Id"),
        CONSTRAINT "FK_EmailAddresses_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Emails_ContactPersons_ContactPersonId" FOREIGN KEY ("ContactPersonId") REFERENCES "ContactPersons" ("Id") ON DELETE SET NULL,
        CONSTRAINT "FK_Emails_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE SET NULL,
        CONSTRAINT "FK_Emails_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenants" ("Id") ON DELETE SET NULL
    );
    COMMENT ON COLUMN "EmailAddresses"."Id" IS 'Primary key for email address';
    COMMENT ON COLUMN "EmailAddresses"."Address" IS 'Email address following RFC 5321 standards';
    COMMENT ON COLUMN "EmailAddresses"."UserId" IS 'Foreign key to the user who owns this email';
    COMMENT ON COLUMN "EmailAddresses"."CustomerId" IS 'Optional foreign key to associated customer';
    COMMENT ON COLUMN "EmailAddresses"."TenantId" IS 'Optional foreign key to associated tenant';
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "EmailInvites" (
        "Id" character varying(40) NOT NULL,
        "Token" character varying(1024) NOT NULL,
        "ReferredEmailAddress" character varying(255) NOT NULL,
        "ExpireAt" timestamp with time zone NOT NULL,
        "InviteIsUsed" boolean NOT NULL,
        "UserEntityId" character varying(40),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_EmailInvites" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_EmailInvites_Users_UserEntityId" FOREIGN KEY ("UserEntityId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "EmailVerificationEntity" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "Token" character varying(1024) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "ExpireAt" timestamp with time zone NOT NULL,
        "HasBeenOpened" boolean NOT NULL,
        "HasBeenVerified" boolean NOT NULL,
        "UserEntityId" character varying(1024),
        CONSTRAINT "PK_EmailVerificationEntity" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_EmailVerificationEntity_Users_UserEntityId" FOREIGN KEY ("UserEntityId") REFERENCES "Users" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "EmailVerifications" (
        "Id" character varying(40) NOT NULL,
        "Token" character varying(1024) NOT NULL,
        "ExpireAt" timestamp with time zone NOT NULL,
        "HasBeenOpened" boolean NOT NULL,
        "HasBeenVerified" boolean NOT NULL,
        "UserEntityId" character varying(40),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_EmailVerifications" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_EmailVerifications_Users_UserEntityId" FOREIGN KEY ("UserEntityId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Integrations" (
        "Id" character varying(1024) NOT NULL,
        "OwnerId" character varying(1024) NOT NULL,
        "Name" character varying(100) NOT NULL,
        "ServiceName" character varying(100) NOT NULL,
        "Description" character varying(500),
        "ApiKeySecretReference" character varying(200) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Integrations" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Integrations_Users_OwnerId" FOREIGN KEY ("OwnerId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    COMMENT ON COLUMN "Integrations"."Id" IS 'Primary key for integration';
    COMMENT ON COLUMN "Integrations"."OwnerId" IS 'Foreign key to the user who owns this integration';
    COMMENT ON COLUMN "Integrations"."Name" IS 'Friendly name for the integration';
    COMMENT ON COLUMN "Integrations"."ServiceName" IS 'Name of the third-party service (e.g., Stripe, SendGrid, Twilio)';
    COMMENT ON COLUMN "Integrations"."Description" IS 'Optional description of the integration purpose';
    COMMENT ON COLUMN "Integrations"."ApiKeySecretReference" IS 'Reference to the securely stored API key';
    COMMENT ON COLUMN "Integrations"."CreatedAt" IS 'Timestamp when the integration was created';
    COMMENT ON COLUMN "Integrations"."LastUpdatedAt" IS 'Timestamp when the integration was last updated';
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Notifications" (
        "Id" character varying(40) NOT NULL,
        "OwnerId" character varying(40) NOT NULL,
        "Title" character varying(200) NOT NULL,
        "Message" character varying(1000) NOT NULL,
        "IsRead" boolean NOT NULL DEFAULT FALSE,
        "UserId" character varying(40) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Notifications_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "PhoneNumbers" (
        "Id" character varying(1024) NOT NULL,
        "Number" character varying(50) NOT NULL,
        "CountryCode" character varying(10) NOT NULL,
        "IsPrimary" boolean NOT NULL,
        "IsVerified" boolean NOT NULL,
        "UserId" character varying(1024),
        "CustomerId" character varying(1024),
        "ContactPersonId" character varying(1024),
        "TenantId" character varying(1024) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_PhoneNumbers" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_PhoneNumbers_ContactPersons_ContactPersonId" FOREIGN KEY ("ContactPersonId") REFERENCES "ContactPersons" ("Id"),
        CONSTRAINT "FK_PhoneNumbers_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id"),
        CONSTRAINT "FK_PhoneNumbers_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenants" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_PhoneNumbers_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "ProfileEntity" (
        "Id" character varying(1024) NOT NULL,
        "PhoneNumber" character varying(1024),
        "Bio" character varying(1024),
        "AvatarUrl" text,
        "WebsiteUrl" text,
        "TimeZone" character varying(1024),
        "Language" character varying(1024),
        "Country" character varying(1024),
        "UserId" character varying(1024) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_ProfileEntity" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ProfileEntity_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "ResourcePermissions" (
        "Id" character varying(40) NOT NULL,
        "UserId" character varying(40) NOT NULL,
        "ResourceId" character varying(40) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_ResourcePermissions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ResourcePermissions_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Roles" (
        "Id" character varying(40) NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Description" character varying(500),
        "UserEntityId" character varying(1024),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_Roles" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Roles_Users_UserEntityId" FOREIGN KEY ("UserEntityId") REFERENCES "Users" ("Id")
    );
    COMMENT ON COLUMN "Roles"."Name" IS 'The role name (e.g., Administrator, User, Manager)';
    COMMENT ON COLUMN "Roles"."Description" IS 'Optional description of the role''s purpose and permissions';
    COMMENT ON COLUMN "Roles"."CreatedAt" IS 'Timestamp when the role was created';
    COMMENT ON COLUMN "Roles"."LastUpdatedAt" IS 'Timestamp when the role was last modified';
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "Cities" (
        "Id" character varying(40) NOT NULL,
        "Name" character varying(100) NOT NULL,
        "CountryId" character varying(40) NOT NULL,
        "PostalCode" character varying(20) NOT NULL,
        "StateId" character varying(40) NOT NULL,
        "CountryEntityId" character varying(1024),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_Cities" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Cities_Countries_CountryEntityId" FOREIGN KEY ("CountryEntityId") REFERENCES "Countries" ("Id"),
        CONSTRAINT "FK_Cities_Countries_CountryId" FOREIGN KEY ("CountryId") REFERENCES "Countries" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Cities_States_StateId" FOREIGN KEY ("StateId") REFERENCES "StateEntity" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "ResourcePermissionTypes" (
        "Id" character varying(1024) NOT NULL,
        "ResourcePermissionId" character varying(1024) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_ResourcePermissionTypes" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ResourcePermissionTypes_ResourcePermissions_ResourcePermiss~" FOREIGN KEY ("ResourcePermissionId") REFERENCES "ResourcePermissions" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "RolePermissions" (
        "Id" character varying(40) NOT NULL,
        "RoleId" character varying(40) NOT NULL,
        "PermissionId" character varying(40) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_RolePermissions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_RolePermissions_Permissions_PermissionId" FOREIGN KEY ("PermissionId") REFERENCES "Permissions" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_RolePermissions_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Roles" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
DECLARE
    migrationId1 TEXT := _get_migration_id1();
    migrationId2 TEXT := _get_migration_id2();
    historyTable TEXT := _get_history_table();
BEGIN
    IF NOT EXISTS(SELECT 1 FROM historyTable WHERE "MigrationId" = migrationId1) THEN
    CREATE TABLE "UserRoles" (
        "Id" character varying(40) NOT NULL,
        "UserId" character varying(40) NOT NULL,
        "RoleId" character varying(40) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_UserRoles" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_UserRoles_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Roles" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_UserRoles_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE TABLE "Streets" (
        "Id" character varying(1024) NOT NULL,
        "Name" character varying(200) NOT NULL,
        "CityId" character varying(1024) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Streets" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Streets_Cities_CityId" FOREIGN KEY ("CityId") REFERENCES "Cities" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE TABLE "Addresses" (
        "Id" character varying(1024) NOT NULL,
        "CityId" character varying(1024) NOT NULL,
        "CountryId" character varying(1024) NOT NULL,
        "StreetId" character varying(1024) NOT NULL,
        "IsPrimary" boolean NOT NULL,
        "Longitude" character varying(20),
        "Latitude" character varying(20),
        "Floor" character varying(10) NOT NULL,
        "StreetNumber" character varying(10) NOT NULL,
        "UnitNumber" character varying(10),
        "State" character varying(100) NOT NULL,
        "PostalCode" character varying(20) NOT NULL,
        "CustomerId" character varying(1024),
        "TenantId" character varying(1024) NOT NULL,
        "ContactPersonEntityId" character varying(1024),
        "UserEntityId" character varying(1024),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Addresses" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Addresses_Cities_CityId" FOREIGN KEY ("CityId") REFERENCES "Cities" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Addresses_ContactPersons_ContactPersonEntityId" FOREIGN KEY ("ContactPersonEntityId") REFERENCES "ContactPersons" ("Id"),
        CONSTRAINT "FK_Addresses_Countries_CountryId" FOREIGN KEY ("CountryId") REFERENCES "Countries" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Addresses_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Addresses_Streets_StreetId" FOREIGN KEY ("StreetId") REFERENCES "Streets" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Addresses_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenants" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Addresses_Users_UserEntityId" FOREIGN KEY ("UserEntityId") REFERENCES "Users" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Accounts_Email" ON "Accounts" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Accounts_OwnerId" ON "Accounts" ("OwnerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Addresses_CityId" ON "Addresses" ("CityId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Addresses_ContactPersonEntityId" ON "Addresses" ("ContactPersonEntityId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Addresses_CountryId" ON "Addresses" ("CountryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Addresses_CustomerId" ON "Addresses" ("CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Addresses_PostalCode" ON "Addresses" ("PostalCode");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Addresses_StreetId" ON "Addresses" ("StreetId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Addresses_TenantId" ON "Addresses" ("TenantId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Addresses_UserEntityId" ON "Addresses" ("UserEntityId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Admins_Email" ON "Admins" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ApiLogs_ApiKeyId" ON "ApiLogs" ("ApiKeyId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ApiLogs_ApiKeyId_StatusCode" ON "ApiLogs" ("ApiKeyId", "StatusCode");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ApiLogs_RequestMethod" ON "ApiLogs" ("RequestMethod");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ApiLogs_SessionId" ON "ApiLogs" ("SessionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ApiLogs_SessionId_StatusCode" ON "ApiLogs" ("SessionId", "StatusCode");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ApiLogs_SourceIp" ON "ApiLogs" ("SourceIp");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ApiLogs_StatusCode" ON "ApiLogs" ("StatusCode");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_AuditLogs_Category" ON "AuditLogs" ("Category") WHERE "Category" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_AuditLogs_Category_ModifiedAt" ON "AuditLogs" ("Category", "ModifiedAt") WHERE "Category" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_AuditLogs_IsSoftDeleted" ON "AuditLogs" ("IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_AuditLogs_ModifiedAt" ON "AuditLogs" ("ModifiedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_AuditLogs_ModifiedByUserId" ON "AuditLogs" ("ModifiedByUserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_AuditLogs_TenantId" ON "AuditLogs" ("TenantId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_AuditLogs_TenantId_IsSoftDeleted" ON "AuditLogs" ("TenantId", "IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_AuditLogs_TenantId_ModifiedAt" ON "AuditLogs" ("TenantId", "ModifiedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_AuditLogs_UserId" ON "AuditLogs" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_AuditLogs_UserId_ModifiedAt" ON "AuditLogs" ("UserId", "ModifiedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Cities_CountryEntityId" ON "Cities" ("CountryEntityId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Cities_CountryId" ON "Cities" ("CountryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Cities_IsSoftDeleted" ON "Cities" ("IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Cities_Name" ON "Cities" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Cities_PostalCode" ON "Cities" ("PostalCode");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Cities_StateId" ON "Cities" ("StateId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Cities_StateId_PostalCode_Unique" ON "Cities" ("StateId", "PostalCode");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ContactPersons_CustomerId" ON "ContactPersons" ("CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ContactPersons_CustomerId1" ON "ContactPersons" ("CustomerId1");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_ContactPersons_Id" ON "ContactPersons" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ContactPersons_TenantId" ON "ContactPersons" ("TenantId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ContactPersons_TenantId1" ON "ContactPersons" ("TenantId1");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Countries_GlobalRegionId" ON "Countries" ("GlobalRegionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Countries_IsoCode" ON "Countries" ("IsoCode");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Countries_IsSoftDeleted" ON "Countries" ("IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Countries_Name" ON "Countries" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_CountryRegions_CountryId" ON "CountryRegions" ("CountryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_CountryRegions_CountryId_Name_Unique" ON "CountryRegions" ("CountryId", "Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_CountryRegions_Name" ON "CountryRegions" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Credits_IsSoftDeleted" ON "Credits" ("IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Credits_TenantId" ON "Credits" ("TenantId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Credits_TenantId_IsSoftDeleted" ON "Credits" ("TenantId", "IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Customers_CustomerType" ON "Customers" ("CustomerType");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Customers_Id" ON "Customers" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Customers_IsSoftDeleted" ON "Customers" ("IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Customers_OrganizationId" ON "Customers" ("OrganizationId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Customers_StripeCustomerId" ON "Customers" ("StripeCustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Customers_Type" ON "Customers" ("Type");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Customers_VatNumber" ON "Customers" ("VatNumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_EmailAddresses_ContactPersonEntityId" ON "EmailAddresses" ("ContactPersonEntityId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_EmailAddresses_ContactPersonId" ON "EmailAddresses" ("ContactPersonId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Emails_Address_Unique" ON "EmailAddresses" ("Address");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Emails_CustomerId" ON "EmailAddresses" ("CustomerId") WHERE "CustomerId" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Emails_TenantId" ON "EmailAddresses" ("TenantId") WHERE "TenantId" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Emails_UserId" ON "EmailAddresses" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Emails_UserId_CustomerId" ON "EmailAddresses" ("UserId", "CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_EmailInvites_ExpireAt" ON "EmailInvites" ("ExpireAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_EmailInvites_IsSoftDeleted" ON "EmailInvites" ("IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_EmailInvites_ReferredEmailAddress" ON "EmailInvites" ("ReferredEmailAddress");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_EmailInvites_Token" ON "EmailInvites" ("Token");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_EmailInvites_UserEntityId" ON "EmailInvites" ("UserEntityId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_EmailVerificationEntity_UserEntityId" ON "EmailVerificationEntity" ("UserEntityId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_EmailVerifications_IsSoftDeleted" ON "EmailVerifications" ("IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_EmailVerifications_Token" ON "EmailVerifications" ("Token");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_EmailVerifications_UserEntityId" ON "EmailVerifications" ("UserEntityId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_FileMetadata_CreatedAt" ON "FileMetadata" ("CreatedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_FileMetadata_FileKey" ON "FileMetadata" ("FileKey");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_FileMetadata_TenantId" ON "FileMetadata" ("TenantId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_FileMetadata_TenantId_Folder" ON "FileMetadata" ("TenantId", "Folder");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_FileMetadata_TenantId_UploadedBy" ON "FileMetadata" ("TenantId", "UploadedBy");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_GlobalRegions_Name" ON "GlobalRegions" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Integrations_CreatedAt" ON "Integrations" ("CreatedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Integrations_OwnerId" ON "Integrations" ("OwnerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Integrations_OwnerId_ServiceName_Unique" ON "Integrations" ("OwnerId", "ServiceName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Integrations_ServiceName" ON "Integrations" ("ServiceName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Integrations_ServiceName_CreatedAt" ON "Integrations" ("ServiceName", "CreatedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Languages_Code" ON "Languages" ("Code");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Languages_Name" ON "Languages" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Notifications_CreatedAt" ON "Notifications" ("CreatedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Notifications_IsRead" ON "Notifications" ("IsRead");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Notifications_OwnerId" ON "Notifications" ("OwnerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Notifications_UserId" ON "Notifications" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_PaymentProviders_CreatedAt" ON "PaymentProviders" ("CreatedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_PaymentProviders_IsActive" ON "PaymentProviders" ("IsActive");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_PaymentProviders_Name" ON "PaymentProviders" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Permissions_CreatedAt" ON "Permissions" ("CreatedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Permissions_Name" ON "Permissions" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_PhoneNumbers_ContactPersonId" ON "PhoneNumbers" ("ContactPersonId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_PhoneNumbers_CustomerId" ON "PhoneNumbers" ("CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_PhoneNumbers_Id" ON "PhoneNumbers" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_PhoneNumbers_TenantId" ON "PhoneNumbers" ("TenantId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_PhoneNumbers_UserId" ON "PhoneNumbers" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_ProfileEntity_UserId" ON "ProfileEntity" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ResourcePermissions_IsSoftDeleted" ON "ResourcePermissions" ("IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ResourcePermissions_ResourceId" ON "ResourcePermissions" ("ResourceId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ResourcePermissions_UserId" ON "ResourcePermissions" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_ResourcePermissions_UserId_ResourceId_Unique" ON "ResourcePermissions" ("UserId", "ResourceId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_ResourcePermissionTypes_ResourcePermissionId" ON "ResourcePermissionTypes" ("ResourcePermissionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_RolePermissions_PermissionId" ON "RolePermissions" ("PermissionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_RolePermissions_RoleId" ON "RolePermissions" ("RoleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_RolePermissions_RoleId_PermissionId_Unique" ON "RolePermissions" ("RoleId", "PermissionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Roles_CreatedAt" ON "Roles" ("CreatedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Roles_Name" ON "Roles" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Roles_UserEntityId" ON "Roles" ("UserEntityId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Sessions_SessionKey" ON "Sessions" ("SessionKey");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_StateEntity_CountryId" ON "StateEntity" ("CountryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Streets_CityId" ON "Streets" ("CityId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Streets_CityId_Name_Unique" ON "Streets" ("CityId", "Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Streets_Name" ON "Streets" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Subscriptions_Code" ON "Subscriptions" ("Code");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Subscriptions_CreatedAt" ON "Subscriptions" ("CreatedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Subscriptions_CreatedBy" ON "Subscriptions" ("CreatedBy");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Subscriptions_Name" ON "Subscriptions" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Subscriptions_Status" ON "Subscriptions" ("Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Subscriptions_TenantId" ON "Subscriptions" ("TenantId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Tenants_Country" ON "Tenants" ("Country");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Tenants_CustomerEntityId" ON "Tenants" ("CustomerEntityId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Tenants_CustomerId" ON "Tenants" ("CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Tenants_Email_TenantType" ON "Tenants" ("Email", "TenantType");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Tenants_Id" ON "Tenants" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Tenants_IsActive" ON "Tenants" ("IsActive");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Tenants_IsSoftDeleted" ON "Tenants" ("IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Tenants_Name" ON "Tenants" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Tenants_TenantType" ON "Tenants" ("TenantType");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Tenants_Type_Active_NotDeleted" ON "Tenants" ("TenantType", "IsActive", "IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Tenants_VatNumber" ON "Tenants" ("VatNumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_UserRoles_RoleId" ON "UserRoles" ("RoleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_UserRoles_UserId" ON "UserRoles" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_UserRoles_UserId_RoleId" ON "UserRoles" ("UserId", "RoleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Users_Id" ON "Users" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Users_IsActive" ON "Users" ("IsActive");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Users_IsSoftDeleted" ON "Users" ("IsSoftDeleted");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE INDEX "IX_Users_TenantId" ON "Users" ("TenantId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    CREATE UNIQUE INDEX "IX_Users_UserName" ON "Users" ("UserName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id1()) THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (_get_migration_id1(), '10.0.1');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id2()) THEN
    ALTER TABLE "Users" ADD "SearchVector" character varying(1024) GENERATED ALWAYS AS (to_tsvector('english', coalesce("FirstName", '') || ' ' || coalesce("LastName", '') || ' ' || coalesce("UserName", '') || ' ' || coalesce("Email", ''))) STORED;
    COMMENT ON COLUMN "Users"."SearchVector" IS 'Full-text search vector for user search';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id2()) THEN
    ALTER TABLE "Tenants" ADD "SearchVector" character varying(1024) GENERATED ALWAYS AS (to_tsvector('english', coalesce("Name", '') || ' ' || coalesce("Description", '') || ' ' || coalesce("Email", '') || ' ' || coalesce("VatNumber", ''))) STORED;
    COMMENT ON COLUMN "Tenants"."SearchVector" IS 'Full-text search vector for tenant search';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id2()) THEN
    CREATE TABLE "WebhookEvents" (
        "Id" character varying(40) NOT NULL,
        "EventId" character varying(255) NOT NULL,
        "EventType" character varying(255) NOT NULL,
        "Source" character varying(50) NOT NULL,
        "Payload" text NOT NULL,
        "TenantId" character varying(40) NOT NULL DEFAULT '',
        "Status" character varying(50) NOT NULL,
        "RetryCount" integer NOT NULL DEFAULT 0,
        "ErrorMessage" character varying(2000),
        "ReceivedAt" timestamp with time zone NOT NULL,
        "ProcessedAt" timestamp with time zone,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone,
        "IsSoftDeleted" boolean NOT NULL,
        CONSTRAINT "PK_WebhookEvents" PRIMARY KEY ("Id")
    );
    COMMENT ON COLUMN "WebhookEvents"."Id" IS 'Unique webhook event identifier with prefix (e.g., whevt_01ABCD...)';
    COMMENT ON COLUMN "WebhookEvents"."EventId" IS 'External event ID from webhook provider (e.g., Stripe event ID)';
    COMMENT ON COLUMN "WebhookEvents"."EventType" IS 'Type of webhook event (e.g., payment_intent.succeeded)';
    COMMENT ON COLUMN "WebhookEvents"."Source" IS 'Source of webhook (e.g., stripe, paypal)';
    COMMENT ON COLUMN "WebhookEvents"."Payload" IS 'Raw JSON payload of webhook event';
    COMMENT ON COLUMN "WebhookEvents"."TenantId" IS 'Tenant ID if webhook is tenant-scoped';
    COMMENT ON COLUMN "WebhookEvents"."Status" IS 'Processing status (Pending, Processed, Failed, etc.)';
    COMMENT ON COLUMN "WebhookEvents"."RetryCount" IS 'Number of retry attempts';
    COMMENT ON COLUMN "WebhookEvents"."ErrorMessage" IS 'Error message if processing failed';
    COMMENT ON COLUMN "WebhookEvents"."ReceivedAt" IS 'Timestamp when event was received';
    COMMENT ON COLUMN "WebhookEvents"."ProcessedAt" IS 'Timestamp when event was successfully processed';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id2()) THEN
    CREATE INDEX "IX_Users_SearchVector" ON "Users" USING GIN ("SearchVector");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id2()) THEN
    CREATE INDEX "IX_Tenants_SearchVector" ON "Tenants" USING GIN ("SearchVector");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id2()) THEN
    CREATE UNIQUE INDEX "IX_WebhookEvents_EventId_Source" ON "WebhookEvents" ("EventId", "Source");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id2()) THEN
    CREATE UNIQUE INDEX "IX_WebhookEvents_Id" ON "WebhookEvents" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id2()) THEN
    CREATE INDEX "IX_WebhookEvents_ReceivedAt" ON "WebhookEvents" ("ReceivedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id2()) THEN
    CREATE INDEX "IX_WebhookEvents_Status" ON "WebhookEvents" ("Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id2()) THEN
    CREATE INDEX "IX_WebhookEvents_TenantId" ON "WebhookEvents" ("TenantId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = _get_migration_id2()) THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (_get_migration_id2(), '10.0.1');
    END IF;
END $EF$;

-- Clean up helper functions
DROP FUNCTION IF EXISTS _get_migration_id1();
DROP FUNCTION IF EXISTS _get_migration_id2();
DROP FUNCTION IF EXISTS _get_history_table();

COMMIT;

