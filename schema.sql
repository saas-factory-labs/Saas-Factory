--
-- PostgreSQL database dump
--



-- Dumped from database version 17.7 (Debian 17.7-3.pgdg13+1)
-- Dumped by pg_dump version 17.7 (Ubuntu 17.7-3.pgdg24.04+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: create_tenant_and_user(text, text, integer, text, text, text, text, text, text, text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.create_tenant_and_user(p_tenant_id text, p_tenant_name text, p_tenant_type integer, p_user_id text, p_user_first_name text, p_user_last_name text, p_user_email text, p_external_auth_id text, p_ip_address text DEFAULT NULL::text, p_user_agent text DEFAULT NULL::text) RETURNS json
    LANGUAGE plpgsql SECURITY DEFINER
    SET search_path TO 'public'
    AS $$
DECLARE
    v_audit_id TEXT;
    v_result JSON;
    v_profile_id TEXT;
    v_tenant_type INTEGER;
BEGIN
    -- ========================================
    -- STEP 1: Generate Audit ID (using UUID since ULID extension not available)
    -- ========================================
    v_audit_id := 'audit_' || REPLACE(gen_random_uuid()::text, '-', '');
    
    -- ========================================
    -- STEP 2: Input Validation
    -- ========================================
    
    -- Validate tenant ID format
    IF NOT validate_id_format(p_tenant_id, 'tenant') THEN
        INSERT INTO "SignupAuditLog" 
        VALUES (v_audit_id, p_tenant_id, p_user_id, p_user_email, p_ip_address, p_user_agent, NOW(), false, 'Invalid tenant_id format');
        
        RAISE EXCEPTION 'Invalid tenant_id format. Expected: tenant_XXXXXXXXXXXXXXXXXXXX';
    END IF;
    
    -- Validate user ID format
    IF NOT validate_id_format(p_user_id, 'user') THEN
        INSERT INTO "SignupAuditLog" 
        VALUES (v_audit_id, p_tenant_id, p_user_id, p_user_email, p_ip_address, p_user_agent, NOW(), false, 'Invalid user_id format');
        
        RAISE EXCEPTION 'Invalid user_id format. Expected: user_XXXXXXXXXXXXXXXXXXXX';
    END IF;
    
    -- Validate email format
    IF NOT validate_email_format(p_user_email) THEN
        INSERT INTO "SignupAuditLog" 
        VALUES (v_audit_id, p_tenant_id, p_user_id, p_user_email, p_ip_address, p_user_agent, NOW(), false, 'Invalid email format');
        
        RAISE EXCEPTION 'Invalid email format';
    END IF;
    
    -- Check for duplicate email
    IF email_exists(p_user_email) THEN
        INSERT INTO "SignupAuditLog" 
        VALUES (v_audit_id, p_tenant_id, p_user_id, p_user_email, p_ip_address, p_user_agent, NOW(), false, 'Email already exists');
        
        RAISE EXCEPTION 'Email already registered';
    END IF;
    
    -- Validate required text fields are not empty
    IF TRIM(p_tenant_name) = '' THEN
        RAISE EXCEPTION 'Tenant name cannot be empty';
    END IF;
    
    IF TRIM(p_user_first_name) = '' OR TRIM(p_user_last_name) = '' THEN
        RAISE EXCEPTION 'User first name and last name are required';
    END IF;
    
    -- ========================================
    -- STEP 3: Rate Limiting Check (Optional)
    -- ========================================
    -- Prevent abuse: max 5 signups per email per hour
    IF (SELECT COUNT(*) FROM "SignupAuditLog" 
        WHERE "Email" = p_user_email 
        AND "CreatedAt" > NOW() - INTERVAL '1 hour') >= 5 THEN
        
        INSERT INTO "SignupAuditLog" 
        VALUES (v_audit_id, p_tenant_id, p_user_id, p_user_email, p_ip_address, p_user_agent, NOW(), false, 'Rate limit exceeded');
        
        RAISE EXCEPTION 'Too many signup attempts. Please try again later.';
    END IF;
    
    -- ========================================
    -- STEP 4: Create Tenant
    -- ========================================
    -- TenantType enum: Personal = 0, Organization = 1
    -- Use parameter value passed from application
    v_tenant_type := p_tenant_type;
    
    INSERT INTO "Tenants" (
        "Id",
        "Name",
        "TenantType",
        "IsActive",
        "IsPrimary",
        "Email",
        "CreatedAt",
        "LastUpdatedAt"
    ) VALUES (
        p_tenant_id,
        p_tenant_name,
        v_tenant_type,  -- 0 = Personal (B2C), 1 = Organization (B2B)
        true,  -- Active by default
        true,  -- Primary tenant
        p_user_email,
        NOW(),
        NOW()
    );
    
    -- ========================================
    -- STEP 5: Create User (ProfileEntity will be created after with UserId reference)
    -- ========================================
    INSERT INTO "Users" (
        "Id",
        "FirstName",
        "LastName",
        "UserName",
        "Email",
        "ExternalAuthId",
        "TenantId",
        "IsActive",
        "IsSoftDeleted",
        "LastLogin",
        "CreatedAt",
        "LastUpdatedAt"
    ) VALUES (
        p_user_id,
        p_user_first_name,
        p_user_last_name,
        p_user_email,  -- Use email as username initially
        p_user_email,
        p_external_auth_id,
        p_tenant_id,  -- Link to tenant
        true,
        false,
        NOW(),
        NOW(),
        NOW()
    );
    
    -- ========================================
    -- STEP 6: Create User Profile (references User via UserId)
    -- ========================================
    v_profile_id := 'profile_' || REPLACE(gen_random_uuid()::text, '-', '');
    
    INSERT INTO "ProfileEntity" (
        "Id",
        "UserId",
        "PhoneNumber",
        "Bio",
        "AvatarUrl",
        "WebsiteUrl",
        "TimeZone",
        "Language",
        "Country",
        "IsSoftDeleted",
        "CreatedAt",
        "LastUpdatedAt"
    ) VALUES (
        v_profile_id,
        p_user_id,  -- Link profile to user
        NULL,  -- Empty profile initially
        NULL,
        NULL,
        NULL,
        NULL,
        NULL,
        NULL,
        false,
        NOW(),
        NOW()
    );
    
    -- ========================================
    -- STEP 7: Audit Success
    -- ========================================
    INSERT INTO "SignupAuditLog" (
        "Id",
        "TenantId",
        "UserId",
        "Email",
        "IpAddress",
        "UserAgent",
        "CreatedAt",
        "Success",
        "ErrorMessage"
    ) VALUES (
        v_audit_id,
        p_tenant_id,
        p_user_id,
        p_user_email,
        p_ip_address,
        p_user_agent,
        NOW(),
        true,
        NULL
    );
    
    -- ========================================
    -- STEP 8: Return Success Result
    -- ========================================
    SELECT json_build_object(
        'success', true,
        'tenant_id', p_tenant_id,
        'user_id', p_user_id,
        'profile_id', v_profile_id,
        'email', p_user_email,
        'created_at', NOW()
    ) INTO v_result;
    
    RETURN v_result;
    
EXCEPTION
    WHEN OTHERS THEN
        -- Log any errors
        INSERT INTO "SignupAuditLog" (
            "Id",
            "TenantId",
            "UserId",
            "Email",
            "IpAddress",
            "UserAgent",
            "CreatedAt",
            "Success",
            "ErrorMessage"
        ) VALUES (
            v_audit_id,
            p_tenant_id,
            p_user_id,
            p_user_email,
            p_ip_address,
            p_user_agent,
            NOW(),
            false,
            SQLERRM
        );
        
        -- Re-raise the exception
        RAISE;
END;
$$;


--
-- Name: FUNCTION create_tenant_and_user(p_tenant_id text, p_tenant_name text, p_tenant_type integer, p_user_id text, p_user_first_name text, p_user_last_name text, p_user_email text, p_external_auth_id text, p_ip_address text, p_user_agent text); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.create_tenant_and_user(p_tenant_id text, p_tenant_name text, p_tenant_type integer, p_user_id text, p_user_first_name text, p_user_last_name text, p_user_email text, p_external_auth_id text, p_ip_address text, p_user_agent text) IS 'Securely creates tenant and user during signup. Uses SECURITY DEFINER to bypass RLS for initial account creation only. Includes input validation, rate limiting, and audit logging. Critical: This function should only be called from trusted signup endpoints.';


--
-- Name: email_exists(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.email_exists(email text) RETURNS boolean
    LANGUAGE plpgsql STABLE SECURITY DEFINER
    AS $$
DECLARE
    email_found boolean;
BEGIN
    SELECT (COUNT(*) > 0) INTO email_found
    FROM "Users"
    WHERE LOWER("Email") = LOWER(email);
    
    RETURN email_found;
END;
$$;


--
-- Name: validate_email_format(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.validate_email_format(email text) RETURNS boolean
    LANGUAGE plpgsql IMMUTABLE SECURITY DEFINER
    AS $_$
BEGIN
    -- Basic email validation (RFC 5322 subset)
    RETURN email ~ '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$'
           AND LENGTH(email) <= 320; -- RFC max length
END;
$_$;


--
-- Name: validate_id_format(text, text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.validate_id_format(id text, expected_prefix text) RETURNS boolean
    LANGUAGE plpgsql IMMUTABLE SECURITY DEFINER
    AS $_$
BEGIN
    -- Check format: prefix_timestamp_random or prefix_ULID
    -- Examples: 
    --   tenant_01HX1234567890ABCDEFGHIJ (ULID format - 26 chars)
    --   tenant_18D5F3A8A5C_1234567890 (Custom format - timestamp_random)
    -- Must start with prefix_ and have at least some content after
    RETURN id ~ ('^' || expected_prefix || '_[0-9A-Za-z_]+$')
           AND LENGTH(id) > LENGTH(expected_prefix) + 5; -- Minimum reasonable length
END;
$_$;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Accounts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Accounts" (
    "Id" character varying(1024) NOT NULL,
    "CustomerType" integer NOT NULL,
    "Name" character varying(1024) NOT NULL,
    "Email" character varying(255) NOT NULL,
    "Role" character varying(100) NOT NULL,
    "IsActive" boolean DEFAULT true NOT NULL,
    "OwnerId" character varying(1024),
    "UserId" character varying(1024) NOT NULL,
    "TenantId" character varying(1024) NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "LastUpdatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: Addresses; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Addresses" (
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
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: Admins; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Admins" (
    "Id" character varying(1024) NOT NULL,
    "Email" character varying(255) NOT NULL,
    "Role" character varying(50) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: ApiLogs; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ApiLogs" (
    "Id" integer NOT NULL,
    "ApiKeyId" character varying(450) NOT NULL,
    "SessionId" character varying(450) NOT NULL,
    "RequestPath" character varying(2000) NOT NULL,
    "StatusCode" integer NOT NULL,
    "StatusMessage" character varying(500) NOT NULL,
    "RequestMethod" character varying(10) NOT NULL,
    "SourceIp" character varying(45) NOT NULL,
    "ResponseLatency" integer NOT NULL
);


--
-- Name: COLUMN "ApiLogs"."Id"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."ApiLogs"."Id" IS 'Primary key for the API log entry';


--
-- Name: COLUMN "ApiLogs"."ApiKeyId"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."ApiLogs"."ApiKeyId" IS 'Foreign key reference to the API key used for the request';


--
-- Name: COLUMN "ApiLogs"."SessionId"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."ApiLogs"."SessionId" IS 'Session identifier for tracking user sessions';


--
-- Name: COLUMN "ApiLogs"."RequestPath"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."ApiLogs"."RequestPath" IS 'The API endpoint path that was requested';


--
-- Name: COLUMN "ApiLogs"."StatusCode"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."ApiLogs"."StatusCode" IS 'HTTP status code returned by the API';


--
-- Name: COLUMN "ApiLogs"."StatusMessage"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."ApiLogs"."StatusMessage" IS 'HTTP status message or custom response message';


--
-- Name: COLUMN "ApiLogs"."RequestMethod"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."ApiLogs"."RequestMethod" IS 'HTTP method used for the request (GET, POST, PUT, PATCH, DELETE, etc.)';


--
-- Name: COLUMN "ApiLogs"."SourceIp"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."ApiLogs"."SourceIp" IS 'Source IP address of the client making the request';


--
-- Name: COLUMN "ApiLogs"."ResponseLatency"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."ApiLogs"."ResponseLatency" IS 'Response time in milliseconds for performance monitoring';


--
-- Name: ApiLogs_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."ApiLogs" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ApiLogs_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AuditLogs; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."AuditLogs" (
    "Id" character varying(40) NOT NULL,
    "Action" character varying(200) NOT NULL,
    "Category" character varying(100),
    "NewValue" text NOT NULL,
    "OldValue" text NOT NULL,
    "ModifiedByUserId" character varying(1024),
    "ModifiedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "TenantId" character varying(40) NOT NULL,
    "UserId" character varying(40) NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT now() NOT NULL,
    "LastUpdatedAt" timestamp with time zone DEFAULT now() NOT NULL,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: COLUMN "AuditLogs"."Id"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."AuditLogs"."Id" IS 'Primary key for audit log entry';


--
-- Name: COLUMN "AuditLogs"."Action"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."AuditLogs"."Action" IS 'Description of the action performed (GDPR sensitive)';


--
-- Name: COLUMN "AuditLogs"."Category"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."AuditLogs"."Category" IS 'Category classification for the audit action';


--
-- Name: COLUMN "AuditLogs"."NewValue"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."AuditLogs"."NewValue" IS 'New value after the change (JSON format)';


--
-- Name: COLUMN "AuditLogs"."OldValue"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."AuditLogs"."OldValue" IS 'Previous value before the change (JSON format)';


--
-- Name: COLUMN "AuditLogs"."ModifiedAt"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."AuditLogs"."ModifiedAt" IS 'Timestamp when the action was performed';


--
-- Name: COLUMN "AuditLogs"."TenantId"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."AuditLogs"."TenantId" IS 'Foreign key to the tenant where the action occurred';


--
-- Name: COLUMN "AuditLogs"."UserId"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."AuditLogs"."UserId" IS 'Foreign key to the user who performed the action';


--
-- Name: Cities; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Cities" (
    "Id" character varying(40) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "CountryId" character varying(40) NOT NULL,
    "PostalCode" character varying(20) NOT NULL,
    "StateId" character varying(40) NOT NULL,
    "CountryEntityId" character varying(1024),
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone NOT NULL,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: ContactPersons; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ContactPersons" (
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
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: Countries; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Countries" (
    "Id" character varying(40) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "IsoCode" integer NOT NULL,
    "CityId" character varying(1024) NOT NULL,
    "GlobalRegionId" character varying(40) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone NOT NULL,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: CountryRegions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."CountryRegions" (
    "Id" character varying(40) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "CountryId" character varying(40) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: COLUMN "CountryRegions"."Id"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."CountryRegions"."Id" IS 'Primary key for country region using ULID format';


--
-- Name: COLUMN "CountryRegions"."Name"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."CountryRegions"."Name" IS 'Name of the region within the country (e.g., Syddanmark, Midtjylland)';


--
-- Name: COLUMN "CountryRegions"."CountryId"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."CountryRegions"."CountryId" IS 'Foreign key to the country this region belongs to';


--
-- Name: Credits; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Credits" (
    "Id" character varying(40) NOT NULL,
    "CreditRemaining" numeric(18,2) NOT NULL,
    "TenantId" character varying(40) NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT now() NOT NULL,
    "LastUpdatedAt" timestamp with time zone DEFAULT now() NOT NULL,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: Customers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Customers" (
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
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: DataExports; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DataExports" (
    "Id" character varying(1024) NOT NULL,
    "DownloadUrl" text,
    "FileName" character varying(1024) NOT NULL,
    "FileSize" double precision NOT NULL,
    "TenantId" character varying(1024) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: EmailAddresses; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."EmailAddresses" (
    "Id" character varying(1024) NOT NULL,
    "Address" character varying(320) NOT NULL,
    "UserId" character varying(1024) NOT NULL,
    "CustomerId" character varying(1024),
    "ContactPersonId" character varying(1024),
    "TenantId" character varying(1024),
    "ContactPersonEntityId" character varying(1024),
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: COLUMN "EmailAddresses"."Id"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."EmailAddresses"."Id" IS 'Primary key for email address';


--
-- Name: COLUMN "EmailAddresses"."Address"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."EmailAddresses"."Address" IS 'Email address following RFC 5321 standards';


--
-- Name: COLUMN "EmailAddresses"."UserId"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."EmailAddresses"."UserId" IS 'Foreign key to the user who owns this email';


--
-- Name: COLUMN "EmailAddresses"."CustomerId"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."EmailAddresses"."CustomerId" IS 'Optional foreign key to associated customer';


--
-- Name: COLUMN "EmailAddresses"."TenantId"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."EmailAddresses"."TenantId" IS 'Optional foreign key to associated tenant';


--
-- Name: EmailInvites; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."EmailInvites" (
    "Id" character varying(40) NOT NULL,
    "Token" character varying(1024) NOT NULL,
    "ReferredEmailAddress" character varying(255) NOT NULL,
    "ExpireAt" timestamp with time zone NOT NULL,
    "InviteIsUsed" boolean NOT NULL,
    "UserEntityId" character varying(40),
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: EmailVerificationEntity; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."EmailVerificationEntity" (
    "Id" integer NOT NULL,
    "Token" character varying(1024) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "ExpireAt" timestamp with time zone NOT NULL,
    "HasBeenOpened" boolean NOT NULL,
    "HasBeenVerified" boolean NOT NULL,
    "UserEntityId" character varying(1024)
);


--
-- Name: EmailVerificationEntity_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."EmailVerificationEntity" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."EmailVerificationEntity_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: EmailVerifications; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."EmailVerifications" (
    "Id" character varying(40) NOT NULL,
    "Token" character varying(1024) NOT NULL,
    "ExpireAt" timestamp with time zone NOT NULL,
    "HasBeenOpened" boolean NOT NULL,
    "HasBeenVerified" boolean NOT NULL,
    "UserEntityId" character varying(40),
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone NOT NULL,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: Families; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Families" (
    "Id" character varying(1024) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "IsActive" boolean NOT NULL,
    "Description" character varying(500) NOT NULL,
    "OwnerId" character varying(1024) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: FamilyInvites; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FamilyInvites" (
    "Id" character varying(40) NOT NULL,
    "FamilyId" character varying(40) NOT NULL,
    "UserId" character varying(40) NOT NULL,
    "ExpireAt" timestamp with time zone NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: FamilyMemberEntity; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FamilyMemberEntity" (
    "Id" character varying(1024) NOT NULL,
    "Alias" character varying(1024) NOT NULL,
    "IsActive" boolean NOT NULL,
    "UserId" character varying(1024) NOT NULL,
    "FamilyId" character varying(1024) NOT NULL,
    "FamilyId1" character varying(1024) NOT NULL,
    "FirstName" character varying(1024) NOT NULL,
    "LastName" character varying(1024) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: Files; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Files" (
    "Id" character varying(1024) NOT NULL,
    "OwnerId" character varying(1024) NOT NULL,
    "FileName" character varying(1024) NOT NULL,
    "FileSize" bigint NOT NULL,
    "FileExtension" character varying(1024) NOT NULL,
    "FilePath" character varying(1024) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: GlobalRegions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."GlobalRegions" (
    "Id" character varying(1024) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: Integrations; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Integrations" (
    "Id" character varying(1024) NOT NULL,
    "OwnerId" character varying(1024) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "ServiceName" character varying(100) NOT NULL,
    "Description" character varying(500),
    "ApiKeySecretReference" character varying(200) NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: COLUMN "Integrations"."Id"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Integrations"."Id" IS 'Primary key for integration';


--
-- Name: COLUMN "Integrations"."OwnerId"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Integrations"."OwnerId" IS 'Foreign key to the user who owns this integration';


--
-- Name: COLUMN "Integrations"."Name"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Integrations"."Name" IS 'Friendly name for the integration';


--
-- Name: COLUMN "Integrations"."ServiceName"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Integrations"."ServiceName" IS 'Name of the third-party service (e.g., Stripe, SendGrid, Twilio)';


--
-- Name: COLUMN "Integrations"."Description"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Integrations"."Description" IS 'Optional description of the integration purpose';


--
-- Name: COLUMN "Integrations"."ApiKeySecretReference"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Integrations"."ApiKeySecretReference" IS 'Reference to the securely stored API key';


--
-- Name: COLUMN "Integrations"."CreatedAt"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Integrations"."CreatedAt" IS 'Timestamp when the integration was created';


--
-- Name: COLUMN "Integrations"."LastUpdatedAt"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Integrations"."LastUpdatedAt" IS 'Timestamp when the integration was last updated';


--
-- Name: Languages; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Languages" (
    "Id" character varying(1024) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Code" character varying(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: Notifications; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Notifications" (
    "Id" character varying(40) NOT NULL,
    "OwnerId" character varying(40) NOT NULL,
    "Title" character varying(200) NOT NULL,
    "Message" character varying(1000) NOT NULL,
    "IsRead" boolean DEFAULT false NOT NULL,
    "UserId" character varying(40) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone NOT NULL,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: PaymentProviders; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."PaymentProviders" (
    "Id" character varying(40) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" character varying(500),
    "IsActive" boolean DEFAULT true NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: COLUMN "PaymentProviders"."Id"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."PaymentProviders"."Id" IS 'Unique identifier for the payment provider';


--
-- Name: COLUMN "PaymentProviders"."Name"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."PaymentProviders"."Name" IS 'Name of the payment provider (e.g., Stripe, PayPal, Square)';


--
-- Name: COLUMN "PaymentProviders"."Description"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."PaymentProviders"."Description" IS 'Optional description of the payment provider and its capabilities';


--
-- Name: COLUMN "PaymentProviders"."IsActive"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."PaymentProviders"."IsActive" IS 'Indicates if this payment provider is currently active and available for use';


--
-- Name: Permissions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Permissions" (
    "Id" character varying(40) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone NOT NULL,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: COLUMN "Permissions"."Name"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Permissions"."Name" IS 'The permission name (e.g., ''read:users'', ''write:documents'')';


--
-- Name: COLUMN "Permissions"."Description"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Permissions"."Description" IS 'Optional description of what this permission allows';


--
-- Name: COLUMN "Permissions"."CreatedAt"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Permissions"."CreatedAt" IS 'Timestamp when the permission was created';


--
-- Name: COLUMN "Permissions"."LastUpdatedAt"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Permissions"."LastUpdatedAt" IS 'Timestamp when the permission was last modified';


--
-- Name: PhoneNumbers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."PhoneNumbers" (
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
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: ProfileEntity; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ProfileEntity" (
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
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: ResourcePermissionTypes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ResourcePermissionTypes" (
    "Id" character varying(1024) NOT NULL,
    "ResourcePermissionId" character varying(1024) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: ResourcePermissions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ResourcePermissions" (
    "Id" character varying(40) NOT NULL,
    "UserId" character varying(40) NOT NULL,
    "ResourceId" character varying(40) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: RolePermissions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."RolePermissions" (
    "Id" character varying(40) NOT NULL,
    "RoleId" character varying(40) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone NOT NULL,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL,
    "PermissionId" character varying(40) DEFAULT ''::character varying NOT NULL
);


--
-- Name: Roles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Roles" (
    "Id" character varying(40) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" character varying(500),
    "UserEntityId" character varying(1024),
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone NOT NULL,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: COLUMN "Roles"."Name"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Roles"."Name" IS 'The role name (e.g., Administrator, User, Manager)';


--
-- Name: COLUMN "Roles"."Description"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Roles"."Description" IS 'Optional description of the role''s purpose and permissions';


--
-- Name: COLUMN "Roles"."CreatedAt"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Roles"."CreatedAt" IS 'Timestamp when the role was created';


--
-- Name: COLUMN "Roles"."LastUpdatedAt"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Roles"."LastUpdatedAt" IS 'Timestamp when the role was last modified';


--
-- Name: Searches; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Searches" (
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
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: Sessions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Sessions" (
    "Id" character varying(1024) NOT NULL,
    "SessionKey" character varying(100) NOT NULL,
    "SessionData" character varying(1024) NOT NULL,
    "ExpireDate" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: SignupAuditLog; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."SignupAuditLog" (
    "Id" text NOT NULL,
    "TenantId" text NOT NULL,
    "UserId" text NOT NULL,
    "Email" text NOT NULL,
    "IpAddress" text,
    "UserAgent" text,
    "CreatedAt" timestamp with time zone DEFAULT now() NOT NULL,
    "Success" boolean NOT NULL,
    "ErrorMessage" text
);


--
-- Name: TABLE "SignupAuditLog"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public."SignupAuditLog" IS 'Audit trail for all signup attempts (successful and failed).
Used for security monitoring, rate limiting, and fraud detection.';


--
-- Name: StateEntity; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."StateEntity" (
    "Id" character varying(1024) NOT NULL,
    "Name" character varying(1024) NOT NULL,
    "IsoCode" character varying(1024),
    "CountryId" character varying(1024),
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: Streets; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Streets" (
    "Id" character varying(1024) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "CityId" character varying(1024) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: Subscriptions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Subscriptions" (
    "Id" character varying(40) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" character varying(1000),
    "Code" character varying(50) NOT NULL,
    "Status" character varying(50) NOT NULL,
    "CreatedBy" character varying(40) NOT NULL,
    "UpdatedBy" character varying(40) NOT NULL,
    "TenantId" character varying(40) NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "LastUpdatedAt" timestamp with time zone NOT NULL,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: COLUMN "Subscriptions"."Id"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Subscriptions"."Id" IS 'Unique identifier for the subscription plan';


--
-- Name: COLUMN "Subscriptions"."Name"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Subscriptions"."Name" IS 'Name of the subscription plan (e.g., Basic, Pro, Enterprise)';


--
-- Name: COLUMN "Subscriptions"."Description"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Subscriptions"."Description" IS 'Detailed description of the subscription plan features and benefits';


--
-- Name: COLUMN "Subscriptions"."Code"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Subscriptions"."Code" IS 'Unique code identifier for the subscription plan';


--
-- Name: COLUMN "Subscriptions"."Status"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Subscriptions"."Status" IS 'Current status of the subscription (Active, Inactive, Discontinued, etc.)';


--
-- Name: COLUMN "Subscriptions"."CreatedBy"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Subscriptions"."CreatedBy" IS 'User ID who created this subscription';


--
-- Name: COLUMN "Subscriptions"."UpdatedBy"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Subscriptions"."UpdatedBy" IS 'User ID who last updated this subscription';


--
-- Name: COLUMN "Subscriptions"."CreatedAt"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Subscriptions"."CreatedAt" IS 'Timestamp when the subscription was created';


--
-- Name: Tenants; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Tenants" (
    "Id" character varying(40) NOT NULL,
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
    "IsSoftDeleted" boolean DEFAULT false NOT NULL,
    "TenantType" integer DEFAULT 0 NOT NULL
);


--
-- Name: COLUMN "Tenants"."Id"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."Id" IS 'Unique tenant identifier with prefix (e.g., tenant_01ABCD...)';


--
-- Name: COLUMN "Tenants"."Name"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."Name" IS 'Tenant name: Full name for Personal, Company name for Organization';


--
-- Name: COLUMN "Tenants"."Description"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."Description" IS 'Optional description, typically used for Organization tenants';


--
-- Name: COLUMN "Tenants"."IsActive"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."IsActive" IS 'Whether tenant can access the system';


--
-- Name: COLUMN "Tenants"."IsPrimary"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."IsPrimary" IS 'Indicates primary tenant for multi-tenant B2C users';


--
-- Name: COLUMN "Tenants"."Email"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."Email" IS 'Contact email for the tenant';


--
-- Name: COLUMN "Tenants"."Phone"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."Phone" IS 'Contact phone for the tenant';


--
-- Name: COLUMN "Tenants"."VatNumber"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."VatNumber" IS 'VAT/Tax number (B2B only)';


--
-- Name: COLUMN "Tenants"."Country"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."Country" IS 'Country code (B2B only)';


--
-- Name: COLUMN "Tenants"."CreatedAt"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."CreatedAt" IS 'Timestamp when tenant was created';


--
-- Name: COLUMN "Tenants"."LastUpdatedAt"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."LastUpdatedAt" IS 'Timestamp when tenant was last modified';


--
-- Name: COLUMN "Tenants"."IsSoftDeleted"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."IsSoftDeleted" IS 'Soft delete flag';


--
-- Name: COLUMN "Tenants"."TenantType"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public."Tenants"."TenantType" IS '0 = Personal (B2C), 1 = Organization (B2B)';


--
-- Name: UserRoles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserRoles" (
    "Id" character varying(40) NOT NULL,
    "UserId" character varying(40) NOT NULL,
    "RoleId" character varying(40) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone NOT NULL,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL
);


--
-- Name: Users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Users" (
    "Id" character varying(40) NOT NULL,
    "FirstName" character varying(100) NOT NULL,
    "LastName" character varying(100) NOT NULL,
    "UserName" character varying(100) NOT NULL,
    "IsActive" boolean NOT NULL,
    "Email" character varying(100) NOT NULL,
    "LastLogin" timestamp with time zone NOT NULL,
    "TenantId" character varying(40) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean DEFAULT false NOT NULL,
    "ExternalAuthId" character varying(1024)
);


--
-- Name: Webhooks; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Webhooks" (
    "Id" character varying(1024) NOT NULL,
    "Url" text NOT NULL,
    "Secret" character varying(1024) NOT NULL,
    "Description" character varying(1024),
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone,
    "IsSoftDeleted" boolean NOT NULL
);


--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


--
-- Name: Sessions AK_Sessions_SessionKey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Sessions"
    ADD CONSTRAINT "AK_Sessions_SessionKey" UNIQUE ("SessionKey");


--
-- Name: Accounts PK_Accounts; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Accounts"
    ADD CONSTRAINT "PK_Accounts" PRIMARY KEY ("Id");


--
-- Name: Addresses PK_Addresses; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Addresses"
    ADD CONSTRAINT "PK_Addresses" PRIMARY KEY ("Id");


--
-- Name: Admins PK_Admins; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Admins"
    ADD CONSTRAINT "PK_Admins" PRIMARY KEY ("Id");


--
-- Name: ApiLogs PK_ApiLogs; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ApiLogs"
    ADD CONSTRAINT "PK_ApiLogs" PRIMARY KEY ("Id");


--
-- Name: AuditLogs PK_AuditLogs; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AuditLogs"
    ADD CONSTRAINT "PK_AuditLogs" PRIMARY KEY ("Id");


--
-- Name: Cities PK_Cities; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Cities"
    ADD CONSTRAINT "PK_Cities" PRIMARY KEY ("Id");


--
-- Name: ContactPersons PK_ContactPersons; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ContactPersons"
    ADD CONSTRAINT "PK_ContactPersons" PRIMARY KEY ("Id");


--
-- Name: Countries PK_Countries; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Countries"
    ADD CONSTRAINT "PK_Countries" PRIMARY KEY ("Id");


--
-- Name: CountryRegions PK_CountryRegions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CountryRegions"
    ADD CONSTRAINT "PK_CountryRegions" PRIMARY KEY ("Id");


--
-- Name: Credits PK_Credits; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Credits"
    ADD CONSTRAINT "PK_Credits" PRIMARY KEY ("Id");


--
-- Name: Customers PK_Customers; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Customers"
    ADD CONSTRAINT "PK_Customers" PRIMARY KEY ("Id");


--
-- Name: DataExports PK_DataExports; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataExports"
    ADD CONSTRAINT "PK_DataExports" PRIMARY KEY ("Id");


--
-- Name: EmailInvites PK_EmailInvites; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmailInvites"
    ADD CONSTRAINT "PK_EmailInvites" PRIMARY KEY ("Id");


--
-- Name: EmailVerificationEntity PK_EmailVerificationEntity; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmailVerificationEntity"
    ADD CONSTRAINT "PK_EmailVerificationEntity" PRIMARY KEY ("Id");


--
-- Name: EmailVerifications PK_EmailVerifications; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmailVerifications"
    ADD CONSTRAINT "PK_EmailVerifications" PRIMARY KEY ("Id");


--
-- Name: EmailAddresses PK_Emails; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmailAddresses"
    ADD CONSTRAINT "PK_Emails" PRIMARY KEY ("Id");


--
-- Name: Families PK_Families; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Families"
    ADD CONSTRAINT "PK_Families" PRIMARY KEY ("Id");


--
-- Name: FamilyInvites PK_FamilyInvites; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FamilyInvites"
    ADD CONSTRAINT "PK_FamilyInvites" PRIMARY KEY ("Id");


--
-- Name: FamilyMemberEntity PK_FamilyMemberEntity; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FamilyMemberEntity"
    ADD CONSTRAINT "PK_FamilyMemberEntity" PRIMARY KEY ("Id");


--
-- Name: Files PK_Files; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Files"
    ADD CONSTRAINT "PK_Files" PRIMARY KEY ("Id");


--
-- Name: GlobalRegions PK_GlobalRegions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."GlobalRegions"
    ADD CONSTRAINT "PK_GlobalRegions" PRIMARY KEY ("Id");


--
-- Name: Integrations PK_Integrations; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Integrations"
    ADD CONSTRAINT "PK_Integrations" PRIMARY KEY ("Id");


--
-- Name: Languages PK_Languages; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Languages"
    ADD CONSTRAINT "PK_Languages" PRIMARY KEY ("Id");


--
-- Name: Notifications PK_Notifications; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Notifications"
    ADD CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id");


--
-- Name: PaymentProviders PK_PaymentProviders; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PaymentProviders"
    ADD CONSTRAINT "PK_PaymentProviders" PRIMARY KEY ("Id");


--
-- Name: Permissions PK_Permissions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Permissions"
    ADD CONSTRAINT "PK_Permissions" PRIMARY KEY ("Id");


--
-- Name: PhoneNumbers PK_PhoneNumbers; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PhoneNumbers"
    ADD CONSTRAINT "PK_PhoneNumbers" PRIMARY KEY ("Id");


--
-- Name: ProfileEntity PK_ProfileEntity; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ProfileEntity"
    ADD CONSTRAINT "PK_ProfileEntity" PRIMARY KEY ("Id");


--
-- Name: ResourcePermissionTypes PK_ResourcePermissionTypes; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ResourcePermissionTypes"
    ADD CONSTRAINT "PK_ResourcePermissionTypes" PRIMARY KEY ("Id");


--
-- Name: ResourcePermissions PK_ResourcePermissions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ResourcePermissions"
    ADD CONSTRAINT "PK_ResourcePermissions" PRIMARY KEY ("Id");


--
-- Name: RolePermissions PK_RolePermissions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."RolePermissions"
    ADD CONSTRAINT "PK_RolePermissions" PRIMARY KEY ("Id");


--
-- Name: Roles PK_Roles; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Roles"
    ADD CONSTRAINT "PK_Roles" PRIMARY KEY ("Id");


--
-- Name: Searches PK_Searches; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Searches"
    ADD CONSTRAINT "PK_Searches" PRIMARY KEY ("Id");


--
-- Name: Sessions PK_Sessions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Sessions"
    ADD CONSTRAINT "PK_Sessions" PRIMARY KEY ("Id");


--
-- Name: StateEntity PK_StateEntity; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StateEntity"
    ADD CONSTRAINT "PK_StateEntity" PRIMARY KEY ("Id");


--
-- Name: Streets PK_Streets; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Streets"
    ADD CONSTRAINT "PK_Streets" PRIMARY KEY ("Id");


--
-- Name: Subscriptions PK_Subscriptions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Subscriptions"
    ADD CONSTRAINT "PK_Subscriptions" PRIMARY KEY ("Id");


--
-- Name: Tenants PK_Tenants; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Tenants"
    ADD CONSTRAINT "PK_Tenants" PRIMARY KEY ("Id");


--
-- Name: UserRoles PK_UserRoles; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "PK_UserRoles" PRIMARY KEY ("Id");


--
-- Name: Users PK_Users; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "PK_Users" PRIMARY KEY ("Id");


--
-- Name: Webhooks PK_Webhooks; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Webhooks"
    ADD CONSTRAINT "PK_Webhooks" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: SignupAuditLog SignupAuditLog_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SignupAuditLog"
    ADD CONSTRAINT "SignupAuditLog_pkey" PRIMARY KEY ("Id");


--
-- Name: IX_Accounts_Email; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Accounts_Email" ON public."Accounts" USING btree ("Email");


--
-- Name: IX_Accounts_OwnerId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Accounts_OwnerId" ON public."Accounts" USING btree ("OwnerId");


--
-- Name: IX_Addresses_CityId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Addresses_CityId" ON public."Addresses" USING btree ("CityId");


--
-- Name: IX_Addresses_ContactPersonEntityId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Addresses_ContactPersonEntityId" ON public."Addresses" USING btree ("ContactPersonEntityId");


--
-- Name: IX_Addresses_CountryId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Addresses_CountryId" ON public."Addresses" USING btree ("CountryId");


--
-- Name: IX_Addresses_CustomerId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Addresses_CustomerId" ON public."Addresses" USING btree ("CustomerId");


--
-- Name: IX_Addresses_PostalCode; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Addresses_PostalCode" ON public."Addresses" USING btree ("PostalCode");


--
-- Name: IX_Addresses_StreetId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Addresses_StreetId" ON public."Addresses" USING btree ("StreetId");


--
-- Name: IX_Addresses_TenantId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Addresses_TenantId" ON public."Addresses" USING btree ("TenantId");


--
-- Name: IX_Addresses_UserEntityId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Addresses_UserEntityId" ON public."Addresses" USING btree ("UserEntityId");


--
-- Name: IX_Admins_Email; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Admins_Email" ON public."Admins" USING btree ("Email");


--
-- Name: IX_ApiLogs_ApiKeyId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ApiLogs_ApiKeyId" ON public."ApiLogs" USING btree ("ApiKeyId");


--
-- Name: IX_ApiLogs_ApiKeyId_StatusCode; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ApiLogs_ApiKeyId_StatusCode" ON public."ApiLogs" USING btree ("ApiKeyId", "StatusCode");


--
-- Name: IX_ApiLogs_RequestMethod; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ApiLogs_RequestMethod" ON public."ApiLogs" USING btree ("RequestMethod");


--
-- Name: IX_ApiLogs_SessionId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ApiLogs_SessionId" ON public."ApiLogs" USING btree ("SessionId");


--
-- Name: IX_ApiLogs_SessionId_StatusCode; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ApiLogs_SessionId_StatusCode" ON public."ApiLogs" USING btree ("SessionId", "StatusCode");


--
-- Name: IX_ApiLogs_SourceIp; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ApiLogs_SourceIp" ON public."ApiLogs" USING btree ("SourceIp");


--
-- Name: IX_ApiLogs_StatusCode; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ApiLogs_StatusCode" ON public."ApiLogs" USING btree ("StatusCode");


--
-- Name: IX_AuditLogs_Category; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AuditLogs_Category" ON public."AuditLogs" USING btree ("Category") WHERE ("Category" IS NOT NULL);


--
-- Name: IX_AuditLogs_Category_ModifiedAt; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AuditLogs_Category_ModifiedAt" ON public."AuditLogs" USING btree ("Category", "ModifiedAt") WHERE ("Category" IS NOT NULL);


--
-- Name: IX_AuditLogs_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AuditLogs_IsSoftDeleted" ON public."AuditLogs" USING btree ("IsSoftDeleted");


--
-- Name: IX_AuditLogs_ModifiedAt; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AuditLogs_ModifiedAt" ON public."AuditLogs" USING btree ("ModifiedAt");


--
-- Name: IX_AuditLogs_ModifiedByUserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AuditLogs_ModifiedByUserId" ON public."AuditLogs" USING btree ("ModifiedByUserId");


--
-- Name: IX_AuditLogs_TenantId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AuditLogs_TenantId" ON public."AuditLogs" USING btree ("TenantId");


--
-- Name: IX_AuditLogs_TenantId_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AuditLogs_TenantId_IsSoftDeleted" ON public."AuditLogs" USING btree ("TenantId", "IsSoftDeleted");


--
-- Name: IX_AuditLogs_TenantId_ModifiedAt; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AuditLogs_TenantId_ModifiedAt" ON public."AuditLogs" USING btree ("TenantId", "ModifiedAt");


--
-- Name: IX_AuditLogs_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AuditLogs_UserId" ON public."AuditLogs" USING btree ("UserId");


--
-- Name: IX_AuditLogs_UserId_ModifiedAt; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AuditLogs_UserId_ModifiedAt" ON public."AuditLogs" USING btree ("UserId", "ModifiedAt");


--
-- Name: IX_Cities_CountryEntityId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Cities_CountryEntityId" ON public."Cities" USING btree ("CountryEntityId");


--
-- Name: IX_Cities_CountryId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Cities_CountryId" ON public."Cities" USING btree ("CountryId");


--
-- Name: IX_Cities_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Cities_IsSoftDeleted" ON public."Cities" USING btree ("IsSoftDeleted");


--
-- Name: IX_Cities_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Cities_Name" ON public."Cities" USING btree ("Name");


--
-- Name: IX_Cities_PostalCode; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Cities_PostalCode" ON public."Cities" USING btree ("PostalCode");


--
-- Name: IX_Cities_StateId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Cities_StateId" ON public."Cities" USING btree ("StateId");


--
-- Name: IX_Cities_StateId_PostalCode_Unique; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Cities_StateId_PostalCode_Unique" ON public."Cities" USING btree ("StateId", "PostalCode");


--
-- Name: IX_ContactPersons_CustomerId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ContactPersons_CustomerId" ON public."ContactPersons" USING btree ("CustomerId");


--
-- Name: IX_ContactPersons_CustomerId1; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ContactPersons_CustomerId1" ON public."ContactPersons" USING btree ("CustomerId1");


--
-- Name: IX_ContactPersons_Id; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_ContactPersons_Id" ON public."ContactPersons" USING btree ("Id");


--
-- Name: IX_ContactPersons_TenantId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ContactPersons_TenantId" ON public."ContactPersons" USING btree ("TenantId");


--
-- Name: IX_ContactPersons_TenantId1; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ContactPersons_TenantId1" ON public."ContactPersons" USING btree ("TenantId1");


--
-- Name: IX_Countries_GlobalRegionId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Countries_GlobalRegionId" ON public."Countries" USING btree ("GlobalRegionId");


--
-- Name: IX_Countries_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Countries_IsSoftDeleted" ON public."Countries" USING btree ("IsSoftDeleted");


--
-- Name: IX_Countries_IsoCode; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Countries_IsoCode" ON public."Countries" USING btree ("IsoCode");


--
-- Name: IX_Countries_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Countries_Name" ON public."Countries" USING btree ("Name");


--
-- Name: IX_CountryRegions_CountryId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_CountryRegions_CountryId" ON public."CountryRegions" USING btree ("CountryId");


--
-- Name: IX_CountryRegions_CountryId_Name_Unique; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_CountryRegions_CountryId_Name_Unique" ON public."CountryRegions" USING btree ("CountryId", "Name");


--
-- Name: IX_CountryRegions_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_CountryRegions_Name" ON public."CountryRegions" USING btree ("Name");


--
-- Name: IX_Credits_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Credits_IsSoftDeleted" ON public."Credits" USING btree ("IsSoftDeleted");


--
-- Name: IX_Credits_TenantId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Credits_TenantId" ON public."Credits" USING btree ("TenantId");


--
-- Name: IX_Credits_TenantId_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Credits_TenantId_IsSoftDeleted" ON public."Credits" USING btree ("TenantId", "IsSoftDeleted");


--
-- Name: IX_Customers_CustomerType; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Customers_CustomerType" ON public."Customers" USING btree ("CustomerType");


--
-- Name: IX_Customers_Id; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Customers_Id" ON public."Customers" USING btree ("Id");


--
-- Name: IX_Customers_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Customers_IsSoftDeleted" ON public."Customers" USING btree ("IsSoftDeleted");


--
-- Name: IX_Customers_OrganizationId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Customers_OrganizationId" ON public."Customers" USING btree ("OrganizationId");


--
-- Name: IX_Customers_StripeCustomerId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Customers_StripeCustomerId" ON public."Customers" USING btree ("StripeCustomerId");


--
-- Name: IX_Customers_Type; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Customers_Type" ON public."Customers" USING btree ("Type");


--
-- Name: IX_Customers_VatNumber; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Customers_VatNumber" ON public."Customers" USING btree ("VatNumber");


--
-- Name: IX_EmailAddresses_ContactPersonEntityId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmailAddresses_ContactPersonEntityId" ON public."EmailAddresses" USING btree ("ContactPersonEntityId");


--
-- Name: IX_EmailAddresses_ContactPersonId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmailAddresses_ContactPersonId" ON public."EmailAddresses" USING btree ("ContactPersonId");


--
-- Name: IX_EmailInvites_ExpireAt; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmailInvites_ExpireAt" ON public."EmailInvites" USING btree ("ExpireAt");


--
-- Name: IX_EmailInvites_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmailInvites_IsSoftDeleted" ON public."EmailInvites" USING btree ("IsSoftDeleted");


--
-- Name: IX_EmailInvites_ReferredEmailAddress; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmailInvites_ReferredEmailAddress" ON public."EmailInvites" USING btree ("ReferredEmailAddress");


--
-- Name: IX_EmailInvites_Token; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_EmailInvites_Token" ON public."EmailInvites" USING btree ("Token");


--
-- Name: IX_EmailInvites_UserEntityId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmailInvites_UserEntityId" ON public."EmailInvites" USING btree ("UserEntityId");


--
-- Name: IX_EmailVerificationEntity_UserEntityId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmailVerificationEntity_UserEntityId" ON public."EmailVerificationEntity" USING btree ("UserEntityId");


--
-- Name: IX_EmailVerifications_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmailVerifications_IsSoftDeleted" ON public."EmailVerifications" USING btree ("IsSoftDeleted");


--
-- Name: IX_EmailVerifications_Token; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmailVerifications_Token" ON public."EmailVerifications" USING btree ("Token");


--
-- Name: IX_EmailVerifications_UserEntityId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmailVerifications_UserEntityId" ON public."EmailVerifications" USING btree ("UserEntityId");


--
-- Name: IX_Emails_Address_Unique; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Emails_Address_Unique" ON public."EmailAddresses" USING btree ("Address");


--
-- Name: IX_Emails_CustomerId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Emails_CustomerId" ON public."EmailAddresses" USING btree ("CustomerId") WHERE ("CustomerId" IS NOT NULL);


--
-- Name: IX_Emails_TenantId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Emails_TenantId" ON public."EmailAddresses" USING btree ("TenantId") WHERE ("TenantId" IS NOT NULL);


--
-- Name: IX_Emails_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Emails_UserId" ON public."EmailAddresses" USING btree ("UserId");


--
-- Name: IX_Emails_UserId_CustomerId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Emails_UserId_CustomerId" ON public."EmailAddresses" USING btree ("UserId", "CustomerId");


--
-- Name: IX_Families_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Families_Name" ON public."Families" USING btree ("Name");


--
-- Name: IX_Families_OwnerId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Families_OwnerId" ON public."Families" USING btree ("OwnerId");


--
-- Name: IX_FamilyInvites_FamilyId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FamilyInvites_FamilyId" ON public."FamilyInvites" USING btree ("FamilyId");


--
-- Name: IX_FamilyInvites_Id; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FamilyInvites_Id" ON public."FamilyInvites" USING btree ("Id");


--
-- Name: IX_FamilyInvites_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FamilyInvites_IsSoftDeleted" ON public."FamilyInvites" USING btree ("IsSoftDeleted");


--
-- Name: IX_FamilyInvites_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FamilyInvites_UserId" ON public."FamilyInvites" USING btree ("UserId");


--
-- Name: IX_FamilyMemberEntity_FamilyId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FamilyMemberEntity_FamilyId" ON public."FamilyMemberEntity" USING btree ("FamilyId");


--
-- Name: IX_FamilyMemberEntity_FamilyId1; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FamilyMemberEntity_FamilyId1" ON public."FamilyMemberEntity" USING btree ("FamilyId1");


--
-- Name: IX_FamilyMemberEntity_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FamilyMemberEntity_UserId" ON public."FamilyMemberEntity" USING btree ("UserId");


--
-- Name: IX_GlobalRegions_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_GlobalRegions_Name" ON public."GlobalRegions" USING btree ("Name");


--
-- Name: IX_Integrations_CreatedAt; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Integrations_CreatedAt" ON public."Integrations" USING btree ("CreatedAt");


--
-- Name: IX_Integrations_OwnerId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Integrations_OwnerId" ON public."Integrations" USING btree ("OwnerId");


--
-- Name: IX_Integrations_OwnerId_ServiceName_Unique; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Integrations_OwnerId_ServiceName_Unique" ON public."Integrations" USING btree ("OwnerId", "ServiceName");


--
-- Name: IX_Integrations_ServiceName; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Integrations_ServiceName" ON public."Integrations" USING btree ("ServiceName");


--
-- Name: IX_Integrations_ServiceName_CreatedAt; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Integrations_ServiceName_CreatedAt" ON public."Integrations" USING btree ("ServiceName", "CreatedAt");


--
-- Name: IX_Languages_Code; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Languages_Code" ON public."Languages" USING btree ("Code");


--
-- Name: IX_Languages_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Languages_Name" ON public."Languages" USING btree ("Name");


--
-- Name: IX_Notifications_CreatedAt; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Notifications_CreatedAt" ON public."Notifications" USING btree ("CreatedAt");


--
-- Name: IX_Notifications_IsRead; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Notifications_IsRead" ON public."Notifications" USING btree ("IsRead");


--
-- Name: IX_Notifications_OwnerId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Notifications_OwnerId" ON public."Notifications" USING btree ("OwnerId");


--
-- Name: IX_Notifications_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Notifications_UserId" ON public."Notifications" USING btree ("UserId");


--
-- Name: IX_PaymentProviders_CreatedAt; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_PaymentProviders_CreatedAt" ON public."PaymentProviders" USING btree ("CreatedAt");


--
-- Name: IX_PaymentProviders_IsActive; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_PaymentProviders_IsActive" ON public."PaymentProviders" USING btree ("IsActive");


--
-- Name: IX_PaymentProviders_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_PaymentProviders_Name" ON public."PaymentProviders" USING btree ("Name");


--
-- Name: IX_Permissions_CreatedAt; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Permissions_CreatedAt" ON public."Permissions" USING btree ("CreatedAt");


--
-- Name: IX_Permissions_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Permissions_Name" ON public."Permissions" USING btree ("Name");


--
-- Name: IX_PhoneNumbers_ContactPersonId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_PhoneNumbers_ContactPersonId" ON public."PhoneNumbers" USING btree ("ContactPersonId");


--
-- Name: IX_PhoneNumbers_CustomerId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_PhoneNumbers_CustomerId" ON public."PhoneNumbers" USING btree ("CustomerId");


--
-- Name: IX_PhoneNumbers_Id; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_PhoneNumbers_Id" ON public."PhoneNumbers" USING btree ("Id");


--
-- Name: IX_PhoneNumbers_TenantId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_PhoneNumbers_TenantId" ON public."PhoneNumbers" USING btree ("TenantId");


--
-- Name: IX_PhoneNumbers_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_PhoneNumbers_UserId" ON public."PhoneNumbers" USING btree ("UserId");


--
-- Name: IX_ProfileEntity_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_ProfileEntity_UserId" ON public."ProfileEntity" USING btree ("UserId");


--
-- Name: IX_ResourcePermissionTypes_ResourcePermissionId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ResourcePermissionTypes_ResourcePermissionId" ON public."ResourcePermissionTypes" USING btree ("ResourcePermissionId");


--
-- Name: IX_ResourcePermissions_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ResourcePermissions_IsSoftDeleted" ON public."ResourcePermissions" USING btree ("IsSoftDeleted");


--
-- Name: IX_ResourcePermissions_ResourceId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ResourcePermissions_ResourceId" ON public."ResourcePermissions" USING btree ("ResourceId");


--
-- Name: IX_ResourcePermissions_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ResourcePermissions_UserId" ON public."ResourcePermissions" USING btree ("UserId");


--
-- Name: IX_ResourcePermissions_UserId_ResourceId_Unique; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_ResourcePermissions_UserId_ResourceId_Unique" ON public."ResourcePermissions" USING btree ("UserId", "ResourceId");


--
-- Name: IX_RolePermissions_PermissionId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_RolePermissions_PermissionId" ON public."RolePermissions" USING btree ("PermissionId");


--
-- Name: IX_RolePermissions_RoleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_RolePermissions_RoleId" ON public."RolePermissions" USING btree ("RoleId");


--
-- Name: IX_RolePermissions_RoleId_PermissionId_Unique; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_RolePermissions_RoleId_PermissionId_Unique" ON public."RolePermissions" USING btree ("RoleId", "PermissionId");


--
-- Name: IX_Roles_CreatedAt; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Roles_CreatedAt" ON public."Roles" USING btree ("CreatedAt");


--
-- Name: IX_Roles_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Roles_Name" ON public."Roles" USING btree ("Name");


--
-- Name: IX_Roles_UserEntityId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Roles_UserEntityId" ON public."Roles" USING btree ("UserEntityId");


--
-- Name: IX_Sessions_SessionKey; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Sessions_SessionKey" ON public."Sessions" USING btree ("SessionKey");


--
-- Name: IX_StateEntity_CountryId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StateEntity_CountryId" ON public."StateEntity" USING btree ("CountryId");


--
-- Name: IX_Streets_CityId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Streets_CityId" ON public."Streets" USING btree ("CityId");


--
-- Name: IX_Streets_CityId_Name_Unique; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Streets_CityId_Name_Unique" ON public."Streets" USING btree ("CityId", "Name");


--
-- Name: IX_Streets_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Streets_Name" ON public."Streets" USING btree ("Name");


--
-- Name: IX_Subscriptions_Code; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Subscriptions_Code" ON public."Subscriptions" USING btree ("Code");


--
-- Name: IX_Subscriptions_CreatedAt; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Subscriptions_CreatedAt" ON public."Subscriptions" USING btree ("CreatedAt");


--
-- Name: IX_Subscriptions_CreatedBy; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Subscriptions_CreatedBy" ON public."Subscriptions" USING btree ("CreatedBy");


--
-- Name: IX_Subscriptions_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Subscriptions_Name" ON public."Subscriptions" USING btree ("Name");


--
-- Name: IX_Subscriptions_Status; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Subscriptions_Status" ON public."Subscriptions" USING btree ("Status");


--
-- Name: IX_Subscriptions_TenantId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Subscriptions_TenantId" ON public."Subscriptions" USING btree ("TenantId");


--
-- Name: IX_Tenants_Country; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Tenants_Country" ON public."Tenants" USING btree ("Country");


--
-- Name: IX_Tenants_CustomerEntityId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Tenants_CustomerEntityId" ON public."Tenants" USING btree ("CustomerEntityId");


--
-- Name: IX_Tenants_CustomerId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Tenants_CustomerId" ON public."Tenants" USING btree ("CustomerId");


--
-- Name: IX_Tenants_Email_TenantType; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Tenants_Email_TenantType" ON public."Tenants" USING btree ("Email", "TenantType");


--
-- Name: IX_Tenants_Id; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Tenants_Id" ON public."Tenants" USING btree ("Id");


--
-- Name: IX_Tenants_IsActive; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Tenants_IsActive" ON public."Tenants" USING btree ("IsActive");


--
-- Name: IX_Tenants_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Tenants_IsSoftDeleted" ON public."Tenants" USING btree ("IsSoftDeleted");


--
-- Name: IX_Tenants_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Tenants_Name" ON public."Tenants" USING btree ("Name");


--
-- Name: IX_Tenants_TenantType; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Tenants_TenantType" ON public."Tenants" USING btree ("TenantType");


--
-- Name: IX_Tenants_Type_Active_NotDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Tenants_Type_Active_NotDeleted" ON public."Tenants" USING btree ("TenantType", "IsActive", "IsSoftDeleted");


--
-- Name: IX_Tenants_VatNumber; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Tenants_VatNumber" ON public."Tenants" USING btree ("VatNumber");


--
-- Name: IX_UserRoles_RoleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserRoles_RoleId" ON public."UserRoles" USING btree ("RoleId");


--
-- Name: IX_UserRoles_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserRoles_UserId" ON public."UserRoles" USING btree ("UserId");


--
-- Name: IX_UserRoles_UserId_RoleId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_UserRoles_UserId_RoleId" ON public."UserRoles" USING btree ("UserId", "RoleId");


--
-- Name: IX_Users_Email; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Users_Email" ON public."Users" USING btree ("Email");


--
-- Name: IX_Users_Id; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Users_Id" ON public."Users" USING btree ("Id");


--
-- Name: IX_Users_IsActive; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Users_IsActive" ON public."Users" USING btree ("IsActive");


--
-- Name: IX_Users_IsSoftDeleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Users_IsSoftDeleted" ON public."Users" USING btree ("IsSoftDeleted");


--
-- Name: IX_Users_TenantId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Users_TenantId" ON public."Users" USING btree ("TenantId");


--
-- Name: IX_Users_UserName; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Users_UserName" ON public."Users" USING btree ("UserName");


--
-- Name: idx_signup_audit_email_created; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_signup_audit_email_created ON public."SignupAuditLog" USING btree ("Email", "CreatedAt" DESC);


--
-- Name: idx_signup_audit_ip_created; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_signup_audit_ip_created ON public."SignupAuditLog" USING btree ("IpAddress", "CreatedAt" DESC);


--
-- Name: Accounts FK_Accounts_Users_OwnerId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Accounts"
    ADD CONSTRAINT "FK_Accounts_Users_OwnerId" FOREIGN KEY ("OwnerId") REFERENCES public."Users"("Id");


--
-- Name: Addresses FK_Addresses_Cities_CityId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Addresses"
    ADD CONSTRAINT "FK_Addresses_Cities_CityId" FOREIGN KEY ("CityId") REFERENCES public."Cities"("Id") ON DELETE RESTRICT;


--
-- Name: Addresses FK_Addresses_ContactPersons_ContactPersonEntityId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Addresses"
    ADD CONSTRAINT "FK_Addresses_ContactPersons_ContactPersonEntityId" FOREIGN KEY ("ContactPersonEntityId") REFERENCES public."ContactPersons"("Id");


--
-- Name: Addresses FK_Addresses_Countries_CountryId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Addresses"
    ADD CONSTRAINT "FK_Addresses_Countries_CountryId" FOREIGN KEY ("CountryId") REFERENCES public."Countries"("Id") ON DELETE RESTRICT;


--
-- Name: Addresses FK_Addresses_Customers_CustomerId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Addresses"
    ADD CONSTRAINT "FK_Addresses_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public."Customers"("Id") ON DELETE CASCADE;


--
-- Name: Addresses FK_Addresses_Streets_StreetId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Addresses"
    ADD CONSTRAINT "FK_Addresses_Streets_StreetId" FOREIGN KEY ("StreetId") REFERENCES public."Streets"("Id") ON DELETE RESTRICT;


--
-- Name: Addresses FK_Addresses_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Addresses"
    ADD CONSTRAINT "FK_Addresses_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE CASCADE;


--
-- Name: Addresses FK_Addresses_Users_UserEntityId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Addresses"
    ADD CONSTRAINT "FK_Addresses_Users_UserEntityId" FOREIGN KEY ("UserEntityId") REFERENCES public."Users"("Id");


--
-- Name: ApiLogs FK_ApiLogs_Sessions_SessionId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ApiLogs"
    ADD CONSTRAINT "FK_ApiLogs_Sessions_SessionId" FOREIGN KEY ("SessionId") REFERENCES public."Sessions"("SessionKey") ON DELETE RESTRICT;


--
-- Name: AuditLogs FK_AuditLogs_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AuditLogs"
    ADD CONSTRAINT "FK_AuditLogs_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: AuditLogs FK_AuditLogs_Users_ModifiedByUserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AuditLogs"
    ADD CONSTRAINT "FK_AuditLogs_Users_ModifiedByUserId" FOREIGN KEY ("ModifiedByUserId") REFERENCES public."Users"("Id") ON DELETE RESTRICT;


--
-- Name: AuditLogs FK_AuditLogs_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AuditLogs"
    ADD CONSTRAINT "FK_AuditLogs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE RESTRICT;


--
-- Name: Cities FK_Cities_Countries_CountryEntityId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Cities"
    ADD CONSTRAINT "FK_Cities_Countries_CountryEntityId" FOREIGN KEY ("CountryEntityId") REFERENCES public."Countries"("Id");


--
-- Name: Cities FK_Cities_Countries_CountryId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Cities"
    ADD CONSTRAINT "FK_Cities_Countries_CountryId" FOREIGN KEY ("CountryId") REFERENCES public."Countries"("Id") ON DELETE RESTRICT;


--
-- Name: Cities FK_Cities_States_StateId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Cities"
    ADD CONSTRAINT "FK_Cities_States_StateId" FOREIGN KEY ("StateId") REFERENCES public."StateEntity"("Id") ON DELETE RESTRICT;


--
-- Name: ContactPersons FK_ContactPersons_Customers_CustomerId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ContactPersons"
    ADD CONSTRAINT "FK_ContactPersons_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public."Customers"("Id") ON DELETE CASCADE;


--
-- Name: ContactPersons FK_ContactPersons_Customers_CustomerId1; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ContactPersons"
    ADD CONSTRAINT "FK_ContactPersons_Customers_CustomerId1" FOREIGN KEY ("CustomerId1") REFERENCES public."Customers"("Id");


--
-- Name: ContactPersons FK_ContactPersons_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ContactPersons"
    ADD CONSTRAINT "FK_ContactPersons_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE CASCADE;


--
-- Name: ContactPersons FK_ContactPersons_Tenants_TenantId1; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ContactPersons"
    ADD CONSTRAINT "FK_ContactPersons_Tenants_TenantId1" FOREIGN KEY ("TenantId1") REFERENCES public."Tenants"("Id");


--
-- Name: Countries FK_Countries_GlobalRegions_GlobalRegionId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Countries"
    ADD CONSTRAINT "FK_Countries_GlobalRegions_GlobalRegionId" FOREIGN KEY ("GlobalRegionId") REFERENCES public."GlobalRegions"("Id") ON DELETE RESTRICT;


--
-- Name: CountryRegions FK_CountryRegions_Countries_CountryId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CountryRegions"
    ADD CONSTRAINT "FK_CountryRegions_Countries_CountryId" FOREIGN KEY ("CountryId") REFERENCES public."Countries"("Id") ON DELETE CASCADE;


--
-- Name: EmailAddresses FK_EmailAddresses_ContactPersons_ContactPersonEntityId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmailAddresses"
    ADD CONSTRAINT "FK_EmailAddresses_ContactPersons_ContactPersonEntityId" FOREIGN KEY ("ContactPersonEntityId") REFERENCES public."ContactPersons"("Id");


--
-- Name: EmailAddresses FK_EmailAddresses_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmailAddresses"
    ADD CONSTRAINT "FK_EmailAddresses_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: EmailInvites FK_EmailInvites_Users_UserEntityId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmailInvites"
    ADD CONSTRAINT "FK_EmailInvites_Users_UserEntityId" FOREIGN KEY ("UserEntityId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: EmailVerificationEntity FK_EmailVerificationEntity_Users_UserEntityId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmailVerificationEntity"
    ADD CONSTRAINT "FK_EmailVerificationEntity_Users_UserEntityId" FOREIGN KEY ("UserEntityId") REFERENCES public."Users"("Id");


--
-- Name: EmailVerifications FK_EmailVerifications_Users_UserEntityId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmailVerifications"
    ADD CONSTRAINT "FK_EmailVerifications_Users_UserEntityId" FOREIGN KEY ("UserEntityId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: EmailAddresses FK_Emails_ContactPersons_ContactPersonId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmailAddresses"
    ADD CONSTRAINT "FK_Emails_ContactPersons_ContactPersonId" FOREIGN KEY ("ContactPersonId") REFERENCES public."ContactPersons"("Id") ON DELETE SET NULL;


--
-- Name: EmailAddresses FK_Emails_Customers_CustomerId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmailAddresses"
    ADD CONSTRAINT "FK_Emails_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public."Customers"("Id") ON DELETE SET NULL;


--
-- Name: EmailAddresses FK_Emails_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmailAddresses"
    ADD CONSTRAINT "FK_Emails_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE SET NULL;


--
-- Name: Families FK_Families_Users_OwnerId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Families"
    ADD CONSTRAINT "FK_Families_Users_OwnerId" FOREIGN KEY ("OwnerId") REFERENCES public."Users"("Id") ON DELETE RESTRICT;


--
-- Name: FamilyInvites FK_FamilyInvites_Families_FamilyId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FamilyInvites"
    ADD CONSTRAINT "FK_FamilyInvites_Families_FamilyId" FOREIGN KEY ("FamilyId") REFERENCES public."Families"("Id") ON DELETE CASCADE;


--
-- Name: FamilyInvites FK_FamilyInvites_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FamilyInvites"
    ADD CONSTRAINT "FK_FamilyInvites_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE RESTRICT;


--
-- Name: FamilyMemberEntity FK_FamilyMemberEntity_Families_FamilyId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FamilyMemberEntity"
    ADD CONSTRAINT "FK_FamilyMemberEntity_Families_FamilyId" FOREIGN KEY ("FamilyId") REFERENCES public."Families"("Id") ON DELETE CASCADE;


--
-- Name: FamilyMemberEntity FK_FamilyMemberEntity_Families_FamilyId1; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FamilyMemberEntity"
    ADD CONSTRAINT "FK_FamilyMemberEntity_Families_FamilyId1" FOREIGN KEY ("FamilyId1") REFERENCES public."Families"("Id") ON DELETE CASCADE;


--
-- Name: FamilyMemberEntity FK_FamilyMemberEntity_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FamilyMemberEntity"
    ADD CONSTRAINT "FK_FamilyMemberEntity_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: Integrations FK_Integrations_Users_OwnerId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Integrations"
    ADD CONSTRAINT "FK_Integrations_Users_OwnerId" FOREIGN KEY ("OwnerId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: Notifications FK_Notifications_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Notifications"
    ADD CONSTRAINT "FK_Notifications_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: PhoneNumbers FK_PhoneNumbers_ContactPersons_ContactPersonId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PhoneNumbers"
    ADD CONSTRAINT "FK_PhoneNumbers_ContactPersons_ContactPersonId" FOREIGN KEY ("ContactPersonId") REFERENCES public."ContactPersons"("Id");


--
-- Name: PhoneNumbers FK_PhoneNumbers_Customers_CustomerId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PhoneNumbers"
    ADD CONSTRAINT "FK_PhoneNumbers_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public."Customers"("Id");


--
-- Name: PhoneNumbers FK_PhoneNumbers_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PhoneNumbers"
    ADD CONSTRAINT "FK_PhoneNumbers_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE CASCADE;


--
-- Name: PhoneNumbers FK_PhoneNumbers_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PhoneNumbers"
    ADD CONSTRAINT "FK_PhoneNumbers_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id");


--
-- Name: ProfileEntity FK_ProfileEntity_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ProfileEntity"
    ADD CONSTRAINT "FK_ProfileEntity_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: ResourcePermissionTypes FK_ResourcePermissionTypes_ResourcePermissions_ResourcePermiss~; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ResourcePermissionTypes"
    ADD CONSTRAINT "FK_ResourcePermissionTypes_ResourcePermissions_ResourcePermiss~" FOREIGN KEY ("ResourcePermissionId") REFERENCES public."ResourcePermissions"("Id") ON DELETE CASCADE;


--
-- Name: ResourcePermissions FK_ResourcePermissions_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ResourcePermissions"
    ADD CONSTRAINT "FK_ResourcePermissions_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: RolePermissions FK_RolePermissions_Permissions_PermissionId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."RolePermissions"
    ADD CONSTRAINT "FK_RolePermissions_Permissions_PermissionId" FOREIGN KEY ("PermissionId") REFERENCES public."Permissions"("Id") ON DELETE CASCADE;


--
-- Name: RolePermissions FK_RolePermissions_Roles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."RolePermissions"
    ADD CONSTRAINT "FK_RolePermissions_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."Roles"("Id") ON DELETE CASCADE;


--
-- Name: Roles FK_Roles_Users_UserEntityId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Roles"
    ADD CONSTRAINT "FK_Roles_Users_UserEntityId" FOREIGN KEY ("UserEntityId") REFERENCES public."Users"("Id");


--
-- Name: StateEntity FK_StateEntity_Countries_CountryId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StateEntity"
    ADD CONSTRAINT "FK_StateEntity_Countries_CountryId" FOREIGN KEY ("CountryId") REFERENCES public."Countries"("Id");


--
-- Name: Streets FK_Streets_Cities_CityId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Streets"
    ADD CONSTRAINT "FK_Streets_Cities_CityId" FOREIGN KEY ("CityId") REFERENCES public."Cities"("Id") ON DELETE RESTRICT;


--
-- Name: Subscriptions FK_Subscriptions_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Subscriptions"
    ADD CONSTRAINT "FK_Subscriptions_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE CASCADE;


--
-- Name: Tenants FK_Tenants_Customers_CustomerEntityId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Tenants"
    ADD CONSTRAINT "FK_Tenants_Customers_CustomerEntityId" FOREIGN KEY ("CustomerEntityId") REFERENCES public."Customers"("Id");


--
-- Name: Tenants FK_Tenants_Customers_CustomerId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Tenants"
    ADD CONSTRAINT "FK_Tenants_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public."Customers"("Id") ON DELETE RESTRICT;


--
-- Name: UserRoles FK_UserRoles_Roles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "FK_UserRoles_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."Roles"("Id") ON DELETE CASCADE;


--
-- Name: UserRoles FK_UserRoles_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "FK_UserRoles_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: Users FK_Users_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "FK_Users_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--



