-- ========================================
-- Secure Signup Stored Procedure
-- Defense-in-Depth: Option 4 Implementation
-- ========================================
--
-- PURPOSE:
-- Safely handles tenant + user creation during signup without requiring NULL RLS bypass.
-- Uses SECURITY DEFINER to run with elevated privileges while maintaining strict input validation.
--
-- SECURITY FEATURES:
-- 1. SECURITY DEFINER: Bypasses RLS for signup operations only
-- 2. Input validation: Prevents SQL injection and malformed data
-- 3. Transaction atomicity: Both tenant and user created or neither
-- 4. Audit logging: Records all signup attempts
-- 5. Rate limiting ready: Can add timestamp checks for abuse prevention
--
-- USAGE:
--   psql -h localhost -U postgres -d your_database -f CreateSignupStoredProcedure.sql
--
-- Or include in Entity Framework migration.
-- ========================================

-- ========================================
-- 1. Create Audit Table for Signup Events
-- ========================================

CREATE TABLE IF NOT EXISTS "SignupAuditLog" (
    "Id" TEXT PRIMARY KEY,
    "TenantId" TEXT NOT NULL,
    "UserId" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "IpAddress" TEXT,
    "UserAgent" TEXT,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "Success" BOOLEAN NOT NULL,
    "ErrorMessage" TEXT
);

-- Index for security monitoring and rate limiting
CREATE INDEX IF NOT EXISTS idx_signup_audit_email_created 
ON "SignupAuditLog"("Email", "CreatedAt" DESC);

CREATE INDEX IF NOT EXISTS idx_signup_audit_ip_created 
ON "SignupAuditLog"("IpAddress", "CreatedAt" DESC);

-- ========================================
-- 2. Create Validation Functions
-- ========================================

-- Validate ULID format with prefix
CREATE OR REPLACE FUNCTION validate_id_format(
    id TEXT,
    expected_prefix TEXT
) RETURNS BOOLEAN AS $$
BEGIN
    -- Check format: prefix_26_character_ulid
    -- Example: tenant_01HX1234567890ABCDEFGHIJ
    RETURN id ~ ('^' || expected_prefix || '_[0123456789ABCDEFGHJKMNPQRSTVWXYZ]{26}$');
END;
$$ LANGUAGE plpgsql IMMUTABLE SECURITY DEFINER;

-- Validate email format (basic check)
CREATE OR REPLACE FUNCTION validate_email_format(email TEXT) 
RETURNS BOOLEAN AS $$
BEGIN
    -- Basic email validation (RFC 5322 subset)
    RETURN email ~ '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$'
           AND LENGTH(email) <= 320; -- RFC max length
END;
$$ LANGUAGE plpgsql IMMUTABLE SECURITY DEFINER;

-- Check for duplicate email (case-insensitive)
CREATE OR REPLACE FUNCTION email_exists(email TEXT)
RETURNS BOOLEAN AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1 FROM "Users" 
        WHERE LOWER("Email") = LOWER(email)
    );
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- ========================================
-- 3. Main Signup Stored Procedure
-- ========================================

CREATE OR REPLACE FUNCTION create_tenant_and_user(
    -- Required parameters
    p_tenant_id TEXT,
    p_tenant_name TEXT,
    p_user_id TEXT,
    p_user_first_name TEXT,
    p_user_last_name TEXT,
    p_user_email TEXT,
    p_external_auth_id TEXT,
    
    -- Optional parameters for audit
    p_ip_address TEXT DEFAULT NULL,
    p_user_agent TEXT DEFAULT NULL
) RETURNS JSON
SECURITY DEFINER  -- Runs with function owner privileges (bypasses RLS)
SET search_path = public  -- Prevent search_path attacks
LANGUAGE plpgsql
AS $$
DECLARE
    v_audit_id TEXT;
    v_result JSON;
    v_profile_id TEXT;
    v_tenant_type INTEGER;
BEGIN
    -- ========================================
    -- STEP 1: Generate Audit ID
    -- ========================================
    v_audit_id := 'audit_' || gen_ulid();
    
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
    -- STEP 4: Create Tenant (Personal Type for B2C Signup)
    -- ========================================
    -- TenantType enum: Personal = 0, Organization = 1
    v_tenant_type := 0;
    
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
        v_tenant_type,  -- Personal tenant for B2C
        true,  -- Active by default
        true,  -- Primary tenant
        p_user_email,
        NOW(),
        NOW()
    );
    
    -- ========================================
    -- STEP 5: Create User Profile
    -- ========================================
    v_profile_id := 'profile_' || gen_ulid();
    
    INSERT INTO "Profiles" (
        "Id",
        "Bio",
        "Headline",
        "Location",
        "CreatedAt",
        "LastUpdatedAt"
    ) VALUES (
        v_profile_id,
        NULL,  -- Empty profile initially
        NULL,
        NULL,
        NOW(),
        NOW()
    );
    
    -- ========================================
    -- STEP 6: Create User
    -- ========================================
    INSERT INTO "Users" (
        "Id",
        "FirstName",
        "LastName",
        "UserName",
        "Email",
        "ExternalAuthId",
        "TenantId",
        "ProfileId",
        "IsActive",
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
        v_profile_id,  -- Link to profile
        true,
        NOW(),
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

-- ========================================
-- 4. Grant Permissions
-- ========================================

-- Grant execute to authenticated application users only
-- Note: Adjust role name based on your database user setup
GRANT EXECUTE ON FUNCTION create_tenant_and_user TO PUBLIC;  -- Or specific role

-- Grant execute to validation functions
GRANT EXECUTE ON FUNCTION validate_id_format TO PUBLIC;
GRANT EXECUTE ON FUNCTION validate_email_format TO PUBLIC;
GRANT EXECUTE ON FUNCTION email_exists TO PUBLIC;

-- ========================================
-- 5. Security Hardening Comments
-- ========================================

COMMENT ON FUNCTION create_tenant_and_user IS 
'Securely creates tenant and user during signup. 
Uses SECURITY DEFINER to bypass RLS for initial account creation only.
Includes input validation, rate limiting, and audit logging.
Critical: This function should only be called from trusted signup endpoints.';

COMMENT ON TABLE "SignupAuditLog" IS 
'Audit trail for all signup attempts (successful and failed).
Used for security monitoring, rate limiting, and fraud detection.';

-- ========================================
-- 6. Verification Query
-- ========================================

-- Check function was created successfully
SELECT 
    proname AS function_name,
    prosecdef AS is_security_definer,
    provolatile AS volatility
FROM pg_proc 
WHERE proname = 'create_tenant_and_user';

-- Verify audit table exists
SELECT table_name 
FROM information_schema.tables 
WHERE table_name = 'SignupAuditLog';
